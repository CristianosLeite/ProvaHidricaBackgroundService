using System.Windows;
using ProvaHidrica.Database;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Windows;

namespace ProvaHidrica.Services
{
    public class LogService(IDbConnectionFactory connectionFactory)
        : LogRepository(connectionFactory)
    {
        public void ShowLogs(object sender, RoutedEventArgs e)
        {
            LogsWindow logs = new();
            logs.Show();
        }
    }
}
