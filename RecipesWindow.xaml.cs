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
    public partial class RecipesWindow : Window
    {
        public RecipesWindow()
        {
            InitializeComponent();
            LoadRecipes();
        }

        public class Recipe
        {
            public string Title { get; set; }
            public string Ingredients { get; set; }
            public string Instructions { get; set; }
            public float Calories { get; set; }
            public float Protein { get; set; }
            public float Carbs { get; set; }
            public float Fat { get; set; }
        }

        private void LoadRecipes()
        {
            List<Recipe> recipes = new();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Title, Ingredients, Instructions, Calories, Protein, Carbs, Fat FROM Recipe";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    Title = reader["Title"].ToString(),
                    Ingredients = reader["Ingredients"].ToString(),
                    Instructions = reader["Instructions"].ToString(),
                    Calories = float.Parse(reader["Calories"].ToString()),
                    Protein = float.Parse(reader["Protein"].ToString()),
                    Carbs = float.Parse(reader["Carbs"].ToString()),
                    Fat = float.Parse(reader["Fat"].ToString())
                });
            }

            RecipeList.ItemsSource = recipes;
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

        private void GoToRecipes_Click(object sender, RoutedEventArgs e)
        {
            new RecipesWindow().Show();
            this.Close();
        }

        private void GoToSupplements_Click(object sender, RoutedEventArgs e)
        {
            new SupplementWindow().Show();
            this.Close();
        }

    }
}
