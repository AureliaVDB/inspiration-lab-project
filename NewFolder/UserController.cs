using KeepTrackApp.Models;
using KeepTrackApp.Interfaces;
using System;

namespace KeepTrackApp.Controllers
{
    public class UserController
    {
        private readonly IUserInterface _ui;
        private readonly User _user;

        public UserController(IUserInterface ui, User user)
        {
            _ui = ui;
            _user = user;
        }

        public void Login()
        {
            _ui.DisplayMessage("Logging in...");
            bool result = _user.Login();
            _ui.DisplayMessage(result ? "Login successful!" : "Login failed.");
        }

        public void ShowDashboard()
        {
            _ui.DisplayDashboard();
            var summary = _user.GetDashboardSummary();
            _ui.DisplayMessage(summary.ToString());
        }

        public void ChangePassword()
        {
            _ui.DisplayMessage("Enter new password:");
            string newPassword = _ui.GetUserInput();
            _user.Password = newPassword;
            _user.ChangePassword();
            _ui.DisplayMessage("Password changed.");
        }

        public void Logout()
        {
            _user.Logout();
            _ui.DisplayMessage("Logged out successfully.");
        }
    }
}
