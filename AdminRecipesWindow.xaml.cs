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
    public partial class AdminRecipesWindow : Window
    {
        public class Recipe
        {
            public int RecipeId { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string Ingredients { get; set; }
            public string Instructions { get; set; }
            public string Image { get; set; }
            public float Calories { get; set; }
            public float Protein { get; set; }
            public float Carbs { get; set; }
            public float Fat { get; set; }
            public string AdminId { get; set; }
        }

        private string currentAdminId;
        private string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";


        public AdminRecipesWindow(string adminId)
        {
            InitializeComponent();
            LoadRecipes();
            currentAdminId = adminId;
        }

        private void LoadRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM Recipe";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = Convert.ToInt32(reader["RecipeId"]),
                    Title = reader["Title"].ToString(),
                    Category = reader["Category"].ToString(),
                    Ingredients = reader["Ingredients"].ToString(),
                    Instructions = reader["Instructions"].ToString(),
                    Image = reader["Image"].ToString(),
                    Calories = float.Parse(reader["Calories"].ToString()),
                    Protein = float.Parse(reader["Protein"].ToString()),
                    Carbs = float.Parse(reader["Carbs"].ToString()),
                    Fat = float.Parse(reader["Fat"].ToString()),
                    AdminId = reader["AdminId"].ToString()
                });
            }

            RecipeGrid.ItemsSource = null;
            RecipeGrid.ItemsSource = recipes;
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            string title = AddTitleBox.Text.Trim();
            string category = AddCategoryBox.Text.Trim();
            string ingredients = AddIngredientsBox.Text.Trim();
            string instructions = AddInstructionsBox.Text.Trim();
            string image = AddImageBox.Text.Trim();
            float calories = float.Parse(AddCaloriesBox.Text);
            float protein = float.Parse(AddProteinBox.Text);
            float carbs = float.Parse(AddCarbsBox.Text);
            float fat = float.Parse(AddFatBox.Text);

            string query = @"INSERT INTO Recipe 
                (Title, Category, Ingredients, Instructions, Image, Calories, Protein, Carbs, Fat, AdminId) 
                VALUES (@Title, @Category, @Ingredients, @Instructions, @Image, @Calories, @Protein, @Carbs, @Fat, @AdminId)";

            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Category", category);
            cmd.Parameters.AddWithValue("@Ingredients", ingredients);
            cmd.Parameters.AddWithValue("@Instructions", instructions);
            cmd.Parameters.AddWithValue("@Image", image);
            cmd.Parameters.AddWithValue("@Calories", calories);
            cmd.Parameters.AddWithValue("@Protein", protein);
            cmd.Parameters.AddWithValue("@Carbs", carbs);
            cmd.Parameters.AddWithValue("@Fat", fat);
            cmd.Parameters.AddWithValue("@AdminId", currentAdminId);

            cmd.ExecuteNonQuery();
            MessageBox.Show("✅ Recipe added.");
            LoadRecipes();
        }
        private void RecipeGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeGrid.SelectedItem is Recipe selected)
            {
                TitleBox.Text = selected.Title;
                CategoryBox.Text = selected.Category;
                IngredientsBox.Text = selected.Ingredients;
                InstructionsBox.Text = selected.Instructions;
                CaloriesBox.Text = selected.Calories.ToString();
                ProteinBox.Text = selected.Protein.ToString();
                CarbsBox.Text = selected.Carbs.ToString();
                FatBox.Text = selected.Fat.ToString();
            }
        }

        private void UpdateRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeGrid.SelectedItem is not Recipe selected) return;

            string query = @"UPDATE Recipe SET Title = @Title, Category = @Category, 
        Ingredients = @Ingredients, Instructions = @Instructions, 
        Calories = @Calories, Protein = @Protein, Carbs = @Carbs, Fat = @Fat 
        WHERE RecipeId = @Id";

            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", TitleBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Category", CategoryBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Ingredients", IngredientsBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Instructions", InstructionsBox.Text.Trim());
            cmd.Parameters.AddWithValue("@Calories", float.Parse(CaloriesBox.Text));
            cmd.Parameters.AddWithValue("@Protein", float.Parse(ProteinBox.Text));
            cmd.Parameters.AddWithValue("@Carbs", float.Parse(CarbsBox.Text));
            cmd.Parameters.AddWithValue("@Fat", float.Parse(FatBox.Text));
            cmd.Parameters.AddWithValue("@Id", selected.RecipeId);

            cmd.ExecuteNonQuery();
            MessageBox.Show("✅ Recipe updated.");
            LoadRecipes();
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeGrid.SelectedItem is not Recipe selected) return;

            var confirm = MessageBox.Show($"Delete recipe '{selected.Title}'?", "Confirm", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM Recipe WHERE RecipeId = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", selected.RecipeId);
            cmd.ExecuteNonQuery();

            MessageBox.Show("🗑 Recipe deleted.");
            LoadRecipes();
        }
        


        private void ClearForm()
        {
            TitleBox.Text = "";
            CategoryBox.Text = "";
            IngredientsBox.Text = "";
            InstructionsBox.Text = "";
            
            CaloriesBox.Text = "";
            ProteinBox.Text = "";
            CarbsBox.Text = "";
            FatBox.Text = "";
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

