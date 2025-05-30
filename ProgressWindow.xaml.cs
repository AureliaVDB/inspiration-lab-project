using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Data;





namespace KeepTrackAppUI
{
    public partial class ProgressWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> WeekLabels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private string currentUserId;
        public SeriesCollection MacroSeries { get; set; }
        public List<string> DayLabels { get; set; } = new();
        private float currentWeight;


        public ProgressWindow(string userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadWeeklyWeights();
            LoadWeeklyWeightChart();
            LoadMacrosRemaining();
            LoadTodayMealLog();
            MacroSelector.SelectedIndex = 0;
        }

        public ProgressWindow()
        {
            InitializeComponent();
            LoadWeeklyWeights();
        }

        private void SubmitWeight_Click(object sender, RoutedEventArgs e)
        {
            if (!float.TryParse(WeightBox.Text.Trim(), out float weightKg) || weightKg <= 0)
            {
                MessageBox.Show("Please enter a valid weight.");
                return;
            }

            // 1. Save to MySQL
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insert = @"INSERT INTO Progress (UserId, Date, Weight) 
                              VALUES (@UserId, @Date, @Weight)";
            using var cmd = new MySqlCommand(insert, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today);
            cmd.Parameters.AddWithValue("@Weight", weightKg);
            cmd.ExecuteNonQuery();

            // 2. Calculate Macros
            float proteinTarget = weightKg * 2f;
            float fatTarget = weightKg * 1f;
            float carbTarget = weightKg * 3f;
            float calorieTarget = (proteinTarget * 4) + (fatTarget * 9) + (carbTarget * 4);

            MacroResult.Text =
                $"🎯 Target Macros for {weightKg} kg:\n" +
                $"Protein: {proteinTarget}g\n" +
                $"Fats: {fatTarget}g\n" +
                $"Carbs: {carbTarget}g\n" +
                $"Calories: {calorieTarget} kcal";

            // 3. Refresh list
            LoadWeeklyWeights();
        }

        private void LoadWeeklyWeights()
        {
            var weights = new List<string>();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            DateTime startOfWeek = GetStartOfWeek(DateTime.Today);
            string query = @"SELECT Date, Weight FROM Progress 
                             WHERE UserId = @UserId AND Date >= @StartOfWeek 
                             ORDER BY Date";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var day = Convert.ToDateTime(reader["Date"]).ToString("dddd");
                var weight = reader["Weight"].ToString();
                weights.Add($"📅 {day}: {weight} kg");
            }

