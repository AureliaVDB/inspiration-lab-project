namespace KeepTrackApp.Models
{
    public enum RecipeCategory { Breakfast, Lunch, Dinner, Snack }

    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public RecipeCategory Category { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string Image { get; set; }
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fat { get; set; }
        public string AdminId { get; set; }
    }
}
