using System;
using System.Collections.Generic;
using KeepTrackApp.Models;

namespace KeepTrackApp
{
    public static class AdminDashboard
    {
        // Reference to shared user data
        public static List<User> RegisteredUsers { get; set; }

        public static void LoadAdminDashboard()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("======== ADMIN DASHBOARD ========");
                Console.WriteLine("1. View All Users");
                Console.WriteLine("2. Search User");
                Console.WriteLine("3. Create or Edit User");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Manage Supplements");
                Console.WriteLine("6. Manage Recipes");
                Console.WriteLine("7. View User Progress");
                Console.WriteLine("0. Logout");

                Console.Write("\nSelect an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewAllUsers();
                        break;
                    case "2":
                        SearchUser();
                        break;
                    case "3":
                        CreateOrEditUser();
                        break;
                    case "4":
                        DeleteUser();
                        break;
                    case "5":
                        AdminSupplements.Supplements = Program.supplements;
                        AdminSupplements.ManageSupplements();
                        break;
                    case "6":
                        AdminRecipes.Recipes = Program.recipes;
                        AdminRecipes.ManageRecipes();
                        break;
                    case "7":
                        AdminProgress.RegisteredUsers = Program.registeredUsers;
                        AdminProgress.UserMeals = Program.userMeals;
                        AdminProgress.UserProgress = Program.userProgress;
                        AdminProgress.ViewUserProgress();
                        break;
                    case "0":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void ViewAllUsers()
        {
            Console.Clear();
            Console.WriteLine("==== USER LIST ====\n");

            foreach (var user in RegisteredUsers)
            {
                Console.WriteLine($"ID: {user.UserId} | Age: {user.Age} | Sex: {user.Gender} | Created: {user.CreatedAt:dd/MM/yyyy}");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void SearchUser()
        {
            Console.Clear();
            Console.WriteLine("==== SEARCH USER ====\n");

            Console.Write("Enter User ID: ");
            string userId = Console.ReadLine();

            var user = RegisteredUsers.Find(u => u.UserId == userId);
            if (user != null)
            {
                Console.WriteLine($"\nUser ID : {user.UserId}");
                Console.WriteLine($"Name    : {user.Username}");
                Console.WriteLine($"Email   : {user.Email}");
                Console.WriteLine($"DOB     : {user.DateOfBirth:dd/MM/yyyy} (Age: {user.Age})");
                Console.WriteLine($"Sex     : {user.Gender}");
                Console.WriteLine($"Joined  : {user.CreatedAt:dd/MM/yyyy}");
            }
            else
            {
                Console.WriteLine("User not found.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void CreateOrEditUser()
        {
            Console.Clear();
            Console.WriteLine("==== CREATE OR EDIT USER ====\n");

            Console.Write("Enter existing User ID to edit (or leave empty to create new): ");
            string inputId = Console.ReadLine();

            User user;

            if (!string.IsNullOrWhiteSpace(inputId))
            {
                user = RegisteredUsers.Find(u => u.UserId == inputId);
                if (user == null)
                {
                    Console.WriteLine("❌ User not found.");
                    Console.WriteLine("Press Enter to return...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nEditing existing user...");
            }
            else
            {
                user = new User
                {
                    UserId = UserHelper.GenerateUniqueUsername(),
                    CreatedAt = DateTime.Now
                };
                Console.WriteLine($"New User ID: {user.UserId}");
            }

            Console.Write("Enter Display Name: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) user.Username = name;

            Console.Write("Enter Date of Birth (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dob))
            {
                user.DateOfBirth = dob;
                user.Age = User.CalculateAgeFromDob(dob);
            }

            Console.Write("Enter Gender (M/F): ");
            string genderInput = Console.ReadLine().ToUpper();
            if (genderInput == "M") user.Gender = Gender.Male;
            else if (genderInput == "F") user.Gender = Gender.Female;

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email;

            if (!RegisteredUsers.Exists(u => u.UserId == user.UserId))
            {
                RegisteredUsers.Add(user);
                Console.WriteLine("✅ New user created.");
            }
            else
            {
                Console.WriteLine("✅ User updated.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }


        static void DeleteUser()
        {
            Console.Clear();
            Console.WriteLine("==== DELETE USER ====\n");

            Console.Write("Enter User ID to delete: ");
            string userId = Console.ReadLine();

            var user = RegisteredUsers.Find(u => u.UserId == userId);
            if (user != null)
            {
                Console.Write("Are you sure? Type DELETE to confirm: ");
                if (Console.ReadLine().Trim().ToUpper() == "DELETE")
                {
                    RegisteredUsers.Remove(user);
                    Console.WriteLine("✅ User deleted.");
                }
                else
                {
                    Console.WriteLine("❎ Cancelled.");
                }
            }
            else
            {
                Console.WriteLine("User not found.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }
    }
}

