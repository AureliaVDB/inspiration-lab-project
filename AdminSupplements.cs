using System;
using System.Collections.Generic;
using KeepTrackApp.Models;
using MySql.Data.MySqlClient;


namespace KeepTrackApp
{
    public static class AdminSupplements
    {
        public static List<Supplement> Supplements { get; set; }

        // Caution message shown to both users and admins
        public static string CautionNotice =
            "⚠️ This information is for educational purposes only. Please consult a licensed healthcare provider before using any supplement. You are solely responsible for your health decisions.";

        public static void ManageSupplements()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ADMIN - SUPPLEMENT MANAGEMENT =====\n");

                Console.WriteLine("1. View All Supplements");
                Console.WriteLine("2. Add Supplement");
                Console.WriteLine("3. Edit Supplement");
                Console.WriteLine("4. Delete Supplement");
                Console.WriteLine("0. Return to Admin Dashboard");

                Console.Write("\nChoose an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewAll();
                        break;
                    case "2":
                        AddSupplement();
                        break;
                    case "3":
                        EditSupplement();
                        break;
                    case "4":
                        DeleteSupplement();
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
            Console.WriteLine("==== SUPPLEMENT LIST ====\n");

            Console.WriteLine(CautionNotice + "\n");

            foreach (var s in Supplements)
            {
                Console.WriteLine($"ID: {s.SupplementId} | Name: {s.Name}");
                Console.WriteLine($"Dosage: {s.Dosage}");
                Console.WriteLine($"Instructions: {s.Instructions}");
                Console.WriteLine($"Benefits: {s.Benefits}");
                Console.WriteLine($"Risks: {s.Risks}");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void AddSupplement()
        {
            Console.Clear();
            Console.WriteLine("==== ADD NEW SUPPLEMENT ====\n");

            var supplement = new Supplement
            {
                SupplementId = Supplements.Count + 1,
                AdminId = "Admin"
            };

            Console.Write("Name: ");
            supplement.Name = Console.ReadLine();

            Console.Write("Dosage: ");
            supplement.Dosage = Console.ReadLine();

            Console.Write("Instructions: ");
            supplement.Instructions = Console.ReadLine();

            Console.Write("Benefits: ");
            supplement.Benefits = Console.ReadLine();

            Console.Write("Risks: ");
            supplement.Risks = Console.ReadLine();

            Supplements.Add(supplement);
            //add to database
            string connectionString = "server=localhost;port=3306;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertSupplement = @"INSERT INTO Supplement 
                (AdminId, Name, Dosage, Instructions, Benefits, Risks)
                VALUES 
                (@AdminId, @Name, @Dosage, @Instructions, @Benefits, @Risks);";

            using var cmd = new MySqlCommand(insertSupplement, conn);
            cmd.Parameters.AddWithValue("@AdminId", supplement.AdminId);
            cmd.Parameters.AddWithValue("@Name", supplement.Name);
            cmd.Parameters.AddWithValue("@Dosage", supplement.Dosage);
            cmd.Parameters.AddWithValue("@Instructions", supplement.Instructions);
            cmd.Parameters.AddWithValue("@Benefits", supplement.Benefits);
            cmd.Parameters.AddWithValue("@Risks", supplement.Risks);

            cmd.ExecuteNonQuery();

            Console.WriteLine("\n✅ Supplement added. Press Enter to return...");
            Console.ReadLine();
        }

        static void EditSupplement()
        {
            Console.Clear();
            Console.WriteLine("==== EDIT SUPPLEMENT ====\n");

            Console.Write("Enter Supplement ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadLine();
                return;
            }

            var supplement = Supplements.Find(s => s.SupplementId == id);
            if (supplement == null)
            {
                Console.WriteLine("Supplement not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Leave blank to keep existing value.\n");

            Console.Write($"Name ({supplement.Name}): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) supplement.Name = input;

            Console.Write($"Dosage ({supplement.Dosage}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) supplement.Dosage = input;

            Console.Write($"Instructions ({supplement.Instructions}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) supplement.Instructions = input;

            Console.Write($"Benefits ({supplement.Benefits}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) supplement.Benefits = input;

            Console.Write($"Risks ({supplement.Risks}): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) supplement.Risks = input;

            Console.WriteLine("\n✅ Supplement updated. Press Enter to return...");
            Console.ReadLine();
        }

        static void DeleteSupplement()
        {
            Console.Clear();
            Console.WriteLine("==== DELETE SUPPLEMENT ====\n");

            Console.Write("Enter Supplement ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Console.ReadLine();
                return;
            }

            var supplement = Supplements.Find(s => s.SupplementId == id);
            if (supplement == null)
            {
                Console.WriteLine("Supplement not found.");
                Console.ReadLine();
                return;
            }

            Console.Write("Type DELETE to confirm: ");
            if (Console.ReadLine().ToUpper() == "DELETE")
            {
                Supplements.Remove(supplement);
                Console.WriteLine("✅ Supplement deleted.");
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

