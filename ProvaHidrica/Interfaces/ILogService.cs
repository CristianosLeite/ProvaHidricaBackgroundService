using System.Windows;

namespace ProvaHidrica.Interfaces
{
    public interface ILogService : ILogRepository
    {
        void ShowLogs(object sender, RoutedEventArgs e);
    }
}
