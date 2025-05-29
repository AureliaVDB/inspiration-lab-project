using System;
using System.Collections.Generic;
using KeepTrackApp.Models;
using KeepTrackApp.Utils;
using MySql.Data.MySqlClient;


namespace KeepTrackApp
{
    class Program
    {
        public static List<User> registeredUsers = new();
        public static List<Admin> admins = new()
        {
            new Admin { AdminId = "Raja", Password = "Admin" },
            new Admin { AdminId = "Aurelia", Password = "Admin" }
        };
        public static List<Supplement> supplements = new()
        {
            new Supplement
            {
                SupplementId = 1,
                AdminId = "Raja",
                Name = "Creatine",
                Dosage = "5g per day after workout",
                Instructions = "Mix with water or shake post-exercise.",
                Benefits = "Improves strength, performance, and muscle growth.",
                Risks = "Can cause bloating, dehydration if not enough water is consumed."
            },
            new Supplement
            {
                SupplementId = 2,
                AdminId = "Aurelia",
                Name = "Fish Oil",
                Dosage = "1000mg per day with a meal",
                Instructions = "Take with breakfast or lunch.",
                Benefits = "Supports heart, brain, and joint health.",
                Risks = "Might cause fishy aftertaste or mild stomach upset."
            }
        };
        public static List<Recipe> recipes = new()
        {
            new Recipe
            {
                RecipeId = 1,
                Title = "Grilled Chicken",
                Category = RecipeCategory.Meat,
                Ingredients = "Chicken breast, olive oil, spices",
                Instructions = "Grill chicken for 6–8 mins per side.",
                Image = "[image placeholder]",
                Calories = 300,
                Protein = 35,
                Carbs = 2,
                Fat = 15,
                AdminId = "Raja"
            },
            new Recipe
            {
                RecipeId = 2,
                Title = "Salmon Bowl",
                Category = RecipeCategory.Fish,
                Ingredients = "Salmon, rice, avocado",
                Instructions = "Bake salmon, serve on rice with toppings.",
                Image = "[image placeholder]",
                Calories = 500,
                Protein = 40,
                Carbs = 30,
                Fat = 20,
                AdminId = "Aurelia"
            }
            // Add more as needed
        };

        public static Dictionary<string, Progress> userProgress = new();
        public static Dictionary<string, List<MealLog>> userMeals = new();


