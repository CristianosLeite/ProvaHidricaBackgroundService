using Npgsql;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Models;
using ProvaHidrica.Types;
using ProvaHidrica.Utils;
using System.Collections.ObjectModel;

namespace ProvaHidrica.Database
{
    public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        //<summary>
        // Load a list of users from the database
        //</summary>
        public ObservableCollection<User> LoadUsersList()
        {
            try
            {
                var usersList = new ObservableCollection<User>();

                using var connection = _connectionFactory.GetConnection();
                {
                    connection.Open();
                    using var command = new NpgsqlCommand(
                        "SELECT id, user_name, badge_number, permissions FROM users",
                        connection
                    );
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader.GetString(0);
                        var userName = reader.GetString(1);
                        var badgeNumber = reader.GetString(2);
                        var permissionsArray = reader.GetFieldValue<string[]>(3);

                        var permissions = permissionsArray.ToList();

                        usersList.Add(new User(id, userName, badgeNumber, permissions));
                    }
                }

                return usersList;
            }
            catch (PostgresException e)
            {
                throw new Exception("Erro ao carregar a lista de usuários." + e);
            }
        }

        //<summary>
        // Get a user by id from the database
        //</summary>
        public async Task<User?> GetUserById(string id)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var selectUser = new NpgsqlCommand(
                    "SELECT id, user_name, badge_number, permissions FROM users WHERE id = @id;",
                    connection
                );
                selectUser.Parameters.AddWithValue("@id", id);

                using var reader = await selectUser.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    reader.Read();

                    string userId = reader.GetString(0);
                    string username = reader.GetString(1);
                    string badgeNumber = reader.GetString(2);
                    string[] permissionsArray = reader.GetFieldValue<string[]>(3);

                    List<string> permissions = [.. permissionsArray];

                    return new User(id, username, badgeNumber, permissions);
                }
                else
                {
                    return null;
                }
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao buscar o usuário." + e);
                throw;
            }
        }

        //<summary>
        // Save a user into the database
        //</summary>
        public async Task<bool> SaveUser(User user, Context context)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                await connection.OpenAsync();

                var insertUser = new NpgsqlCommand(
                    "INSERT INTO users (id, user_name, badge_number, permissions) "
                        + "VALUES (@id, @user_name, @badge_number, @permissions) "
                        + "ON CONFLICT (id) DO UPDATE "
                        + "SET badge_number = user_name = @user_name, @badge_number, permissions = @permissions;",
                    connection
                );
                insertUser.Parameters.AddWithValue("@id", user.Id);
                insertUser.Parameters.AddWithValue("@user_name", user.UserName);
                insertUser.Parameters.AddWithValue("@badge_number", user.BadgeNumber);
                insertUser.Parameters.AddWithValue("@permissions", user.Permissions);

                await insertUser.ExecuteNonQueryAsync();

                return true;
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao cadastrar o usuário." + e);
                throw;
            }
        }

        //<summary>
        // Find a user by badge number
        //</summary>
        public async Task<User?> FindUserByBadgeNumber(string badgeNumber)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                await connection.OpenAsync();

                var selectUser = new NpgsqlCommand(
                    "SELECT id, user_name, permissions FROM users WHERE badge_number = @badge_number;",
                    connection
                );
                selectUser.Parameters.AddWithValue("@badge_number", badgeNumber);

                using var reader = await selectUser.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    reader.Read();

                    string id = reader.GetString(0);
                    string username = reader.GetString(1);
                    string[] permissionsArray = reader.GetFieldValue<string[]>(3);

                    List<string> permissions = [.. permissionsArray];

                    return new User(id, username, badgeNumber, permissions);
                }
                else
                {
                    return null;
                }
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao buscar o usuário." + e);
                throw;
            }
        }

        //<summary>
        // Delete a user from the database
        //</summary>
        public async Task<bool> DeleteUser(User user)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var deleteUser = new NpgsqlCommand("DELETE FROM users WHERE id = @id;", connection);
                deleteUser.Parameters.AddWithValue("@id", user.Id);

                await deleteUser.ExecuteNonQueryAsync();
                return true;
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao deletar o usuário." + e);
                throw;
            }
        }
    }
}
