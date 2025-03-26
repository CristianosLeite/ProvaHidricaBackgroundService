namespace ProvaHidrica.Models
{
    public class Recipe(long? recipeId, string vp, string cis, string description, int sprinklerHeight)
    {
        public long? RecipeId { get; set; } = recipeId;
        public string? Vp { get; set; } = vp;
        public string? Cis { get; set; } = cis;
        public string Description { get; set; } = description;
        public int SprinklerHeight { get; set; } = sprinklerHeight;
    }
}
