using System;
using System.Collections.Generic;
using KeepTrackApp.Models;
using KeepTrackApp.Utils;


namespace KeepTrackApp
{
    public static class AdminProgress
    {
        public static List<User> RegisteredUsers { get; set; }
        public static Dictionary<string, List<MealLog>> UserMeals { get; set; }
        public static Dictionary<string, Progress> UserProgress { get; set; }

        public static void ViewUserProgress()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====== ADMIN - USER PROGRESS ======\n");

                Console.WriteLine("All Users:\n");
                foreach (var user in RegisteredUsers)
                    Console.WriteLine($"ID: {user.UserId} | Age: {user.Age} | Sex: {user.Gender} | Added: {user.CreatedAt:dd/MM/yyyy}");

                Console.Write("\nEnter User ID to view progress (or 0 to return): ");
                string userId = Console.ReadLine();
                if (userId == "0") return;

                var selected_user = RegisteredUsers.Find(u => u.UserId == userId);
                if (selected_user == null)
                {
                    Console.WriteLine("❌ User not found. Press Enter to try again.");
                    Console.ReadLine();
                    continue;
                }

                ShowProgressForUser(selected_user);
            }
        }

        static void ShowProgressForUser(User user)
        {
            Console.Clear();
            Console.WriteLine($"===== PROGRESS FOR {user.Username.ToUpper()} =====\n");

            // Weight entries
            if (UserProgress.ContainsKey(user.UserId))
            {
                Console.WriteLine("Weight Entries:");
                foreach (var (day, weight) in UserProgress[user.UserId].WeightEntries)
                {
                    Console.WriteLine($"📅 {day:dd/MM/yyyy}: {weight} kg");
                }
            }
            else
            {
                Console.WriteLine("No weight data available.");
            }

            Console.WriteLine("\n📊 Weekly Macro Overview");

            DateTime startOfWeek = GetStartOfWeek(DateTime.Today);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            float weeklyP = 0, weeklyF = 0, weeklyC = 0, weeklyKcal = 0;
            int daysWithMeals = 0;

            for (int i = 0; i < 7; i++)
            {
                DateTime date = startOfWeek.AddDays(i);
                var meals = UserMeals.ContainsKey(user.UserId)
                    ? UserMeals[user.UserId].FindAll(m => m.Date.Date == date)
                    : new List<MealLog>();

                if (meals.Count == 0) continue;

                Console.WriteLine($"\n📌 Day {i + 1} - {date:dddd}");

                float dayP = 0, dayF = 0, dayC = 0, dayKcal = 0;
                foreach (var m in meals)
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

            if (daysWithMeals > 0)
            {
                Console.WriteLine("\n======= WEEK RECAP =======");
                Console.WriteLine($"Avg Protein : {weeklyP / daysWithMeals}g");
                Console.WriteLine($"Avg Fat     : {weeklyF / daysWithMeals}g");
                Console.WriteLine($"Avg Carbs   : {weeklyC / daysWithMeals}g");
                Console.WriteLine($"Avg Calories: {weeklyKcal / daysWithMeals} kcal");
            }
            else
            {
                Console.WriteLine("No meals tracked for this week.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static DateTime GetStartOfWeek(DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}

