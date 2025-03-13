using System.Windows;
using ProvaHidrica.Database;
using ProvaHidrica.Services;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();

            Task.Run(() =>
                {
                    //Thread.Sleep(2000);

                    try
                    {
                        // Try a connection to the database
                        DbConnectionFactory connectionFactory = new();
                        Db db = new(connectionFactory);
                    }
                    catch
                    {
                        // If the connection fails, application shoul be closed
                        ShowErrorMessage(
                            "Erro ao conectar com o banco de dados. A aplicação será encerrda."
                        );
                        Dispatcher.Invoke(() => App.Current.Shutdown());
                    }

                    try
                    {
                        Api.Init();
                    }
                    catch
                    {
                        // If the initialization fails, application shoul be closed
                        ShowErrorMessage("Erro ao conectar a API. A aplicação será encerrda.");
                        Dispatcher.Invoke(() => App.Current.Shutdown());
                    }
                })
                .ContinueWith(
                    t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var mainWindow = new MainWindow();
                            Application.Current.MainWindow = mainWindow;
                            Close();
                        });
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
