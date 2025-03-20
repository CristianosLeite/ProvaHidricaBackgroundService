using System.Windows;
using System.Windows.Media.Effects;
using ProvaHidrica.Components;
using ProvaHidrica.Database;
using ProvaHidrica.Models;
using ProvaHidrica.Services;
using ProvaHidrica.Types;
using ProvaHidrica.Utils;

namespace ProvaHidrica.Windows
{
    /// <summary>
    /// Lógica interna para NfcWindow.xaml
    /// </summary>
    public partial class NfcWindow : Window
    {
        private bool _disposed = false;
        private readonly Db db;
        private readonly Context context;
        private readonly NfcService _nfcSerivce;
        private User? User = null;
        public event EventHandler<bool>? WorkDone;
        public bool IsWorkDone { get; private set; }

        public NfcWindow(Context context, User? user = null)
        {
            InitializeComponent();

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            _nfcSerivce = new NfcService();
            _nfcSerivce.CardInserted += OnCardInserted;

            this.context = context;
            User = user;

            Main.Children.Add(new NfcStd(this.context));
            InitializeNfc();

            SetProperties();
        }

        private void SetProperties()
        {
            Topmost = true;
        }

        private void InitializeNfc()
        {
            Dispatcher.Invoke(() => Task.Run(InitializeNfcReader));
        }

        private void InitializeNfcReader()
        {
            try
            {
                _nfcSerivce.InitializeNfcReader();
            }
            catch
            {
                HandleNfcInitializationError();
            }
        }

        private void HandleNfcInitializationError()
        {
            if (context == Context.Login)
            {
                Dispatcher.Invoke(() =>
                {
                    Login login = new();
                    Main.Children.Clear();
                    Main.Children.Add(login);
                    IsWorkDone = true;
                });
                return;
            }
            else if (context == Context.Create)
            {
                Dispatcher.Invoke(async () =>
                {
                    if (User != null)
                    {
                        User.Id = GenerateRandomId();
                        if (await SaveToDatabase(User, Context.Create))
                            Close();
                    }
                });
                return;
            }
            HandleContextError();
        }

        private void ReloadWindow(object sernder, RoutedEventArgs e)
        {
            if (!IsWorkDone)
                Dispatcher.Invoke(() =>
                {
                    App.Current.MainWindow.Effect = new BlurEffect();
                    NfcWindow nfcWindow = new(context, User);
                    nfcWindow.Show();
                });
            App.Current.MainWindow.IsEnabled = true;
            App.Current.MainWindow.Effect = null;
        }

        private async void OnCardInserted(object? sender, string uid)
        {
            User? user = await db.GetUserById(uid);

            if (user != null)
            {
                HandleExistingUser(user);
            }
            else
            {
                HandleNewUser(uid);
            }
        }

        private void HandleExistingUser(User user)
        {
            if (context == Context.Login)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    IsWorkDone = true;
                    Auth.SetLoggedInUser(user);
                    Auth.SetLoggedAt(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    await Api.SendMessageAsync("authenticated", user);
                    await ShowLoadingAndSuccess();
                    Close();
                });
                return;
            }
            else if (context == Context.Create)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    await ShowLoadingAndError("Crachá de identificação já cadastrado");
                    NfcStd nfcStd = new(Context.Create);
                    Main.Children.Clear();
                    Main.Children.Add(nfcStd);
                });
                return;
            }
            else
            {
                HandleContextError();
            }
        }

        private void HandleNewUser(string id)
        {
            if (context == Context.Login)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    await ShowLoadingAndUserNotFound();
                    NfcStd nfcStd = new(Context.Login);
                    Main.Children.Clear();
                    Main.Children.Add(nfcStd);
                });
            }
            else if (context == Context.Create)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    if (User != null)
                    {
                        User.Id = id;

                        IsWorkDone = await SaveToDatabase(User, Context.Create);
                        if (IsWorkDone)
                            await Api.SendMessageAsync("userCreated", User);
                        User = null; // Avoid saving the same user twice
                        Close();
                    }
                });
            }
            else
                HandleContextError();
        }

        private async Task<bool> SaveToDatabase(User user, Context context)
        {
            bool isSaved = await db.SaveUser(user, context);
            if (isSaved)
            {
                IsWorkDone = true;
                await ShowLoadingAndSuccess();
                Dispatcher.Invoke(() => WorkDone?.Invoke(this, true));
                return true;
            }
            else
            {
                IsWorkDone = false;
                await ShowLoadingAndError("Usuário já cadastrado.");
                return false;
            }
        }

        private void HandleContextError()
        {
            ErrorMessage.Show("Contexto inválido. " + context);
        }

        private static string GenerateRandomId()
        {
            Random random = new();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return Convert.ToHexString(buffer);
        }

        private async Task ShowLoadingAndSuccess()
        {
            Main.Children.Clear();
            Main.Children.Add(new NfcLoading());
            await Task.Delay(1000);

            Main.Children.Add(new NfcSuccess());
            await Task.Delay(1000);

            Main.Children.Clear();
        }

        private async Task ShowLoadingAndError(string errorMessage)
        {
            Main.Children.Clear();
            Main.Children.Add(new NfcLoading());
            await Task.Delay(1000);

            Main.Children.Add(new NfcError(errorMessage));
            await Task.Delay(1000);

            Main.Children.Clear();
        }

        private async Task ShowLoadingAndUserNotFound()
        {
            Main.Children.Add(new NfcLoading());
            await Task.Delay(1000);

            Main.Children.Add(new NfcUserNotFound());
            await Task.Delay(1000);

            Main.Children.Clear();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            Dispose();
        }

        private void Dispose()
        {
            if (!_disposed)
            {
                _nfcSerivce.CardInserted -= OnCardInserted;
                Close();
                _disposed = true;
            }
        }
    }
}
