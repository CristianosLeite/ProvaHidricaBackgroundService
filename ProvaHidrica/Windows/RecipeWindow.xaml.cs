using System.Windows;
using ProvaHidrica.Components;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para RecipeWindow.xaml
    /// </summary>
    public partial class RecipeWindow : Window
    {
        public RecipeWindow()
        {
            InitializeComponent();
            Topmost = true;
            Main.Children.Add(new AppRecipe());
        }
    }
}
