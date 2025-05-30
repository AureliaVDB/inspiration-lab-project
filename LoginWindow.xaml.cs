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

namespace KeepTrackAppUI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string input = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email/UserID and password.");
                return;
            }

            // Admin login logic (hardcoded)
            if ((input.ToLower() == "aurelia" || input.ToLower() == "raja") && password == "admin")
            {
                new AdminDashboardWindow(input).Show();
                this.Close();
                return;
            }

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM User WHERE (Email = @Input OR UserId = @Input) AND Password = @Password LIMIT 1";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Input", input);
            cmd.Parameters.AddWithValue("@Password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string userId = reader["UserId"].ToString();
                new DashboardWindow(userId).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Invalid login. Try again.");
            }
        }


        private void RegisterText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close(); 
        }
    }
}
