using System.Collections.ObjectModel;
using Npgsql;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Models;

namespace ProvaHidrica.Database
{
    internal class OperationRepository(IDbConnectionFactory connectionFactory)
        : IOperationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<ObservableCollection<Operation>> LoadOperations()
        {
            try
            {
                var operations = new ObservableCollection<Operation>();

                using var connection = _connectionFactory.GetConnection();
                connection?.Open();

                using var command = new NpgsqlCommand(
                    "SELECT * FROM operations ORDER BY \"createdAt\" DESC",
                    connection
                );
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    operations.Add(MapOperation(reader));
                }

                return operations;
            }
            catch (PostgresException e)
            {
                throw new Exception("Erro ao carregar a lista de operações." + e);
            }
        }

        public async Task<List<Operation>> GetOperationsByDate(
            string opInfo,
            string initialDate,
            string finalDate
        )
        {
            var operations = new List<Operation>();

            using var connection = _connectionFactory.GetConnection();
            connection?.Open();

            string? query = BuildQuery(opInfo);

            if (query == null)
                return operations;

            using var command = new NpgsqlCommand(query, connection);
            AddParameters(command, opInfo, initialDate, finalDate);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                operations.Add(MapOperation(reader));
            }

            return operations;
        }

        private static Operation MapOperation(NpgsqlDataReader reader)
        {
            return new Operation(
                reader.GetGuid(0).ToString(),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetInt32(5),
                reader.GetDateTime(6),
                reader.GetDateTime(7),
                reader.GetTimeSpan(8),
                reader.GetString(9),
                reader.GetBoolean(10),
                reader.GetBoolean(11),
                reader.GetBoolean(12),
                reader.GetBoolean(13),
                reader.GetBoolean(14),
                reader.GetBoolean(15),
                reader.GetBoolean(16),
                reader.GetBoolean(17),
                reader.GetBoolean(18),
                reader.GetBoolean(19),
                reader.GetBoolean(20),
                reader.GetBoolean(21),
                reader.GetBoolean(22),
                reader.GetBoolean(23),
                reader.GetBoolean(24),
                reader.GetBoolean(25),
                reader.GetBoolean(26),
                reader.GetBoolean(27),
                reader.GetBoolean(28),
                reader.GetBoolean(29),
                reader.GetBoolean(30),
                reader.GetBoolean(31),
                reader.GetBoolean(32),
                reader.GetBoolean(33),
                reader.GetBoolean(34),
                reader.GetBoolean(35),
                reader.GetBoolean(36),
                reader.GetBoolean(37),
                reader.GetBoolean(38),
                reader.GetBoolean(39),
                reader.GetBoolean(40),
                reader.GetBoolean(41),
                reader.GetBoolean(42),
                reader.GetDateTime(43)
            );
        }

        private static string? BuildQuery(string opInfo)
        {
            if (opInfo == string.Empty)
            {
                return "SELECT * FROM operations WHERE \"createdAt\" BETWEEN @initialDate AND @finalDate ORDER BY \"createdAt\" DESC";
            }
            else if (opInfo.Length == 14) // VP
            {
                return "SELECT * FROM operations WHERE vp = @target AND \"createdAt\" BETWEEN @initialDate AND @finalDate ORDER BY \"createdAt\" DESC";
            }
            else if (opInfo.Length == 17) // Chassis
            {
                return "SELECT * FROM operations WHERE chassis = @target AND \"createdAt\" BETWEEN @initialDate AND @finalDate ORDER BY \"createdAt\" DESC";
            }
            else if (opInfo.Length == 8) // CIS
            {
                return "SELECT * FROM operations WHERE cis = @target AND \"createdAt\" BETWEEN @initialDate AND @finalDate ORDER BY \"createdAt\" DESC";
            }
            else
            {
                return null;
            }
        }

        private static void AddParameters(
            NpgsqlCommand command,
            string opInfo,
            string initialDate,
            string finalDate
        )
        {
            if (!string.IsNullOrEmpty(opInfo))
            {
                command.Parameters.AddWithValue("@target", opInfo);
            }

            command.Parameters.AddWithValue("@initialDate", DateTime.Parse(initialDate));
            command.Parameters.AddWithValue("@finalDate", DateTime.Parse(finalDate).AddDays(1.0));
        }
    }
}
