using Npgsql;

namespace ProvaHidrica.Interfaces
{
    public interface IDbConnectionFactory
    {
        NpgsqlConnection? GetConnection();
    }
}
