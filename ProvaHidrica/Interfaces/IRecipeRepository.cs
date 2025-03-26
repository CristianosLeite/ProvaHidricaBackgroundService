using System.Collections.ObjectModel;
using ProvaHidrica.Models;
using ProvaHidrica.Types;

namespace ProvaHidrica.Interfaces
{
    public interface IRecipeRepository
    {
        ObservableCollection<Recipe> LoadRecipeList();
        Task<Recipe?> GetRecipeByVp(string vp);
        Task<bool> SaveRecipe(Recipe recipe, Context context);
        Task<bool> DeleteRecipe(long? id);
    }
}
