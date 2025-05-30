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
using Microsoft.Win32;
using System.IO;



namespace KeepTrackAppUI
{
    public partial class ProfileWindow : Window
    {
        private string currentUserId;
        public ProfileWindow(string userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadUserProfile();
        }

        public ProfileWindow()
        {
            InitializeComponent();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"SELECT UserId, Username, Email, DateOfBirth, Gender, Age, ProfileImagePath
                             FROM User WHERE UserId = @UserId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                UserIdBox.Text = reader["UserId"].ToString();
                UsernameBox.Text = reader["Username"].ToString();
                EmailBox.Text = reader["Email"].ToString();
                GenderBox.Text = reader["Gender"].ToString();
                AgeBox.Text = reader["Age"].ToString();
                if (!reader.IsDBNull(reader.GetOrdinal("ProfileImagePath")))
                {
                    string imagePath = reader["ProfileImagePath"].ToString();
                    LoadProfileImage(imagePath);
                }
            }
        }

        private void UpdateName_Click(object sender, RoutedEventArgs e)
        {
            string newName = NewNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Please enter a valid name.");
                return;
            }

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string update = "UPDATE User SET Username = @Username WHERE UserId = @UserId";
            using var cmd = new MySqlCommand(update, conn);
            cmd.Parameters.AddWithValue("@Username", newName);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Name updated!");
            LoadUserProfile(); // refresh the fields
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            new ChangePasswordWindow(currentUserId, EmailBox.Text).Show();
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete your account?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // Delete progress data
            var delProgress = new MySqlCommand("DELETE FROM Progress WHERE UserId = @UserId", conn);
            delProgress.Parameters.AddWithValue("@UserId", currentUserId);
            delProgress.ExecuteNonQuery();

            // Delete user
            var delUser = new MySqlCommand("DELETE FROM User WHERE UserId = @UserId", conn);
            delUser.Parameters.AddWithValue("@UserId", currentUserId);
            delUser.ExecuteNonQuery();

            MessageBox.Show("Account deleted. The app will now close.");
            Application.Current.Shutdown();
        }

        private void GoToRecipes_Click(object sender, RoutedEventArgs e)
        {
            new RecipesWindow().Show();
        }

        private void GoToSupplements_Click(object sender, RoutedEventArgs e)
        {
            new SupplementWindow().Show();
        }

        private void GoToProgress_Click(object sender, RoutedEventArgs e)
        {
            new ProgressWindow().Show();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(selectedPath);
                string destDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProfileImages");

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                string destPath = System.IO.Path.Combine(destDir, fileName);
                File.Copy(selectedPath, destPath, true); // overwrite

                // Save the path to MySQL
                string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                string update = "UPDATE User SET ProfileImagePath = @Path WHERE UserId = @UserId";
                using var cmd = new MySqlCommand(update, conn);
                cmd.Parameters.AddWithValue("@Path", destPath);
                cmd.Parameters.AddWithValue("@UserId", currentUserId);
                cmd.ExecuteNonQuery();

                LoadProfileImage(destPath);
                MessageBox.Show("✅ Profile picture updated!");
            }
        }
        private void LoadProfileImage(string path)
        {
            if (!File.Exists(path)) return;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            ProfileImage.Source = bitmap;
        }



    }
}

