using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeepTrackApp.Utils;
using MySql.Data.MySqlClient;
using KeepTrackApp.Models;

namespace KeepTrackAppUI
{
    public partial class RegisterWindow : Window
    {
        private string verificationCode = "";

        public RegisterWindow()
        {
            InitializeComponent();
            UserIdBox.Text = UserHelper.GenerateUniqueUsername(); // auto-fill user ID
        }

        //private void SendEmail_Click(object sender, RoutedEventArgs e)
        //{
        //    string email = EmailBox.Text.Trim();
        //    if (!UserHelper.ValidateEmail(email))
        //    {
        //        ResultText.Text = "❌ Invalid email format.";
        //        return;
        //    }

        //    verificationCode = UserHelper.GenerateVerificationCode();
        //    string userId = UserIdBox.Text;
        //    UserHelper.SendVerificationEmail(email, verificationCode, userId);

        //    ResultText.Text = "✅ Verification code sent.";
        //}

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ResultText.Text = "";

            string userId = UserIdBox.Text;
            string displayName = DisplayNameBox.Text.Trim();
            //string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;
            //string code = CodeBox.Text.Trim();
            DateTime? dob = DobPicker.SelectedDate;

            // Validation
            if (string.IsNullOrWhiteSpace(displayName) || dob == null)
            {
                ResultText.Text = "❌ Please fill out all fields.";
                return;
            }

            //if (!UserHelper.ValidateEmail(email))
            //{
            //    ResultText.Text = "❌ Invalid email.";
            //    return;
            //}

            if (!UserHelper.ValidatePassword(password))
            {
                ResultText.Text = "❌ Password must meet the requirements.";
                return;
            }

            if (password != confirm)
            {
                ResultText.Text = "❌ Passwords do not match.";
                return;
            }

            //if (code != verificationCode)
            //{
            //    ResultText.Text = "❌ Incorrect confirmation code.";
            //    return;
            //}

            string gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            if (gender != "Male" && gender != "Female")
            {
                ResultText.Text = "❌ Please select a valid gender.";
                return;
            }

            int age = User.CalculateAgeFromDob(dob.Value);
            DateTime createdAt = DateTime.Now;

            // Insert into database
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertQuery = @"INSERT INTO User (UserId, Username, Email, Password, CreatedAt, DateOfBirth, Age, Gender)
                                   VALUES (@UserId, @Username, @Email, @Password, @CreatedAt, @DateOfBirth, @Age, @Gender);";

            using var cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Username", displayName);
            //cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
            cmd.Parameters.AddWithValue("@DateOfBirth", dob.Value);
            cmd.Parameters.AddWithValue("@Age", age);
            cmd.Parameters.AddWithValue("@Gender", gender);

            cmd.ExecuteNonQuery();

            MessageBox.Show("✅ Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            new LoginWindow().Show();
            this.Close();

        }
    }
}

