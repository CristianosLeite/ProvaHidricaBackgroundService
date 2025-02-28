using ProvaHidrica.Interfaces;

namespace ProvaHidrica.Database
{
    public class DbConfig : IDbConfig
    {
        public static string? ConnectionString { get; set; } =
            Environment.GetEnvironmentVariable("PROVA_HIDRICA_DB_CONNECTION");
    }
}
