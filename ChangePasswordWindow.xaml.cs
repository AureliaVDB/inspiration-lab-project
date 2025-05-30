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
using MySql.Data.MySqlClient;
using KeepTrackApp.Utils;

namespace KeepTrackAppUI
{
    public partial class ChangePasswordWindow : Window
    {
        private string currentUserId;
        private string currentUserEmail;
        private string verificationCode;

        public ChangePasswordWindow(string userId, string userEmail)
        {
            InitializeComponent();
            currentUserId = userId;
            currentUserEmail = userEmail;
        }

        //private void SendCode_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(currentUserEmail) || !UserHelper.ValidateEmail(currentUserEmail))
        //    {
        //        ResultText.Text = "❌ Invalid email.";
        //        return;
        //    }

        //    verificationCode = UserHelper.GenerateVerificationCode();
        //    UserHelper.SendVerificationEmail(currentUserEmail, verificationCode, currentUserId);
        //    ResultText.Text = "✅ Verification code sent to email.";
        //}

        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = CurrentPasswordBox.Password.Trim();
            string newPassword = NewPasswordBox.Password.Trim();
            string confirmPassword = ConfirmNewPasswordBox.Password.Trim();
            //string code = CodeBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ResultText.Text = "❌ Please fill in all fields.";
                return;
            }

            if (!UserHelper.ValidatePassword(newPassword))
            {
                ResultText.Text = "❌ New password doesn't meet requirements.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                ResultText.Text = "❌ New passwords do not match.";
                return;
            }

            //if (code != verificationCode)
            //{
            //    ResultText.Text = "❌ Incorrect verification code.";
            //    return;
            //}

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // check old password
            string checkQuery = "SELECT Password FROM User WHERE UserId = @UserId";
            using var checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@UserId", currentUserId);

            string dbPassword = checkCmd.ExecuteScalar()?.ToString();
            if (dbPassword == null || dbPassword != oldPassword)
            {
                ResultText.Text = "❌ Current password is incorrect.";
                return;
            }

            // update password
            string updateQuery = "UPDATE User SET Password = @NewPassword WHERE UserId = @UserId";
            using var updateCmd = new MySqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@NewPassword", newPassword);
            updateCmd.Parameters.AddWithValue("@UserId", currentUserId);
            updateCmd.ExecuteNonQuery();

            MessageBox.Show("✅ Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}

