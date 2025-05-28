using System;

namespace KeepTrackApp.Models
{
    public enum MealCategory { Breakfast, Lunch, Dinner, Snack }

    public class MealLog
    {
        public int MealId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public MealCategory Category { get; set; }
        public string Description { get; set; }
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fat { get; set; }

        public object GetDailyMacros() => new {};
        public void EditMeal() {}
        public void DeleteMeal() {}
        public void AutoFillFromRecipe(int recipeId) {}
    }
}
