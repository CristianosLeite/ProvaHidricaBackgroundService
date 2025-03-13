using System.Windows;

namespace ProvaHidrica
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "ProvaHidrica";

            mutex = new(true, appName, out bool createdNew);

            if (!createdNew)
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
