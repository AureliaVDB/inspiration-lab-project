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


namespace KeepTrackAppUI
{
    public partial class DashboardWindow : Window
    {
        private string currentUserId;

        public DashboardWindow(string userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void GoToRecipes_Click(object sender, RoutedEventArgs e)
        {
            new RecipesWindow().Show();
            this.Close();
        }

        private void GoToSupplements_Click(object sender, RoutedEventArgs e)
        {
            new SupplementWindow().Show();
            this.Close();
        }

        private void TrackProgress_Click(object sender, RoutedEventArgs e)
        {
            new ProgressWindow(currentUserId).Show();
            this.Close();
        }

        private void GoToProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfileWindow(currentUserId).Show();
            this.Close();
        }
    }
}

