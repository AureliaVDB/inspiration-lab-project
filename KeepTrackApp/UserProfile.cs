using System;

namespace KeepTrackApp.Models
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public float HeightCm { get; set; }
        public float CalorieGoal { get; set; }
        public float ProteinGoal { get; set; }
        public float CarbGoal { get; set; }
        public float FatGoal { get; set; }

        public int CalculateAge()
        {
            var today = DateTime.Today;
            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public float CalculateBMI()
        {
            float heightMeters = HeightCm / 100;
            return heightMeters > 0 ? CalorieGoal / (heightMeters * heightMeters) : 0;
        }

        public void UpdateGoals(float calorie, float protein, float carb, float fat)
        {
            CalorieGoal = calorie;
            ProteinGoal = protein;
            CarbGoal = carb;
            FatGoal = fat;
        }
    }
}
