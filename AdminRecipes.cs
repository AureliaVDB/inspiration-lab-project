using System;
using System.Collections.Generic;
using KeepTrackApp.Models;
using MySql.Data.MySqlClient;


namespace KeepTrackApp
{
    public static class AdminRecipes
    {
        public static List<Recipe> Recipes { get; set; }

        public static void ManageRecipes()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ADMIN - RECIPE MANAGEMENT =====\n");

                Console.WriteLine("1. View All Recipes");
                Console.WriteLine("2. Add Recipe");
                Console.WriteLine("3. Edit Recipe");
                Console.WriteLine("4. Delete Recipe");
                Console.WriteLine("0. Return to Admin Dashboard");

                Console.Write("\nChoose an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewAll();
                        break;
                    case "2":
                        AddRecipe();
                        break;
                    case "3":
                        EditRecipe();
                        break;
                    case "4":
                        DeleteRecipe();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void ViewAll()
        {
            Console.Clear();
            Console.WriteLine("==== RECIPE LIST ====\n");

            foreach (RecipeCategory category in Enum.GetValues(typeof(RecipeCategory)))
            {
                Console.WriteLine($"\n📂 {category.ToString().ToUpper()}");

                foreach (var recipe in Recipes.FindAll(r => r.Category == category))
                {
                    Console.WriteLine($"\n🍽 {recipe.Title.ToUpper()} ({recipe.Protein}P / {recipe.Carbs}C / {recipe.Fat}F / {recipe.Calories} kcal)");
                    Console.WriteLine($"Ingredients : {recipe.Ingredients}");
                    Console.WriteLine($"Instructions: {recipe.Instructions}");
                    Console.WriteLine($"Image       : {recipe.Image}");
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void AddRecipe()
        {
            Console.Clear();
            Console.WriteLine("==== ADD NEW RECIPE ====\n");

            var recipe = new Recipe
            {
                RecipeId = Recipes.Count + 1,
                AdminId = "Admin"
            };

            Console.Write("Title: ");
            recipe.Title = Console.ReadLine();

            Console.WriteLine("Category (0: Breakfast, 1: Lunch, 2: Dinner, 3: Snack): ");
            if (Enum.TryParse(Console.ReadLine(), out RecipeCategory cat))
                recipe.Category = cat;

            Console.Write("Ingredients: ");
            recipe.Ingredients = Console.ReadLine();

            Console.Write("Instructions: ");
            recipe.Instructions = Console.ReadLine();

            Console.Write("Image (placeholder or URL): ");
            recipe.Image = Console.ReadLine();

            Console.Write("Calories: ");
            float.TryParse(Console.ReadLine(), out float kcal);
            recipe.Calories = kcal;

            Console.Write("Protein: ");
            float.TryParse(Console.ReadLine(), out float p);
            recipe.Protein = p;

            Console.Write("Carbs: ");
            float.TryParse(Console.ReadLine(), out float c);
            recipe.Carbs = c;

            Console.Write("Fat: ");
            float.TryParse(Console.ReadLine(), out float f);
            recipe.Fat = f;

            Recipes.Add(recipe);
            //add to databse
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertRecipe = @"INSERT INTO Recipe 
                (Title, Category, Ingredients, Instructions, Image, Calories, Protein, Carbs, Fat, AdminId)
                VALUES
                (@Title, @Category, @Ingredients, @Instructions, @Image, @Calories, @Protein, @Carbs, @Fat, @AdminId);";

            using var cmd = new MySqlCommand(insertRecipe, conn);
            cmd.Parameters.AddWithValue("@Title", recipe.Title);
            cmd.Parameters.AddWithValue("@Category", recipe.Category.ToString());
            cmd.Parameters.AddWithValue("@Ingredients", recipe.Ingredients);
            cmd.Parameters.AddWithValue("@Instructions", recipe.Instructions);
            cmd.Parameters.AddWithValue("@Image", recipe.Image);
            cmd.Parameters.AddWithValue("@Calories", recipe.Calories);
            cmd.Parameters.AddWithValue("@Protein", recipe.Protein);
            cmd.Parameters.AddWithValue("@Carbs", recipe.Carbs);
            cmd.Parameters.AddWithValue("@Fat", recipe.Fat);
            cmd.Parameters.AddWithValue("@AdminId", recipe.AdminId);

            cmd.ExecuteNonQuery();
            Console.WriteLine("\n✅ Recipe added. Press Enter to return...");
            Console.ReadLine();
        }

        static void EditRecipe()
        {
            Console.Clear();
            Console.WriteLine("==== EDIT RECIPE ====\n");

            Console.Write("Enter Recipe ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadLine();
                return;
            }

            var recipe = Recipes.Find(r => r.RecipeId == id);
            if (recipe == null)
            {
                Console.WriteLine("Recipe not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Leave blank to keep existing value.\n");

            Console.Write($"Title ({recipe.Title}): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) recipe.Title = input;

            Console.WriteLine($"Category ({recipe.Category}) - Enter 0-3 to change or leave blank: ");
            input = Console.ReadLine();
            if (Enum.TryParse(input, out RecipeCategory newCat)) recipe.Category = newCat;

            Console.Write($"Ingredients ({recipe.Ingredients}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) recipe.Ingredients = input;

            Console.Write($"Instructions ({recipe.Instructions}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) recipe.Instructions = input;

            Console.Write($"Image ({recipe.Image}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) recipe.Image = input;

            Console.Write($"Calories ({recipe.Calories}): ");
            input = Console.ReadLine();
            if (float.TryParse(input, out float kcal)) recipe.Calories = kcal;

            Console.Write($"Protein ({recipe.Protein}): ");
            input = Console.ReadLine();
            if (float.TryParse(input, out float p)) recipe.Protein = p;

            Console.Write($"Carbs ({recipe.Carbs}): ");
            input = Console.ReadLine();
            if (float.TryParse(input, out float c)) recipe.Carbs = c;

            Console.Write($"Fat ({recipe.Fat}): ");
            input = Console.ReadLine();
            if (float.TryParse(input, out float f)) recipe.Fat = f;

            Console.WriteLine("\n✅ Recipe updated. Press Enter to return...");
            Console.ReadLine();
        }

        static void DeleteRecipe()
        {
            Console.Clear();
            Console.WriteLine("==== DELETE RECIPE ====\n");

            Console.Write("Enter Recipe ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadLine();
                return;
            }

            var recipe = Recipes.Find(r => r.RecipeId == id);
            if (recipe == null)
            {
                Console.WriteLine("Recipe not found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Type DELETE to confirm: ");
            if (Console.ReadLine().ToUpper() == "DELETE")
            {
                Recipes.Remove(recipe);
                Console.WriteLine("✅ Recipe deleted.");
            }
            else
            {
                Console.WriteLine("❎ Cancelled.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }
    }
}