            WeightList.ItemsSource = weights;
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-diff).Date;
        }

        private void GoToRecipes_Click(object sender, RoutedEventArgs e)
        {
            new RecipesWindow().Show();
        }

        private void GoToSupplements_Click(object sender, RoutedEventArgs e)
        {
            new SupplementWindow().Show();
        }
        private void GoToProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfileWindow(currentUserId).Show();
            this.Close();
        }
        private void GoToProgress_Click(object sender, RoutedEventArgs e)
        {
            new ProgressWindow(currentUserId).Show();
            this.Close();
        }
        private void LoadWeeklyWeightChart()
        {
            var weeklyAverages = new Dictionary<string, double>();
            var culture = CultureInfo.CurrentCulture;

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Weight, Date FROM Progress WHERE UserId = @UserId ORDER BY Date ASC";
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);

            using var reader = cmd.ExecuteReader();
            var weeklyGroups = new Dictionary<string, List<double>>();

            while (reader.Read())
            {
                double weight = Convert.ToDouble(reader["Weight"]);
                DateTime date = Convert.ToDateTime(reader["Date"]);

                // Group by year-week string like "2025-W21"
                int week = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                string key = $"{date.Year}-W{week}";

                if (!weeklyGroups.ContainsKey(key))
                    weeklyGroups[key] = new List<double>();

                weeklyGroups[key].Add(weight);
            }

            foreach (var group in weeklyGroups)
            {
                weeklyAverages[group.Key] = group.Value.Average();
            }

            // Bind to chart
            WeekLabels = weeklyAverages.Keys.ToList();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Average Weight",
                    Values = new ChartValues<double>(weeklyAverages.Values),
                    PointGeometry = DefaultGeometries.Circle,
                    Stroke = Brushes.Green,
                    Fill = Brushes.Transparent
                }
            };

            Formatter = value => value.ToString("0.0");

            WeightChart.Series = SeriesCollection;
            WeightChart.AxisX[0].Labels = WeekLabels;
            WeightChart.AxisY[0].LabelFormatter = Formatter;
        }
        private void AddMeal_Click(object sender, RoutedEventArgs e)
        {
            if (!float.TryParse(CarbInput.Text, out float carbs) ||
                !float.TryParse(ProteinInput.Text, out float protein) ||
                !float.TryParse(FatInput.Text, out float fat))
            {
                MessageBox.Show("Please enter valid numbers for all macros.");
                return;
            }

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string insertQuery = @"INSERT INTO MealLog (UserId, Date, Carbs, Protein, Fat)
                           VALUES (@UserId, @Date, @Carbs, @Protein, @Fat)";
            using var cmd = new MySqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today);
            cmd.Parameters.AddWithValue("@Carbs", carbs);
            cmd.Parameters.AddWithValue("@Protein", protein);
            cmd.Parameters.AddWithValue("@Fat", fat);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Meal logged!");
            LoadMacrosRemaining(); 
            LoadTodayMealLog();
        }
        private void LoadMacrosRemaining()
        {
            float totalCarbs = 0, totalProtein = 0, totalFat = 0;
            float weightKg = GetMostRecentWeight();

            // Get daily macro targets
            float targetCarbs = weightKg * 3f;
            float targetProtein = weightKg * 2f;
            float targetFat = weightKg * 1f;

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string select = "SELECT SUM(Carbs), SUM(Protein), SUM(Fat) FROM MealLog WHERE UserId = @UserId AND Date = @Date";
            using var cmd = new MySqlCommand(select, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                totalCarbs = reader.IsDBNull(0) ? 0 : Convert.ToSingle(reader.GetDouble(0));
                totalProtein = reader.IsDBNull(1) ? 0 : Convert.ToSingle(reader.GetDouble(1));
                totalFat = reader.IsDBNull(2) ? 0 : Convert.ToSingle(reader.GetDouble(2));
            }

            float remainingCarbs = targetCarbs - totalCarbs;
            float remainingProtein = targetProtein - totalProtein;
            float remainingFat = targetFat - totalFat;

            float totalConsumedCalories = (totalCarbs * 4) + (totalProtein * 4) + (totalFat * 9);
            float totalTargetCalories = (targetCarbs * 4) + (targetProtein * 4) + (targetFat * 9);
            float remainingCalories = totalTargetCalories - totalConsumedCalories;

            RemainingMacrosText.Text = $"Macros left today:\n" +
                $"Carbs: {remainingCarbs:0.0}g\n" +
                $"Protein: {remainingProtein:0.0}g\n" +
                $"Fat: {remainingFat:0.0}g\n" +
                $"Calories: {remainingCalories:0.0} kcal";
        }
        private float GetMostRecentWeight()
        {
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Weight FROM Progress WHERE UserId = @UserId ORDER BY Date DESC LIMIT 1";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToSingle(result) : 70f; // default 70kg if no weight yet
        }
        private void LoadTodayMealLog()
        {
            List<string> meals = new();
            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"SELECT Carbs, Protein, Fat FROM MealLog
                     WHERE UserId = @UserId AND Date = @Date
                     ORDER BY Date DESC";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                float carbs = Convert.ToSingle(reader["Carbs"]);
                float protein = Convert.ToSingle(reader["Protein"]);
                float fat = Convert.ToSingle(reader["Fat"]);

                float calories = (carbs * 4) + (protein * 4) + (fat * 9);
                meals.Add($"• {calories:0} kcal — {carbs}g carbs, {protein}g protein, {fat}g fat");
            }

            LoggedMealsList.ItemsSource = meals;
        }
        private void LoadMacroChart(string macroType)
        {
            DayLabels = new List<string>();
            ChartValues<double> actualValues = new();
            ChartValues<double> goalLine = new();

            float weight = currentWeight == 0 ? GetMostRecentWeight() : currentWeight;

            // Targets
            float carbTarget = weight * 3f;
            float proteinTarget = weight * 2f;
            float fatTarget = weight * 1f;
            float calorieTarget = (carbTarget * 4) + (proteinTarget * 4) + (fatTarget * 9);

            float dailyGoal = macroType switch
            {
                "Carbs" => carbTarget,
                "Protein" => proteinTarget,
                "Fat" => fatTarget,
                "Calories" => calorieTarget,
                _ => 0
            };

            string dbColumn = macroType;
            if (macroType == "Calories")
                dbColumn = "Carbs, Protein, Fat"; // we'll calculate total manually

            DateTime start = GetStartOfWeek(DateTime.Today);

            string query = $@"SELECT Date, SUM(Carbs) AS Carbs, SUM(Protein) AS Protein, SUM(Fat) AS Fat
                      FROM MealLog
                      WHERE UserId = @UserId AND Date >= @Start
                      GROUP BY Date
                      ORDER BY Date";

            string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", currentUserId);
            cmd.Parameters.AddWithValue("@Start", start);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DateTime date = Convert.ToDateTime(reader["Date"]);
                float c = Convert.ToSingle(reader["Carbs"]);
                float p = Convert.ToSingle(reader["Protein"]);
                float f = Convert.ToSingle(reader["Fat"]);
                double value = macroType switch
                {
                    "Carbs" => c,
                    "Protein" => p,
                    "Fat" => f,
                    "Calories" => (c * 4) + (p * 4) + (f * 9),
                    _ => 0
                };

                DayLabels.Add(date.ToString("ddd"));
                actualValues.Add(value);
                goalLine.Add(dailyGoal);
            }

            MacroSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = macroType,
                    Values = actualValues,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Goal",
                    Values = goalLine,
                    StrokeDashArray = new DoubleCollection { 2 },
                    Stroke = Brushes.Gray,
                    Fill = Brushes.Transparent
                }
            };

            Formatter = value => value.ToString("0");

            MacroChart.Series = MacroSeries;
            MacroChart.AxisX[0].Labels = DayLabels;
            MacroChart.AxisY[0].LabelFormatter = Formatter;
        }
        private void MacroSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MacroSelector.SelectedItem is ComboBoxItem selected)
            {
                string macro = selected.Content.ToString();
                LoadMacroChart(macro);
            }
        }





    }
}

