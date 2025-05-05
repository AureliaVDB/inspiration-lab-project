using System;

namespace KeepTrackApp.Models
{
    public enum Gender
    {
        Male,
        Female
    }

    public class User
    {
        public string UserId { get; set; } // kiXXXXXXX
        public string Username { get; set; } // Display name
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public static int CalculateAgeFromDob(DateTime dob)
        {
            var today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}


