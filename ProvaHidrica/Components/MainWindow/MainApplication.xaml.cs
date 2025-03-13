using ProvaHidrica.Database;
using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Services;
using ProvaHidrica.Types;
using ProvaHidrica.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProvaHidrica.Components
{
    public partial class MainApplication : UserControl
    {
        private readonly Db _db;
        private readonly Plc _plc;
        private readonly LogService _logService;
        private readonly ModeService _modeService;
        private readonly PlcService _plcService;
        private readonly DoorService _doorService;
        private readonly BarcodeReaderService _codeBarsReaderService;
        private bool _isCisReading = false;

        public MainApplication()
        {
            InitializeComponent();

            DbConnectionFactory connectionFactory = new();

            _db = new(connectionFactory);
            _plc = new Plc();
            _logService = new LogService(connectionFactory);
            _modeService = new ModeService(this, _logService);
            _plcService = new PlcService(_plc);
            _doorService = new DoorService(this, _modeService, _plc, _plcService);
            _codeBarsReaderService = new BarcodeReaderService();

            //_codeBarsReaderService.SubscribeReader(OnDataReceived);

            SetDoors();

            try
            {
                InitializeCodeBarsReader();
            }
            catch
            {
                // Do not throw on constructor
            }

            if (!IsCodeBarsReaderConnected())
                SetMode(); // Manual mode will be set

            InitializePlc();
        }

        private void SetDoors()
        {
            _doorService.SetDoors(
                [
                    door1,
                    door2
                ]
            );
        }

        public void InitializeCodeBarsReader()
        {
            try
            {
                _codeBarsReaderService.InitializeCodeBarsReader();
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

        private void InitializePlc()
        {
            Task.Run(async () =>
                {
                    if (!await ConnectPlc())
                        ErrorMessage.Show("Erro ao conectar com o PLC.");
                })
                .ContinueWith(async t =>
                {
                    if (t.IsCompleted)
                        if (await IsPlcConnected())
                            SubscribeDoors();
                });
        }

        //private void OnDataReceived(object? sender, string data)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        if (data.Length == 14)
        //        {
        //            VPInput.Text = data;
        //            _isCisReading = true;
        //        }
        //        else if (data.Length > 5 && data.Length < 11)
        //        {
        //            if (_isCisReading)
        //            {
        //                CISInput.Text = data;
        //                _isCisReading = false;
        //            }
        //            else
        //            {
        //                // Add 0 until reach 10 characters
        //                data = data.PadLeft(10, '0');
        //                PartnumberInput.Text = data;
        //            }
        //        }
        //        else
        //        {
        //            data = "";
        //        }
        //    });
        //}

        //private void SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    string vp = VPInput.Text;
        //    string cis = CISInput.Text;

        //    if (PartnumberInput.Text is string partnumber && partnumber.Length == 10)
        //        PartnumberChanged(partnumber);
        //    else if (vp.Length == 14 || cis.Length == 7)
        //        VPOrCisChanged(vp, cis);
        //}

        //private async void PartnumberChanged(string partnumber)
        //{
        //    PartnumberInput.Text = partnumber;

        //    bool isPlcConnected = await EnsurePlcConnection();

        //    if (isPlcConnected)
        //    {
        //        if (partnumber.Length < 10)
        //        {
        //            StatusInput.Text = "Leitura inválida.";
        //            ClearTextInput();
        //            return;
        //        }
        //        try
        //        {
        //            int door = await _db.GetAssociatedDoor(partnumber);
        //            if (door == 0)
        //            {
        //                StatusInput.Text = "Desenho não encontrado.";
        //                ClearTextInput();
        //                return;
        //            }

        //            _ = door switch
        //            {
        //                1 => door = 10,
        //                2 => door = 11,
        //                3 => door = 12,
        //                4 => door = 13,
        //                5 => door = 14,
        //                6 => door = 15,
        //                7 => door = 16,
        //                8 => door = 17,
        //                9 => door = 18,
        //                _ => door = 0,
        //            };

        //            await WriteToPlc(door, partnumber, string.Empty, Event.Reading);
        //        }
        //        catch (Exception e)
        //        {
        //            ErrorMessage.Show("Erro ao interagir com o PLC. " + e);
        //        }
        //    }
        //}

        private async void VPOrCisChanged(string vp, string cis)
        {
            if (string.IsNullOrEmpty(vp))
            {
                CISInput.Text = cis;
                StatusInput.Text = "Aguardando leitura do VP.";
                return;
            }

            if (string.IsNullOrEmpty(cis))
            {
                VPInput.Text = vp;
                StatusInput.Text = "Aguardando leitura do código CIS.";
                return;
            }

            if (!IsValidVp(vp) || !IsValidCis(cis))
            {
                StatusInput.Text = "Leitura inválida.";
                ClearTextInput();
                return;
            }

            if (await AreDoorsOpen())
            {
                StatusInput.Text = "Aguardando fechamento das portas.";
                return;
            }

            var recipe = await _db.GetRecipeByVp(vp);
            if (recipe == null)
            {
                StatusInput.Text = "Receita não cadastrada.";
                ClearTextInput();
                return;
            }

            //await OpenDoorsForRecipe(vp, cis);

            RecipeInput.Text = recipe.Description;
            StatusInput.Text = "Receita carregada com sucesso!";

            ClearTextInput();
        }

        private static bool IsValidVp(string vp) => vp.Length == 14;

        private static bool IsValidCis(string cis) => cis.Length == 7;

        private async Task<bool> AreDoorsOpen()
        {
            List<int> doors = Enumerable.Range(1, 9).ToList();
            return await CheckForOpenDoors(doors);
        }

        //private async Task OpenDoorsForRecipe(string vp, string cis)
        //{
        //    var doorsToOpen = await _db.GetRecipeAssociatedDoors(vp);
        //    foreach (var door in doorsToOpen)
        //    {
        //        if (_doorService.Subscriptions.Count == 0)
        //            SubscribeDoors();
        //        await WriteToPlc(door, vp, cis, Event.Reading);
        //        Thread.Sleep(100);
        //    }
        //}

        private void ClearTextInput()
        {
            VPInput.Text = string.Empty;
            CISInput.Text = string.Empty;
            //PartnumberInput.Text = string.Empty;
            _codeBarsReaderService.ClearData();
        }

        private bool IsCodeBarsReaderConnected() =>
            _codeBarsReaderService.IsCodeBarsReaderConnected();

        private async Task<bool> ConnectPlc() => await _plcService.Connect();

        private void SubscribeDoors() => _doorService.SubscribeDoors();

        private void UnsubscribeAll(object sender, RoutedEventArgs e) =>
            _doorService.UnsubscribeAll(sender, e);

        private async Task<bool> EnsurePlcConnection() => await _plcService.EnsureConnection();

        private async Task<bool> IsPlcConnected() => await _plc.GetPlcStatus();

        private async Task WriteToPlc(int door, string target, string cis, Event @event)
        {
            if (_doorService.Subscriptions.Count == 0)
                _doorService.SubscribeDoors();

            if (!Auth.UserHasPermission("O"))
            {
                ErrorMessage.Show("Usuário não tem permissão para abrir portas.");
                return;
            }
            await _plcService.WriteToPlc(door, target, cis, @event);

            // Await for 3 seconds and reset the values
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                Dispatcher.Invoke(() =>
                {
                    //PartnumberInput.Text = string.Empty;
                    VPInput.Text = string.Empty;
                    CISInput.Text = string.Empty;
                    //PartnumberInput.Text = string.Empty;
                    RecipeInput.Text = "Nenhuma receita selecionada";
                    StatusInput.Text = "Aguardando leitura...";
                });
            });
        }

        private void ManualDoorOpen(object sender, RoutedEventArgs e) =>
            _doorService.ManualDoorOpen(sender, e);

        private async Task<bool> CheckForOpenDoors(List<int> doors) =>
            await _doorService.CheckForOpenDoors(doors);

        private void SetMode() => _modeService.SetMode();

        private void SwitchMode(object sender, RoutedEventArgs e) =>
            _modeService.SwitchMode(sender, e);

        private void ShowLogs(object sender, RoutedEventArgs e) => _logService.ShowLogs(sender, e);

        private void ModeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            _modeService.ModeButton_MouseLeftButtonDown(sender, e);

        private void ModeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) =>
            _modeService.ModeButton_MouseLeftButtonUp(sender, e);
    }
}
