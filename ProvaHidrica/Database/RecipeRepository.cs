using Npgsql;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Models;
using ProvaHidrica.Types;
using ProvaHidrica.Utils;
using System.Collections.ObjectModel;

namespace ProvaHidrica.Database
{
    internal class RecipeRepository(IDbConnectionFactory connectionFactory) : IRecipeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public ObservableCollection<Recipe> LoadRecipeList()
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var recipeList = new ObservableCollection<Recipe>();

                using var command = new NpgsqlCommand(
                    "SELECT recipe_id, description, sprinkler_height FROM public.recipes;",
                    connection
                );
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Recipe recipe = new(
                        reader.GetInt32(0), // RecipeId
                        reader.GetString(1), // Description
                        reader.GetString(2), // VP
                        reader.GetInt32(3) // SprinklerHeight
                    );

                    Db db = new(_connectionFactory);

                    recipeList.Add(recipe);
                }

                return recipeList;
            }
            catch (PostgresException e)
            {
                throw new Exception("Erro ao carregar a lista de receitas." + e);
            }
        }

        public async Task<Recipe?> GetRecipeByVp(string vp)
        {
            try
            {
                using var conn = _connectionFactory.GetConnection();
                await conn.OpenAsync();

                using var command = new NpgsqlCommand(
                    "SELECT recipe_id, vp, description, sprinkler_height FROM public.recipes WHERE vp = @vp;",
                    conn
                );

                command.Parameters.AddWithValue("@vp", vp);

                using var reader = await command.ExecuteReaderAsync();

                if (!reader.Read())
                    return null;

                var recipe = new Recipe(
                    reader.GetInt64(0), // RecipeId
                    reader.GetString(1), // VP
                    reader.GetString(2), // Description
                    reader.GetInt32(3) // SprinklerHeight
                );

                return recipe;
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao carregar a receita." + e);
                throw;
            }
        }

        public async Task<bool> SaveRecipe(
            Recipe recipe,
            Context context
        )
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                await connection.OpenAsync();

                var command = new NpgsqlCommand(
                    recipe.RecipeId == null
                        ? "INSERT INTO public.recipes (vp, description, sprinkler_height) VALUES (@vp, @description @sprinklerHeight) RETURNING recipe_id;"
                        : "UPDATE public.recipes SET vp = @vp, description = @description, sprinkler_height = @sprinklerHeight WHERE recipe_id = @recipeId;",
                    connection
                );

                if (recipe.RecipeId != null)
                    command.Parameters.AddWithValue("@recipeId", recipe.RecipeId);

                command.Parameters.AddWithValue("@vp", recipe.Vp);
                command.Parameters.AddWithValue("@description", recipe.Description);

                long recipeId;
                if (recipe.RecipeId == null)
#pragma warning disable CS8605 // If the value is null, an exception will be thrown
                    recipeId = (long)await command.ExecuteScalarAsync();
#pragma warning restore CS8605
                else
                {
                    await command.ExecuteNonQueryAsync();
                    recipeId = recipe.RecipeId.Value;
                }

                return true;
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Erro ao salvar a receita." + e);
                return false;
                throw;
            }
        }

        public async Task<bool> DeleteRecipe(string vp)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var deleteCommand = new NpgsqlCommand(
                    "DELETE FROM public.recipes WHERE vp = @vp;",
                    connection
                );
                deleteCommand.Parameters.AddWithValue("@vp", vp);
                await deleteCommand.ExecuteNonQueryAsync();

                return true;
            }
            catch (PostgresException e)
            {
                ErrorMessage.Show("Não foi possível deletar a receita." + e);
                return false;
                throw;
            }
        }
    }
}
