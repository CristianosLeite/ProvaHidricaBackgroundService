using ProvaHidrica.Components;
using ProvaHidrica.Database;
using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Models;
using ProvaHidrica.Services;
using ProvaHidrica.Types;
using ProvaHidrica.Utils;
using ProvaHidrica.Windows;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ProvaHidrica
{
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon _notifyIcon;
        private bool _isExit;
        private readonly Db _db;
        private readonly NfcService _nfcService;
        private readonly Plc _plc;
        private readonly PlcService _plcService;
        private readonly NfcService _nfcSerivce;

        public MainWindow(Db db, NfcService nfcService, Plc plc, PlcService plcService)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("./favicon.ico"),
                Visible = true,
                Text = "Prova Hidrica - Conecsa",
            };

            InitializeComponent();
            InitializeNotifyIcon();

            _nfcSerivce = new NfcService();

            BarcodeReaderService.SubscribeReader(OnDataReceived);

            _db = db;
            _nfcService = nfcService;
            _plc = plc;
            _plcService = plcService;

            try
            {
                InitializeCodeBarsReader();
            }
            catch
            {
                // Do not throw on constructor
            }

            if (!IsBarcodeReaderConnected())
            {
                Task.Run(async () =>
                {
                    await Api.SendMessageAsync("erro", new());
                });
            }

            InitializeNfcReader();

            _nfcSerivce.CardInserted += OnCardInserted;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Header.Children.Add(new Header());
            Main.Children.Add(new MainApplication());
            //Toolbar.Children.Add(new Toolbar());
            Footer.Children.Add(new Footer());
        }

        private void InitializeNfcReader()
        {
            Dispatcher.Invoke(
                () =>
                    Task.Run(() =>
                    {
                        try
                        {
                            _nfcService.InitializeNfcReader();
                        }
                        catch
                        {
                            ErrorMessage.Show(
                                "Erro ao conectar o leitor NFC, verifique a conexão e tente novamente."
                            );
                        }
                    })
            );
        }

        private void OnCardInserted(object? sender, string uid)
        {
            Dispatcher.Invoke(async () =>
            {
                User? user = await _db.GetUserById(uid);

                if (user != null)
                {
                    HandleExistingUser(user);
                    await _plcService.WriteToPlc(1, "", "", Event.Reading);
                }
                else
                {
                    // HandleNewUser(uid);
                }
            });
        }

        private void HandleExistingUser(User user)
        {
            if (IsUserAuthenticated())
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    Auth.SetLoggedInUser(user);
                    Auth.SetLoggedAt(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    await Api.SendMessageAsync("authenticated", user);
                    ShowMainWindow();
                });
            }
            else
            {
                LoadNfcWindow(Context.Login);
            }
        }

        private static void LoadNfcWindow(Context context = Context.Open)
        {
            NfcWindow? nfcWindow = null;

            Thread staThread =
                new(() =>
                {
                    nfcWindow = new NfcWindow(context);
                    nfcWindow.Main.Children.Add(new NfcStd(context));
                    nfcWindow.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
                    if (context == Context.Login)
                        nfcWindow.ShowDialog();
                });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon.DoubleClick += (s, e) =>
            {
                if (IsUserAuthenticated())
                {
                    ShowMainWindow();
                }
                else
                {
                    LoadNfcWindow(Context.Login);
                }
            };
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(
                "Login",
                null,
                (s, e) =>
                {
                    LoadNfcWindow(Context.Login);
                }
            );
            contextMenu.Items.Add("Sair", null, (s, e) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private static void InitializeCodeBarsReader()
        {
            try
            {
                BarcodeReaderService.InitializeCodeBarsReader();
            }
            catch (Exception e)
            {
                ErrorMessage.Show(
                    "Erro ao conectar o leitor de código de barras. Operação será executada em modo manual.\n"
                        + e
                );
                throw; // Throw exception on ModeService
            }
        }

        private static bool IsBarcodeReaderConnected() =>
            BarcodeReaderService.IsBarcodeReaderConnected();

        private void ShowMainWindow()
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private static bool IsUserAuthenticated()
        {
            return Auth.GetUserId() != "0";
        }

        private void OnDataReceived(object? sender, string data)
        {
            Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (data.Length == 14)
                    {
                        Recipe? recipe = await _db.GetRecipeByVp(data);

                        if (recipe == null)
                        {
                            object? error = new { message = "Receita não encontrada" };
                            await Api.SendMessageAsync("error", error);
                            return;
                        }

                        await Api.SendMessageAsync("barcodeData", recipe);
                    }
                    else
                    {
                        await Api.SendMessageAsync("barcodeData", data);
                    }
                }
                catch (Exception e)
                {
                    ErrorMessage.Show("Erro ao carregar a receita." + e);
                    return;
                }
            });
        }

        private void ExitApplication()
        {
            _isExit = true;
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                Hide();
            }
            base.OnClosing(e);
        }
    }
}
