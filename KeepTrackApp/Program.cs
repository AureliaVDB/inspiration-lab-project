using System;
using KeepTrackApp.Controllers;
using KeepTrackApp.Models;
using KeepTrackApp.Utils;

namespace KeepTrackApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new User
            {
                UserId = "U001",
                Username = "testuser",
                Email = "test@example.com",
                Password = "1234",
                CreatedAt = DateTime.Now
            };

            var ui = new ConsoleUserInterface();
            var controller = new UserController(ui, user);

            ui.DisplayMessage("Welcome to KeepTrack!");
            controller.Login();
            controller.ShowDashboard();
            controller.ChangePassword();
            controller.Logout();
        }
    }
}
