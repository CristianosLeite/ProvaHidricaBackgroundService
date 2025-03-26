using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Types;
using ProvaHidrica.Windows;

namespace ProvaHidrica.Components
{
    /// <summary>
    /// Interação lógica para EditRecipe.xam
    /// </summary>
    public partial class EditRecipe : UserControl
    {
        private readonly Db _db;
        private readonly AppRecipe _createRecipe;
        private readonly Context context;
        private readonly long? RecipeId = null;
        private int Index;

        public EditRecipe(AppRecipe createRecipe, Recipe recipe, Context context)
        {
            InitializeComponent();
            DataContext = this;
            this.context = context;

            DbConnectionFactory connectionFactory = new();
            _db = new(connectionFactory);

            if (context == Context.Update)
            {
                Title.Content = "Editar receita";
            }
            else
            {
                Title.Content = "Cadastrar nova receita";
            }

            _createRecipe = createRecipe;

            RecipeId = recipe.RecipeId;
            TbVP.Text = recipe.Vp;
            Description.Text = recipe.Description;
            SprinklerHeight.Text = recipe.SprinklerHeight.ToString();
        }

        public void SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            Index = SprinklerHeight.SelectedIndex;
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.Index = SprinklerHeight.SelectedIndex;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (
                    string.IsNullOrEmpty(TbVP.Text)
                    || string.IsNullOrEmpty(Description.Text)
                    || string.IsNullOrEmpty(SprinklerHeight.Text)
                )
                {
                    MessageBox.Show("Preencha todos os campos antes de salvar!", "Atenção");
                    return;
                }

                if (TbVP.Text.Length != 14)
                {
                    MessageBox.Show("O VP deve conter 14 caracteres.", "Atenção");
                    return;
                }

                bool result = await _db.SaveRecipe(
                    new(RecipeId, TbVP.Text, Description.Text, Index + 1),
                    context
                );

                if (!result)
                    return;

                MessageBox.Show(
                    $"Receita {(context == Context.Create ? "cadastrada" : "atualizada")} com sucesso.",
                    "Sucesso"
                );

                _createRecipe.UpdateRecipeList();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inexperado. " + ex.Message, "Erro");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            AppRecipe recipe = new();
            var parentWindow = Window.GetWindow(this) as RecipeWindow;
            parentWindow?.Main?.Children.Clear();
            parentWindow?.Main.Children.Add(recipe);
        }
    }
}
