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
    public partial class AdminSupplementsWindow : Window
    {
        public class Supplement
        {
            public int SupplementId { get; set; }
            public string Name { get; set; }
            public string Dosage { get; set; }
            public string Instructions { get; set; }
            public string Benefits { get; set; }
            public string Risks { get; set; }
            public string AdminId { get; set; }
        }
        private string currentAdminId;
        string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";


        public AdminSupplementsWindow(string adminId)
        {
            InitializeComponent();
            currentAdminId = adminId;
            LoadSupplements();
        }

        private void LoadSupplements()
        {
            List<Supplement> supplements = new List<Supplement>();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM Supplement";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                supplements.Add(new Supplement
                {
                    SupplementId = Convert.ToInt32(reader["SupplementId"]),
                    Name = reader["Name"].ToString(),
                    Dosage = reader["Dosage"].ToString(),
                    Instructions = reader["Instructions"].ToString(),
                    Benefits = reader["Benefits"].ToString(),
                    Risks = reader["Risks"].ToString(),
                    AdminId = reader["AdminId"].ToString()
                });
            }

            SuppGrid.ItemsSource = null;
            SuppGrid.ItemsSource = supplements;
        }

        private void AddSupplement_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string dosage = DosageBox.Text.Trim();
            string instructions = InstructionsBox.Text.Trim();
            string benefits = BenefitsBox.Text.Trim();
            string risks = RisksBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(dosage) ||
                string.IsNullOrWhiteSpace(instructions) || string.IsNullOrWhiteSpace(benefits) ||
                string.IsNullOrWhiteSpace(risks))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertQuery = @"INSERT INTO Supplement 
                (Name, Dosage, Instructions, Benefits, Risks, AdminId) 
                VALUES (@Name, @Dosage, @Instructions, @Benefits, @Risks, @AdminId)";

            using var cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Dosage", dosage);
            cmd.Parameters.AddWithValue("@Instructions", instructions);
            cmd.Parameters.AddWithValue("@Benefits", benefits);
            cmd.Parameters.AddWithValue("@Risks", risks);
            cmd.Parameters.AddWithValue("@AdminId", currentAdminId);

            cmd.ExecuteNonQuery();

            MessageBox.Show("✅ Supplement added!");
            ClearForm();
            LoadSupplements();
        }


        private void ClearForm()
        {
            NameBox.Text = "";
            DosageBox.Text = "";
            InstructionsBox.Text = "";
            BenefitsBox.Text = "";
            RisksBox.Text = "";
        }
        private void SuppGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuppGrid.SelectedItem is Supplement selected)
            {
                EditNameBox.Text = selected.Name;
                EditDosageBox.Text = selected.Dosage;
                EditInstructionsBox.Text = selected.Instructions;
                EditBenefitsBox.Text = selected.Benefits;
                EditRisksBox.Text = selected.Risks;
            }
        }
        private void UpdateSupplement_Click(object sender, RoutedEventArgs e)
        {
            if (SuppGrid.SelectedItem is not Supplement selected)
            {
                MessageBox.Show("Please select a supplement to update.");
                return;
            }

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"UPDATE Supplement SET 
                Name = @Name, Dosage = @Dosage, Instructions = @Instructions, 
                Benefits = @Benefits, Risks = @Risks 
                WHERE SupplementId = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", EditNameBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Dosage", EditDosageBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Instructions", EditInstructionsBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Benefits", EditBenefitsBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Risks", EditRisksBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Id", selected.SupplementId);

            cmd.ExecuteNonQuery();
            MessageBox.Show("✅ Supplement updated.");
            LoadSupplements();
        }
        private void DeleteSupplement_Click(object sender, RoutedEventArgs e)
        {
            if (SuppGrid.SelectedItem is not Supplement selected)
            {
                MessageBox.Show("Please select a supplement to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Delete supplement \"{selected.Name}\"?", "Confirm Delete", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "DELETE FROM Supplement WHERE SupplementId = @Id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", selected.SupplementId);
            cmd.ExecuteNonQuery();

            MessageBox.Show("🗑️ Supplement deleted.");
            LoadSupplements();
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

