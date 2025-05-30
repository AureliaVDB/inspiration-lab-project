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
using KeepTrackApp.Models;

namespace KeepTrackAppUI
{
    public partial class AdminUserManagerWindow : Window
    {
        public class SimpleUser
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Password { get; set; }
            

        }

        private List<SimpleUser> users = new List<SimpleUser>();
        private string currentAdminId;

        public AdminUserManagerWindow(string adminId)
        {
            InitializeComponent();
            currentAdminId = adminId;
            LoadUsers();
            UserTable.SelectionChanged += UserTable_SelectionChanged;
        }
        


        private void LoadUsers()
        {
            users.Clear();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT UserId, Username, Email, Age, Gender, DateOfBirth, Password FROM User";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new SimpleUser
                {
                    UserId = reader["UserId"].ToString(),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    Age = Convert.ToInt32(reader["Age"]),
                    Gender = reader["Gender"].ToString(),
                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                    Password = reader["Password"].ToString()
                });
            }

            UserTable.ItemsSource = null;
            UserTable.ItemsSource = users;
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserTable.SelectedItem is not SimpleUser selectedUser)
            {
                MessageBox.Show("❌ No user selected.");
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete user {selectedUser.UserId}?", "Confirm Delete", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var delete = new MySqlCommand("DELETE FROM User WHERE UserId = @UserId", conn);
            delete.Parameters.AddWithValue("@UserId", selectedUser.UserId);
            delete.ExecuteNonQuery();

            MessageBox.Show("❌ User deleted.");
            LoadUsers();
        }

        private void UserTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserTable.SelectedItem is SimpleUser selectedUser)
            {
                UserIdBox.Text = selectedUser.UserId;
                UsernameBox.Text = selectedUser.Username;
                DobPicker.SelectedDate = selectedUser.DateOfBirth;
                GenderBox.SelectedItem = GenderBox.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == selectedUser.Gender);
                EmailBox.Text = selectedUser.Email;
                PasswordBox.Password = selectedUser.Password;
            }
        }

        private void SaveUserEdit_Click(object sender, RoutedEventArgs e)
        {
            if (UserTable.SelectedItem is not SimpleUser selectedUser)
            {
                MessageBox.Show("❌ Please select a user to edit.");
                return;
            }

            string newName = UsernameBox.Text.Trim();
            string newEmail = EmailBox.Text.Trim();
            DateTime? newDob = DobPicker.SelectedDate;
            string newGender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string newPassword = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newEmail) ||
                newDob == null || string.IsNullOrWhiteSpace(newGender) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("⚠️ Please fill in all fields.");
                return;
            }

            int newAge = User.CalculateAgeFromDob(newDob.Value);

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string updateQuery = @"
                UPDATE User 
                SET Username = @Name, 
                    Email = @Email, 
                    Password = @Password,
                    DateOfBirth = @Dob, 
                    Gender = @Gender, 
                    Age = @Age 
                WHERE UserId = @UserId";

            using var cmd = new MySqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@Name", newName);
            cmd.Parameters.AddWithValue("@Email", newEmail);
            cmd.Parameters.AddWithValue("@Password", newPassword);
            cmd.Parameters.AddWithValue("@Dob", newDob.Value);
            cmd.Parameters.AddWithValue("@Gender", newGender);
            cmd.Parameters.AddWithValue("@Age", newAge);
            cmd.Parameters.AddWithValue("@UserId", selectedUser.UserId);

            cmd.ExecuteNonQuery();

            MessageBox.Show("✅ User information updated.");
            LoadUsers();
        }

        private void GoToEditUsers_Click(object sender, RoutedEventArgs e)
        {
            new AdminUserManagerWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminRecipes_Click(object sender, RoutedEventArgs e)
        {
            new AdminRecipesWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminSupplements_Click(object sender, RoutedEventArgs e)
        {
            new AdminSupplementsWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminProgress_Click(object sender, RoutedEventArgs e)
        {
            new AdminProgressWindow(currentAdminId).Show();
            this.Close();
        }
    }
}

