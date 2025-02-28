namespace ProvaHidrica.Interfaces
{
    internal interface IDbConfig
    {
        public static string? ConnectionString { get; } =
            Environment.GetEnvironmentVariable("PICKBYOPEN_DB_CONNECTION");
    }
}
