using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProvaHidrica.Database;
using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Services;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

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
                            var serviceCollection = new ServiceCollection();
                            ConfigureServices(serviceCollection);
                            ServiceProvider = serviceCollection.BuildServiceProvider();

                            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                            Application.Current.MainWindow = mainWindow;
                            Close();
                        });
                    },
                    TaskScheduler.FromCurrentSynchronizationContext()
                );
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<Db>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<DbConnectionFactory>();
            services.AddSingleton<Db>();
            services.AddSingleton<Plc>();
            services.AddSingleton<PlcService>();
            services.AddSingleton<NfcService>();
            services.AddTransient<MainWindow>();
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
