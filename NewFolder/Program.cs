using System;
using System.Collections.Generic;
using KeepTrackApp.Models;
using KeepTrackApp.Utils;

namespace KeepTrackApp
{
    class Program
    {
        static List<User> registeredUsers = new();
        static List<Admin> admins = new()
        {
            new Admin { AdminId = "Raja", Password = "Admin" },
            new Admin { AdminId = "Aurelia", Password = "Admin" }
        };

        static int userCounter = 0000000;

        static void Main(string[] args)
        {
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
                Console.WriteLine($" Admin login {admin.AdminId}!");
                return;
            }

            // Check User Login
            var user = registeredUsers.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Console.WriteLine($" User login {user.Username}!");
                return;
            }

            Console.WriteLine(" try again");
        }

        static void RegisterUser()
        {
            Console.WriteLine("\n Keep track ");

            string userId = $"ki{userCounter++}";
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
            UserHelper.SendVerificationEmail(email, code);

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
                Console.WriteLine("❌ Password must be 8+ chars with uppercase, lowercase, number, symbol.");
                return;
            }

            Console.Write("Confirm password: ");
            string confirm = Console.ReadLine();
            if (password != confirm)
            {
                Console.WriteLine("❌ Passwords do not match.");
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
            Console.WriteLine($"\n✅ Registration successful! Welcome, {user.Username}!");
        }
    }
}
