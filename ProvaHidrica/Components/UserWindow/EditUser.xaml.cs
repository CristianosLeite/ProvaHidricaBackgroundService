using System.Windows;
using System.Windows.Controls;
using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Types;
using ProvaHidrica.Windows;

namespace ProvaHidrica.Components
{
    /// <summary>
    /// Interação lógica para EditUser.xaml
    /// </summary>
    public partial class EditUser : UserControl
    {
        private readonly Db db;
        private readonly Context Context;
        public string UserId { get; set; }

        public EditUser(User user, Context context)
        {
            InitializeComponent();

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            DataContext = this;
            Context = context;

            UserId = user.Id;

            SetTitle(context);

            TbBadgeNumber.Text = user.BadgeNumber;
            TbUserName.Text = user.UserName;

            CheckPermissions(user.Permissions);
        }

        private void SetTitle(Context context)
        {
            Title.Content = context == Context.Create ? "Cadastrar novo usuário" : "Editar usuário";
        }

        private void CheckPermissions(object permissions)
        {
            if (permissions is IEnumerable<string> permissionList)
            {
                foreach (var permission in permissionList)
                {
                    switch (permission)
                    {
                        case "RU":
                            ViewUsers.IsChecked = true;
                            break;
                        case "WU":
                            EditUsers.IsChecked = true;
                            break;
                        case "RR":
                            ViewRecipes.IsChecked = true;
                            break;
                        case "WR":
                            EditRecipes.IsChecked = true;
                            break;
                        case "OA":
                            AutoOperation.IsChecked = true;
                            break;
                        case "OM":
                            ManualOperation.IsChecked = true;
                            break;
                        case "RO":
                            ViewOperations.IsChecked = true;
                            break;
                        case "RL":
                            ViewLogs.IsChecked = true;
                            break;
                        case "ER":
                            ExportReports.IsChecked = true;
                            break;
                        case "MS":
                            ManageSettings.IsChecked = true;
                            break;
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                {
                    return;
                }

                List<string> permissions = GetSelectedPermissions();

                User user = new(UserId, TbBadgeNumber.Text, TbUserName.Text, permissions);

                if (Context == Context.Create)
                    HandleCreateUser(user);
                else if (Context == Context.Update)
                    SaveUser(user.Id, permissions, Context);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado. " + ex.Message, "Erro");
            }
        }

        private void HandleCreateUser(User user)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Hide();

            NfcWindow nfc = new(Context.Create, user);
            nfc.WorkDone += (sender, isWorkDone) =>
            {
                if (isWorkDone)
                {
                    ShowSuccessMessage();
                    parentWindow?.Close();
                }
                else
                {
                    parentWindow?.Show();
                }
            };
            nfc.ShowDialog();

            if (!nfc.IsWorkDone)
            {
                parentWindow?.Show();
            }
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show(
                $"Usuário {TbUserName.Text} {(Context == Context.Create ? "cadastrado" : "atualizado")} com sucesso.",
                "Sucesso"
            );
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(TbBadgeNumber.Text) || string.IsNullOrEmpty(TbUserName.Text))
            {
                MessageBox.Show("Preencha todos os campos antes de salvar!", "Atenção");
                return false;
            }

            if (!IsAnyPermissionChecked())
            {
                MessageBox.Show("Necessário atribuir ao menos uma permissão ao usuário");
                return false;
            }

            return true;
        }

        private bool IsAnyPermissionChecked()
        {
            return ViewUsers.IsChecked == true
                || EditUsers.IsChecked == true
                || ViewRecipes.IsChecked == true
                || EditRecipes.IsChecked == true
                || AutoOperation.IsChecked == true
                || ManualOperation.IsChecked == true
                || ViewOperations.IsChecked == true
                || ViewLogs.IsChecked == true
                || ExportReports.IsChecked == true
                || ManageSettings.IsChecked == true;
        }

        private List<string> GetSelectedPermissions()
        {
            List<string> permissions = [];

            if (ViewUsers.IsChecked == true)
                permissions.Add("RU");
            if (EditUsers.IsChecked == true)
                permissions.Add("WU");
            if (ViewRecipes.IsChecked == true)
                permissions.Add("RR");
            if (EditRecipes.IsChecked == true)
                permissions.Add("WR");
            if (AutoOperation.IsChecked == true)
                permissions.Add("OA");
            if (ManualOperation.IsChecked == true)
                permissions.Add("OM");
            if (ViewOperations.IsChecked == true)
                permissions.Add("RO");
            if (ViewLogs.IsChecked == true)
                permissions.Add("RL");
            if (ExportReports.IsChecked == true)
                permissions.Add("ER");
            if (ManageSettings.IsChecked == true)
                permissions.Add("MS");

            return permissions;
        }

        private async void SaveUser(string id, List<string> permissions, Context context)
        {
            User newUser = new(id, TbBadgeNumber.Text, TbUserName.Text, permissions);
            await db.SaveUser(newUser, context);
            ShowSuccessMessage();
        }

        private void Close()
        {
            AppUser user = new();
            var parentWindow = Window.GetWindow(this) as UserWindow;
            parentWindow?.Main?.Children.Clear();
            parentWindow?.Main.Children.Add(user);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
