using System.Windows;
using Npgsql;
using ProvaHidrica.Interfaces;

namespace ProvaHidrica.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public NpgsqlConnection? GetConnection()
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable(
                    "PROVA_HIDRICA_DB_CONNECTION"
                );
                return new NpgsqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return null;
            }
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
