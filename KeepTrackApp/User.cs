using System;

namespace KeepTrackApp.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool Login() => true;
        public void Logout() {}
        public void ChangePassword() {}
        public void DeleteAccount() {}
        public void UpdateProfile() {}
        public void UploadProfilePicture(string filePath) {}
        public object GetDashboardSummary() => new {};
    }
}
