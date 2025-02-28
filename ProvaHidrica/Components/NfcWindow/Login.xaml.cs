using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Services;
using ProvaHidrica.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace ProvaHidrica.Components
{
    /// <summary>
    /// Interação lógica para Login.xam
    /// </summary>
    public partial class Login : UserControl
    {
        private readonly Db db;
        public bool IsWorkDone { get; private set; }

        public Login()
        {
            InitializeComponent();

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);
        }

        private void ReloadWindow(object sernder, RoutedEventArgs e)
        {
            if (!IsWorkDone)
                Dispatcher.BeginInvoke(() =>
                {
                    App.Current.MainWindow.Effect = new BlurEffect();
                    NfcWindow nfcWindow = new(Types.Context.Login, new("", "", "", []));
                    nfcWindow.Show();
                    Dispatcher.InvokeShutdown();
                });
            App.Current.MainWindow.IsEnabled = true;
            App.Current.MainWindow.Effect = null;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            User? user = await db.FindUserByBadgeNumber(BadgeNumber.Text);

            if (user != null)
            {
                IsWorkDone = true;
                Auth.SetLoggedInUser(user);
                Auth.SetLoggedAt(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                Window.GetWindow(this)!.Close();
            }
            else
            {
                MessageBox.Show(
                    "Usuário não encontrado",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
