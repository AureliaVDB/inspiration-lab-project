using System;
using KeepTrackApp.Interfaces;

namespace KeepTrackApp.Utils
{
    public class ConsoleUserInterface : IUserInterface
    {
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string GetUserInput()
        {
            return Console.ReadLine();
        }

        public void DisplayDashboard()
        {
            Console.WriteLine("Welcome to your dashboard!");
        }
    }
}
