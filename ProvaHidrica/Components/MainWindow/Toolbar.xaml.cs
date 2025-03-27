using System.Windows;
using System.Windows.Controls;
using ProvaHidrica.Services;
using ProvaHidrica.Windows;

namespace ProvaHidrica.Components
{
    /// <summary>
    /// Interação lógica para Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            if (!Auth.UserHasPermission("RU"))
            {
                MessageBox.Show(
                    "Você não tem permissão para acessar essa funcionalidade.",
                    "Acesso negado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            UserWindow users = new();
            users.Show();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            if (!Auth.UserHasPermission("ER"))
            {
                MessageBox.Show(
                    "Você não tem permissão para acessar essa funcionalidade.",
                    "Acesso negado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            ReportWindow reports = new();
            reports.Show();
        }

        private void Recipe_Click(object sender, RoutedEventArgs e)
        {
            if (!Auth.UserHasPermission("RR"))
            {
                MessageBox.Show(
                    "Você não tem permissão para acessar essa funcionalidade.",
                    "Acesso negado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            RecipeWindow recipe = new();
            recipe.Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            if (!Auth.UserHasPermission("MS"))
            {
                MessageBox.Show(
                    "Você não tem permissão para acessar essa funcionalidade.",
                    "Acesso negado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            ConfigWindow config = new();
            config.Show();
        }
    }
}
