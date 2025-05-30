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
    public partial class AdminDashboardWindow : Window
    {
        private string currentAdminId;

        public AdminDashboardWindow(string adminId)
        {
            InitializeComponent();
            currentAdminId = adminId;
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

