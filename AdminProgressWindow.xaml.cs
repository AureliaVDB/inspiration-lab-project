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
using System.Globalization;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;

namespace KeepTrackAppUI
{
    public partial class AdminProgressWindow : Window
    {
        private string currentAdminId;
        private string connectionString = "server=localhost;port=3307;user=root;password=Svana13*;database=keeptrack;";
        public AdminProgressWindow(string adminId)
        {
            InitializeComponent();
            currentAdminId = adminId;
        }

        private void LoadUserProgress_Click(object sender, RoutedEventArgs e)
        {
            string userId = UserSearchBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userId))
            {
                MessageBox.Show("Please enter a valid user ID.");
                return;
            }

            LoadWeightChart(userId);
            LoadMacroChart(userId);
        }

        private void LoadWeightChart(string userId)
        {
            var weeklyAverages = new Dictionary<string, double>();
            var weeklyGroups = new Dictionary<string, List<double>>();
            var culture = CultureInfo.CurrentCulture;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Weight, Date FROM Progress WHERE UserId = @UserId ORDER BY Date ASC";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                double weight = Convert.ToDouble(reader["Weight"]);
                DateTime date = Convert.ToDateTime(reader["Date"]);

                int week = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                string key = $"{date.Year}-W{week}";

                if (!weeklyGroups.ContainsKey(key))
                    weeklyGroups[key] = new List<double>();

                weeklyGroups[key].Add(weight);
            }

            foreach (var group in weeklyGroups)
                weeklyAverages[group.Key] = group.Value.Average();

            WeightChart.Series = new SeriesCollection
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

            WeightChart.AxisX.Clear();
            WeightChart.AxisX.Add(new Axis { Labels = weeklyAverages.Keys.ToList(), Title = "Week" });
            WeightChart.AxisY.Clear();
            WeightChart.AxisY.Add(new Axis { Title = "Weight (kg)", LabelFormatter = value => value.ToString("0") });
        }


        private void LoadMacroChart(string userId)
        {
            var dailyMacros = new Dictionary<string, (float carbs, float protein, float fat)>();

            float weightKg = GetMostRecentWeight(userId);
            float targetCarbs = weightKg * 3f;
            float targetProtein = weightKg * 2f;
            float targetFat = weightKg * 1f;
            float targetCalories = (targetCarbs + targetProtein) * 4 + targetFat * 9;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"SELECT Date, SUM(Carbs) AS C, SUM(Protein) AS P, SUM(Fat) AS F
                             FROM MealLog
                             WHERE UserId = @UserId
                             GROUP BY Date
                             ORDER BY Date DESC
                             LIMIT 7";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string date = Convert.ToDateTime(reader["Date"]).ToString("ddd");
                float carbs = Convert.ToSingle(reader["C"]);
                float protein = Convert.ToSingle(reader["P"]);
                float fat = Convert.ToSingle(reader["F"]);
                dailyMacros[date] = (carbs, protein, fat);
            }

            var days = dailyMacros.Keys.Reverse().ToList();
            var carbsData = new ChartValues<float>(dailyMacros.Values.Reverse().Select(d => d.carbs));
            var proteinData = new ChartValues<float>(dailyMacros.Values.Reverse().Select(d => d.protein));
            var fatData = new ChartValues<float>(dailyMacros.Values.Reverse().Select(d => d.fat));
            var caloriesData = new ChartValues<float>(dailyMacros.Values.Reverse().Select(d =>
                d.carbs * 4 + d.protein * 4 + d.fat * 9));

            MacroChart.Series = new SeriesCollection
            {
                new LineSeries { Title = "Carbs", Values = carbsData, Stroke = Brushes.Orange, Fill = Brushes.Transparent },
                new LineSeries { Title = "Protein", Values = proteinData, Stroke = Brushes.Blue, Fill = Brushes.Transparent },
                new LineSeries { Title = "Fat", Values = fatData, Stroke = Brushes.Red, Fill = Brushes.Transparent },
                new LineSeries { Title = "Calories", Values = caloriesData, Stroke = Brushes.Black, Fill = Brushes.Transparent }
            };

            MacroChart.AxisX.Clear();
            MacroChart.AxisX.Add(new Axis { Labels = days, Title = "Day" });

            MacroChart.AxisY.Clear();
            MacroChart.AxisY.Add(new Axis { Title = "Grams / kcal", LabelFormatter = value => value.ToString("0") });
        }
        private float GetMostRecentWeight(string userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Weight FROM Progress WHERE UserId = @UserId ORDER BY Date DESC LIMIT 1";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToSingle(result) : 70f;
        }
        private void GoToEditUsers_Click(object sender, RoutedEventArgs e)
        {
            new AdminUserManagerWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminRecipes_Click(object sender, RoutedEventArgs e)
        {
            new AdminRecipesWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminSupplements_Click(object sender, RoutedEventArgs e)
        {
            new AdminSupplementsWindow(currentAdminId).Show();
            this.Close();
        }

        private void GoToAdminProgress_Click(object sender, RoutedEventArgs e)
        {
            new AdminProgressWindow(currentAdminId).Show();
            this.Close();
        }
       


    }
}

