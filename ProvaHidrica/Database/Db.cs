using System.Collections.ObjectModel;
using System.Windows;
using Npgsql;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Models;
using ProvaHidrica.Services;
using ProvaHidrica.Types;

namespace ProvaHidrica.Database
{
    public class Db : DbConfig
    {
        protected readonly IDbConnectionFactory _connectionFactory = null!;
        protected readonly IUserRepository _userRepository = null!;
        protected readonly IRecipeRepository _recipeRepository = null!;
        protected readonly IOperationRepository _operationRepository = null!;
        protected readonly ILogRepository _logRepository = null!;

        public Db(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;

            try
            {
                _userRepository = new UserRepository(connectionFactory);
                _recipeRepository = new RecipeRepository(connectionFactory);
                _operationRepository = new OperationRepository(connectionFactory);
                _logRepository = new LogRepository(connectionFactory);

                CreateSysLogTable();
                CreateUserLogTable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static NpgsqlConnection GetConnection()
        {
            try
            {
                var connectionString = ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Database connection string is not configured."
                    );
                }

                return new NpgsqlConnection(connectionString);
            }
            catch (ArgumentNullException ex)
            {
                ShowErrorMessage("Erro ao obter a string de conexão: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
            catch (InvalidOperationException ex)
            {
                ShowErrorMessage("Erro de configuração: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Erro ao criar a conexão com o banco de dados: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
        }

        // <summary>
        // Show an error message
        // </summary>
        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // <summary>
        // Create SysLogs table
        // </summary>
        private static void CreateSysLogTable()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();

                var createTableCommand = new NpgsqlCommand(
                    "CREATE TABLE IF NOT EXISTS public.SysLogs ("
                        + "id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ("
                        + "INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1), "
                        + "CreatedAt timestamp without time zone NOT NULL, "
                        + "Event character varying COLLATE pg_catalog.\"default\" NOT NULL, "
                        + "Target character varying COLLATE pg_catalog.\"default\" NOT NULL, "
                        + "Device character varying COLLATE pg_catalog.\"default\", "
                        + "CONSTRAINT SysLogs_pkey PRIMARY KEY (id)) "
                        + "TABLESPACE pg_default; "
                        + "ALTER TABLE IF EXISTS public.SysLogs OWNER to postgres;",
                    connection
                );

                createTableCommand.ExecuteNonQuery();
            }
            catch (PostgresException e)
            {
                ShowErrorMessage("Não foi possível criar a tabela de logs do sistema." + e);
            }
        }

        // <summary>
        // Create SysLogs table
        // </summary>
        private static void CreateUserLogTable()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();

                var createTableCommand = new NpgsqlCommand(
                    "CREATE TABLE IF NOT EXISTS public.UserLogs ("
                        + "id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ("
                        + "INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1), "
                        + "CreatedAt timestamp without time zone NOT NULL, "
                        + "Event character varying COLLATE pg_catalog.\"default\" NOT NULL, "
                        + "Target character varying COLLATE pg_catalog.\"default\", "
                        + "UserId character varying COLLATE pg_catalog.\"default\" NOT NULL, "
                        + "CONSTRAINT UserLogs_pkey PRIMARY KEY (id)) "
                        + "TABLESPACE pg_default; "
                        + "ALTER TABLE IF EXISTS public.UserLogs OWNER to postgres;",
                    connection
                );

                createTableCommand.ExecuteNonQuery();
            }
            catch (PostgresException e)
            {
                ShowErrorMessage("Não foi possível criar a tabela de logs de usuário." + e);
            }
        }

        public ObservableCollection<User> LoadUsersList()
        {
            return _userRepository.LoadUsersList();
        }

        public Task<User?> FindUserByBadgeNumber(string badgeNumber)
        {
            return _userRepository.FindUserByBadgeNumber(badgeNumber);
        }

        public async Task<User?> GetUserById(string id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<bool> SaveUser(User user, Context context)
        {
            await _logRepository.LogUserEditUser(Auth.LoggedInUser!, user.UserName, context);
            return await _userRepository.SaveUser(user, context);
        }

        public async Task<bool> DeleteUser(User user)
        {
            await _logRepository.LogUserDeleteUser(Auth.LoggedInUser!, user.UserName);
            return await _userRepository.DeleteUser(user);
        }

        public async Task<Recipe?> GetRecipeByVp(string vp)
        {
            return await _recipeRepository.GetRecipeByVp(vp);
        }

        public ObservableCollection<Recipe> LoadRecipeList()
        {
            return _recipeRepository.LoadRecipeList();
        }

        public async Task<bool> SaveRecipe(Recipe recipe, Context context)
        {
            await _logRepository.LogUserEditRecipe(Auth.LoggedInUser!, recipe, context);
            return await _recipeRepository.SaveRecipe(recipe, context);
        }

        public async Task<bool> DeleteRecipe(Recipe recipe)
        {
            await _logRepository.LogUserDeleteRecipe(Auth.LoggedInUser!, recipe);
            return await _recipeRepository.DeleteRecipe(recipe.RecipeId);
        }

        public async Task<ObservableCollection<Operation>> LoadOperations()
        {
            return await _operationRepository.LoadOperations();
        }

        public async Task<List<Operation>> GetOperationsByDate(
            string opInfo,
            string initialDate,
            string finalDate
        )
        {
            return await _operationRepository.GetOperationsByDate(opInfo, initialDate, finalDate);
        }

        public async Task<List<Log>> LoadLogs()
        {
            return await _logRepository.LoadLogs();
        }

        public async Task LogUserLogin(User user)
        {
            await _logRepository.LogUserLogin(user);
        }

        public async Task LogUserLogout(User user)
        {
            await _logRepository.LogUserLogout(user);
        }

        public async Task<List<UserLog>> GetUserLogsByDate(string initialDate, string finalDate)
        {
            return await _logRepository.GetUserLogsByDate(initialDate, finalDate);
        }

        public async Task LogSysPlcStatusChanged(string status)
        {
            await _logRepository.LogSysPlcStatusChanged(status);
        }

        public async Task LogSysSwitchedMode(string mode)
        {
            await _logRepository.LogSysSwitchedMode(mode);
        }

        public async Task<List<SysLog>> GetSysLogsByDate(string initialDate, string finalDate)
        {
            return await _logRepository.GetSysLogsByDate(initialDate, finalDate);
        }
    }
}