        static void Main(string[] args)
        {
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
           
            
 
            Console.WriteLine(" KEEP TRACK \n");

            Console.Write(" Username : ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("\n Register User");
                RegisterUser();
                return;
            }

            Console.Write(" Password : ");
            string password = Console.ReadLine();

            // Check Admin Login
            var admin = admins.Find(a => a.AdminId == username && a.Password == password);
            if (admin != null)
            {
                Console.WriteLine($"✅ Admin login successful. Welcome, {admin.AdminId}!");
                AdminDashboard.RegisteredUsers = registeredUsers;
                AdminDashboard.LoadAdminDashboard();
                return;
            }


            // Check User Login
            User user = null;
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM User WHERE Username = @Username AND Password = @Password LIMIT 1";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        UserId = reader.GetString("UserId"),
                        Username = reader.GetString("Username"),
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Age = reader.GetInt32("Age"),
                        Gender = reader.GetString("Gender") == "Male" ? Gender.Male : Gender.Female
                    };
                }
            }

            if (user != null)
            {
                Console.WriteLine($" User login {user.Username}!");
                ShowUserDashboard(user);
                return;
            }


            Console.WriteLine(" try again");
        }

        static void RegisterUser()
        {
            Console.WriteLine("\n Keep track ");

            string userId = UserHelper.GenerateUniqueUsername();
            Console.WriteLine($"Your  user ID: {userId}");

            Console.Write("Enter display name: ");
            string displayName = Console.ReadLine();

            Console.Write("Enter date of birth (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dob))
            {
                Console.WriteLine(" Invalid date format.");
                return;
            }

            int age = User.CalculateAgeFromDob(dob);
            Console.WriteLine($"You are {age} years old.");

            Console.Write("Enter gender (M/F): ");
            string genderInput = Console.ReadLine().ToUpper();
            Gender gender;
            if (genderInput == "M")
                gender = Gender.Male;
            else if (genderInput == "F")
                gender = Gender.Female;
            else
            {
                Console.WriteLine(" Invalid gender");
                return;
            }

            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            if (!UserHelper.ValidateEmail(email))
            {
                Console.WriteLine(" Email not found or invalid.");
                return;
            }

            string code = UserHelper.GenerateVerificationCode();
            UserHelper.SendVerificationEmail(email, code, userId);

            Console.Write("Enter the verification code: ");
            if (Console.ReadLine() != code)
            {
                Console.WriteLine(" Incorrect code.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            if (!UserHelper.ValidatePassword(password))
            {
                Console.WriteLine(" Password must be 8+ chars with uppercase, lowercase, number, symbol.");
                return;
            }

            Console.Write("Confirm password: ");
            string confirm = Console.ReadLine();
            if (password != confirm)
            {
                Console.WriteLine(" Passwords do not match.");
                return;
            }

            var user = new User
            {
                UserId = userId,
                Username = displayName,
                Email = email,
                Password = password,
                CreatedAt = DateTime.Now,
                DateOfBirth = dob,
                Age = age,
                Gender = gender
            };

            registeredUsers.Add(user);
            // add to database
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertQuery = @"INSERT INTO User (UserId, Username, Email, Password, CreatedAt, DateOfBirth, Age, Gender)
                       VALUES (@UserId, @Username, @Email, @Password, @CreatedAt, @DateOfBirth, @Age, @Gender);";

            using var cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
            cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
            cmd.Parameters.AddWithValue("@Age", user.Age);
            cmd.Parameters.AddWithValue("@Gender", user.Gender.ToString());

            cmd.ExecuteNonQuery();
            Console.WriteLine($"\n Registration successful! Welcome, {user.Username}!");
        }

        static void ShowUserDashboard(User user)
        {
            //take data from database 
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM User WHERE UserId = @UserId LIMIT 1";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user.Username = reader.GetString("Username");
                    user.Email = reader.GetString("Email");
                    user.Age = reader.GetInt32("Age");
                    user.DateOfBirth = reader.GetDateTime("DateOfBirth");
                    user.CreatedAt = reader.GetDateTime("CreatedAt");
                    user.Gender = reader.GetString("Gender") == "Male" ? Gender.Male : Gender.Female;
                }
            }

            Console.Clear();
            Console.WriteLine("========== USER DASHBOARD ==========\n");

            Console.WriteLine($"User ID   : {user.UserId}");
            Console.WriteLine($"Name      : {user.Username}");
            Console.WriteLine($"Age       : {user.Age}");
            Console.WriteLine($"Sex       : {user.Gender}");
            Console.WriteLine($"Email     : {user.Email}");
            Console.WriteLine("Profile Picture : [placeholder - will show on website]");

            Console.WriteLine("\n------------- OPTIONS -------------");
            Console.Write("Change name (press Enter to skip): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                user.Username = newName;
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                string updateQuery = "UPDATE User SET Username = @Username WHERE UserId = @UserId";
                using var updateCmd = new MySqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Username", user.Username);
                updateCmd.Parameters.AddWithValue("@UserId", user.UserId);
                updateCmd.ExecuteNonQuery();
                Console.WriteLine($" Name updated to {user.Username}");
            }

            Console.WriteLine("\nActions:");
            Console.WriteLine("1. Change Password");
            Console.WriteLine("2. Delete Account");
            Console.WriteLine("3. Go to Recipes");
            Console.WriteLine("4. Go to Supplements");
            Console.WriteLine("5. Go to Progress");
            Console.WriteLine("0. Logout");

            Console.Write("\nSelect an option: ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ChangePassword(user); // You'll define this
                    break;
                case "2":
                    DeleteAccount(user); // You'll define this
                    return;
                case "3":
                    ShowRecipes(user); // Placeholder for now
                    break;
                case "4":
                    ShowSupplements(user); // Placeholder
                    break;
                case "5":
                    ShowProgress(user); // Placeholder
                    break;
                case "0":
                    Console.WriteLine("👋 Logging out...");
                    return;
                default:
                    Console.WriteLine("❌ Invalid option.");
                    break;
            }

            Console.WriteLine("\nPress Enter to refresh dashboard...");
            Console.ReadLine();
            ShowUserDashboard(user); // reload dashboard
        }

        static void ChangePassword(User user)
        {
            Console.Clear();
            Console.WriteLine("======= CHANGE PASSWORD =======\n");

            Console.Write("Enter your email: ");
            string email = Console.ReadLine();

            if (email != user.Email)
            {
                Console.WriteLine("❌ Email does not match your account.");
                return;
            }

            string code = UserHelper.GenerateVerificationCode();
            UserHelper.SendVerificationEmail(email, code, user.UserId);

            Console.Write("Enter the verification code sent to your email: ");
            string inputCode = Console.ReadLine();
            if (inputCode != code)
            {
                Console.WriteLine("❌ Incorrect verification code.");
                return;
            }

            Console.Write("Enter new password: ");
            string newPassword = Console.ReadLine();
            if (!UserHelper.ValidatePassword(newPassword))
            {
                Console.WriteLine("❌ Password must include:");
                Console.WriteLine("- At least 8 characters");
                Console.WriteLine("- Uppercase and lowercase letters");
                Console.WriteLine("- At least 1 digit");
                Console.WriteLine("- At least 1 special character");
                return;
            }

            Console.Write("Confirm new password: ");
            string confirmPassword = Console.ReadLine();

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("❌ Passwords do not match.");
                return;
            }

            user.Password = newPassword;
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string updateQuery = @"UPDATE User 
                       SET Password = @Password 
                       WHERE UserId = @UserId";

            using var cmd = new MySqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@Password", newPassword);
            cmd.Parameters.AddWithValue("@UserId", user.UserId);

            cmd.ExecuteNonQuery();

            Console.WriteLine("✅ Password updated successfully.");
        }

        static void DeleteAccount(User user)
        {
            Console.Clear();
            Console.WriteLine("======== DELETE ACCOUNT ========\n");

            Console.Write("Are you sure you want to delete your account? (Yes/No): ");
            string confirm = Console.ReadLine().Trim().ToLower();

            if (confirm != "yes")
            {
                Console.WriteLine("❎ Account deletion cancelled.");
                return;
            }

            Console.WriteLine("\nReason for deleting (optional):");
            Console.Write("> ");
            string reason = Console.ReadLine();

            Console.WriteLine("\n🛑 NEVER GIVE UP! 🛑");
            Console.WriteLine("\nType 'DELETE' to confirm final deletion: ");
            string final = Console.ReadLine().Trim();

            if (final != "DELETE")
            {
                Console.WriteLine("❌ Final confirmation failed. Account not deleted.");
                return;
            }

            registeredUsers.Remove(user);
            // Delete from MySQL
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // Delete meals
            var delMeals = new MySqlCommand("DELETE FROM MealLog WHERE UserId = @UserId", conn);
            delMeals.Parameters.AddWithValue("@UserId", user.UserId);
            delMeals.ExecuteNonQuery();

            // Delete progress
            var delProgress = new MySqlCommand("DELETE FROM Progress WHERE UserId = @UserId", conn);
            delProgress.Parameters.AddWithValue("@UserId", user.UserId);
            delProgress.ExecuteNonQuery();

            // Delete user
            var delUser = new MySqlCommand("DELETE FROM User WHERE UserId = @UserId", conn);
            delUser.Parameters.AddWithValue("@UserId", user.UserId);
            delUser.ExecuteNonQuery();

            
            Console.WriteLine("\n✅ Your account has been permanently deleted.");
            Console.WriteLine("You have been logged out.");
        }
        static void ShowSupplements(User user)
        {

            //foreach (var supplement in supplements)
            //{
            //    Console.WriteLine($"▶ {supplement.Name.ToUpper()}");
            //    Console.WriteLine($"Dosage       : {supplement.Dosage}");
            //    Console.WriteLine($"Instructions : {supplement.Instructions}");
            //    Console.WriteLine($"Benefits     : {supplement.Benefits}");
            //    Console.WriteLine($"Risks        : {supplement.Risks}");
            //    Console.WriteLine(new string('-', 40));
            //}

            Console.Clear();
            Console.WriteLine("===== SUPPLEMENTS TAB =====\n");
            Console.WriteLine("⚠️ CAUTION NOTICE: Always consult a healthcare provider before taking any supplement.\n");

            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM Supplement";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"▶ {reader["Name"].ToString().ToUpper()}");
                Console.WriteLine($"Dosage       : {reader["Dosage"]}");
                Console.WriteLine($"Instructions : {reader["Instructions"]}");
                Console.WriteLine($"Benefits     : {reader["Benefits"]}");
                Console.WriteLine($"Risks        : {reader["Risks"]}");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\nPress Enter to return to dashboard...");
            Console.ReadLine();

        }
        static void ShowRecipes(User user)
        {
          
            //foreach (RecipeCategory category in Enum.GetValues(typeof(RecipeCategory)))
            //{
            //    Console.WriteLine($"📂 {category.ToString().ToUpper()}");
            //    foreach (var recipe in recipes.FindAll(r => r.Category == category))
            //    {
            //        Console.WriteLine($"\n🍽 {recipe.Title.ToUpper()} ({recipe.Protein}g / {recipe.Carbs}g / {recipe.Fat}g / {recipe.Calories} kcal)");
            //        Console.WriteLine($"Ingredients : {recipe.Ingredients}");
            //        Console.WriteLine($"Instructions: {recipe.Instructions}");
            //        Console.WriteLine($"Image       : {recipe.Image}");
            //        Console.WriteLine(new string('-', 40));
            //    }
            //    Console.WriteLine(); // spacing between categories
            //}
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            Console.Clear();
            Console.WriteLine("======= RECIPES (FROM DATABASE) =======\n");

            // Display category navigation
            Console.WriteLine("Available Categories:");
            foreach (var cat in Enum.GetValues(typeof(RecipeCategory)))
                Console.WriteLine($"- {cat}");

            Console.WriteLine("\nAll Recipes:\n");

            foreach (RecipeCategory category in Enum.GetValues(typeof(RecipeCategory)))
            {
                Console.WriteLine($"\n📂 {category.ToString().ToUpper()}");

                string query = "SELECT * FROM Recipe WHERE Category = @Category";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Category", category.ToString());

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"\n🍽 {reader["Title"].ToString().ToUpper()} ({reader["Protein"]}P / {reader["Carbs"]}C / {reader["Fat"]}F / {reader["Calories"]} kcal)");
                    Console.WriteLine($"Ingredients : {reader["Ingredients"]}");
                    Console.WriteLine($"Instructions: {reader["Instructions"]}");
                    Console.WriteLine($"Image       : {reader["Image"]}");
                    Console.WriteLine(new string('-', 40));
                }
                reader.Close(); // Important to close before next query
            }

            Console.WriteLine("\n(Press Enter to return to dashboard)");
            Console.ReadLine();
        }
        static void ShowProgress(User user)
        {
            Console.Clear();
            Console.WriteLine("======= PROGRESS TAB =======\n");

            // STEP 1: Enter weight
            Console.Write("Enter your current weight (kg): ");
            float.TryParse(Console.ReadLine(), out float weightKg);

            // Save weight entry
            if (!userProgress.ContainsKey(user.UserId))
                userProgress[user.UserId] = new Progress { UserId = user.UserId };
            userProgress[user.UserId].AddWeight(DateTime.Today, weightKg);
            // add to database
            using var weightConn = new MySqlConnection(connectionString);
            weightConn.Open();

            string insertWeight = @"INSERT INTO Progress (UserId, Date, Weight)
                        VALUES (@UserId, @Date, @Weight);";

            using var weightCmd = new MySqlCommand(insertWeight, weightConn);
            weightCmd.Parameters.AddWithValue("@UserId", user.UserId);
            weightCmd.Parameters.AddWithValue("@Date", DateTime.Today);
            weightCmd.Parameters.AddWithValue("@Weight", weightKg);

            weightCmd.ExecuteNonQuery();


            // STEP 2: Generate Target Macros
            float proteinTarget = weightKg * 2f;
            float fatTarget = weightKg * 1f;
            float carbTarget = weightKg * 3f;
            float calorieTarget = (proteinTarget * 4) + (fatTarget * 9) + (carbTarget * 4);

            Console.WriteLine($"\n🎯 Target Macros (based on {weightKg} kg):");
            Console.WriteLine($"Protein : {proteinTarget}g");
            Console.WriteLine($"Fats    : {fatTarget}g");
            Console.WriteLine($"Carbs   : {carbTarget}g");
            Console.WriteLine($"Calories: {calorieTarget} kcal");

            // STEP 3: Add meals for today
            Console.WriteLine("\n📅 Adding meals for today...");
            if (!userMeals.ContainsKey(user.UserId))
                userMeals[user.UserId] = new List<MealLog>();

            while (true)
            {
                Console.Write("\nAdd a new meal? (yes/no): ");
                string addMeal = Console.ReadLine().ToLower();
                if (addMeal != "yes") break;

                var meal = new MealLog
                {
                    MealId = userMeals[user.UserId].Count + 1,
                    UserId = user.UserId,
                    Date = DateTime.Today
                };

                Console.Write("Meal description: ");
                meal.Description = Console.ReadLine();

                Console.Write("Protein (g): ");
                float.TryParse(Console.ReadLine(), out float protein);
                Console.Write("Carbs (g): ");
                float.TryParse(Console.ReadLine(), out float carbs);
                Console.Write("Fat (g): ");
                float.TryParse(Console.ReadLine(), out float fat);

                meal.Protein = protein;
                meal.Carbs = carbs;
                meal.Fat = fat;
                meal.Calories = (protein * 4) + (carbs * 4) + (fat * 9);

                userMeals[user.UserId].Add(meal);
                Console.WriteLine("✅ Meal added!");
                //add to database
                string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";

                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                string insertMeal = @"INSERT INTO MealLog (UserId, Date, Category, Description, Calories, Protein, Carbs, Fat)
                      VALUES (@UserId, @Date, @Category, @Description, @Calories, @Protein, @Carbs, @Fat);";

                using var cmd = new MySqlCommand(insertMeal, conn);
                cmd.Parameters.AddWithValue("@UserId", meal.UserId);
                cmd.Parameters.AddWithValue("@Date", meal.Date);
                cmd.Parameters.AddWithValue("@Category", meal.Category.ToString());
                cmd.Parameters.AddWithValue("@Description", meal.Description);
                cmd.Parameters.AddWithValue("@Calories", meal.Calories);
                cmd.Parameters.AddWithValue("@Protein", meal.Protein);
                cmd.Parameters.AddWithValue("@Carbs", meal.Carbs);
                cmd.Parameters.AddWithValue("@Fat", meal.Fat);

                cmd.ExecuteNonQuery();

            }

            // STEP 4: Show daily totals and compare to targets
            float totalProtein = 0, totalCarbs = 0, totalFat = 0, totalKcal = 0;
            foreach (var meal in userMeals[user.UserId].FindAll(m => m.Date.Date == DateTime.Today))
            {
                totalProtein += meal.Protein;
                totalCarbs += meal.Carbs;
                totalFat += meal.Fat;
                totalKcal += meal.Calories;
            }

            Console.WriteLine("\n📊 Today's Total Intake:");
            Console.WriteLine($"Protein : {totalProtein}g / {proteinTarget}g {(totalProtein >= proteinTarget ? "✅" : "❌")}");
            Console.WriteLine($"Carbs   : {totalCarbs}g / {carbTarget}g {(totalCarbs >= carbTarget ? "✅" : "❌")}");
            Console.WriteLine($"Fat     : {totalFat}g / {fatTarget}g {(totalFat >= fatTarget ? "✅" : "❌")}");
            Console.WriteLine($"Calories: {totalKcal} / {calorieTarget} kcal {(totalKcal >= calorieTarget ? "✅" : "❌")}");

            // STEP 5: Weekly overview
            Console.Clear();
            Console.WriteLine("======= WEEKLY OVERVIEW =======");

            DateTime startOfWeek = GetStartOfWeek(DateTime.Today);
            DateTime endOfWeek = GetEndOfWeek(startOfWeek);
            Console.WriteLine($"📅 Week: {startOfWeek:dd/MM/yyyy} - {endOfWeek:dd/MM/yyyy}");

            float weeklyP = 0, weeklyF = 0, weeklyC = 0, weeklyKcal = 0;
            int daysWithMeals = 0;

            //for (int i = 0; i < 7; i++)
            //{
            //    DateTime date = startOfWeek.AddDays(i);
            //    var mealsForDay = userMeals[user.UserId].FindAll(m => m.Date.Date == date);

            //    if (mealsForDay.Count == 0) continue;

            //    Console.WriteLine($"\n📌 Day {i + 1} - {date:dddd}");
            //    float dayP = 0, dayF = 0, dayC = 0, dayKcal = 0;

            //    foreach (var m in mealsForDay)
            //    {
            //        Console.WriteLine($" - {m.Description} | {m.Protein}P {m.Fat}F {m.Carbs}C = {m.Calories} kcal");
            //        dayP += m.Protein;
            //        dayF += m.Fat;
            //        dayC += m.Carbs;
            //        dayKcal += m.Calories;
            //    }

            //    Console.WriteLine($"   ➕ Total: {dayP}P / {dayF}F / {dayC}C / {dayKcal} kcal");

            //    weeklyP += dayP;
            //    weeklyF += dayF;
            //    weeklyC += dayC;
            //    weeklyKcal += dayKcal;
            //    daysWithMeals++;
            //}
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = startOfWeek.AddDays(i);

                string query = @"SELECT * FROM MealLog 
                     WHERE UserId = @UserId AND Date = @Date";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Date", date.Date);

                using var reader = cmd.ExecuteReader();

                List<MealLog> mealsForDay = new();
                while (reader.Read())
                {
                    mealsForDay.Add(new MealLog
                    {
                        Description = reader["Description"].ToString(),
                        Protein = Convert.ToSingle(reader["Protein"]),
                        Carbs = Convert.ToSingle(reader["Carbs"]),
                        Fat = Convert.ToSingle(reader["Fat"]),
                        Calories = Convert.ToSingle(reader["Calories"])
                    });
                }

                reader.Close();

                if (mealsForDay.Count == 0) continue;

                Console.WriteLine($"\n📌 Day {i + 1} - {date:dddd}");

                float dayP = 0, dayF = 0, dayC = 0, dayKcal = 0;

                foreach (var m in mealsForDay)
                {
                    Console.WriteLine($" - {m.Description} | {m.Protein}P {m.Fat}F {m.Carbs}C = {m.Calories} kcal");
                    dayP += m.Protein;
                    dayF += m.Fat;
                    dayC += m.Carbs;
                    dayKcal += m.Calories;
                }

                Console.WriteLine($"   ➕ Total: {dayP}P / {dayF}F / {dayC}C / {dayKcal} kcal");

                weeklyP += dayP;
                weeklyF += dayF;
                weeklyC += dayC;
                weeklyKcal += dayKcal;
                daysWithMeals++;
            }


            // Show recap
            if (daysWithMeals > 0)
            {
                Console.WriteLine("\n======= WEEK RECAP =======");
                Console.WriteLine($"Avg Protein : {weeklyP / daysWithMeals}g");
                Console.WriteLine($"Avg Fat     : {weeklyF / daysWithMeals}g");
                Console.WriteLine($"Avg Carbs   : {weeklyC / daysWithMeals}g");
                Console.WriteLine($"Avg Calories: {weeklyKcal / daysWithMeals} kcal");

                Console.WriteLine("\n📈 Macro trend graph: [Coming Soon]");
            }
            else
            {
                Console.WriteLine("\n(No meal data for this week.)");
            }

            Console.WriteLine("\nPress Enter to return to dashboard...");
            Console.ReadLine();
        }
        static DateTime GetStartOfWeek(DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-1 * diff).Date;
        }

        static DateTime GetEndOfWeek(DateTime startOfWeek)
        {
            return startOfWeek.AddDays(6).Date;
        }


    }
}

