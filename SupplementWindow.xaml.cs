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
    public partial class SupplementWindow : Window
    {
        public SupplementWindow()
        {
            InitializeComponent();
            LoadSupplements();
        }

        public class Supplement
        {
            public string Name { get; set; }
            public string Dosage { get; set; }
            public string Instructions { get; set; }
            public string Benefits { get; set; }
            public string Risks { get; set; }
        }

        private void LoadSupplements()
        {
            List<Supplement> supplements = new();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Name, Dosage, Instructions, Benefits, Risks FROM Supplement";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                supplements.Add(new Supplement
                {
                    Name = reader["Name"].ToString(),
                    Dosage = reader["Dosage"].ToString(),
                    Instructions = reader["Instructions"].ToString(),
                    Benefits = reader["Benefits"].ToString(),
                    Risks = reader["Risks"].ToString()
                });
            }

            SupplementsList.ItemsSource = supplements;
        }

        private void GoToRecipes_Click(object sender, RoutedEventArgs e)
        {
            RecipesWindow rw = new RecipesWindow();
            rw.Show();
        }
        private void GoToProgress_Click(object sender, RoutedEventArgs e)
        {
            new ProgressWindow().Show();
            this.Close();
        }

        private void GoToProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfileWindow().Show();
            this.Close();
        }
        private void GoToSupplements_Click(object sender, RoutedEventArgs e)
        {
            new SupplementWindow().Show();
            this.Close();
        }

    }
}
