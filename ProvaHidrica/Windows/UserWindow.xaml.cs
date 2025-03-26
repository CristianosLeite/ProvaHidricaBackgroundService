using ProvaHidrica.Components;
using System.Windows;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            Topmost = true;
            Main.Children.Add(new AppUser());
        }
    }
}
