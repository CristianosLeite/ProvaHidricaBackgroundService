using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProvaHidrica.Database;
using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Services;
using ProvaHidrica.Utils;

namespace ProvaHidrica.Components
{
    public partial class MainApplication : UserControl
    {
        private readonly Plc _plc;
        private readonly LogService _logService;
        private readonly ModeService _modeService;
        private readonly PlcService _plcService;
        private readonly DoorService _doorService;

        public MainApplication()
        {
            InitializeComponent();

            Toolbar.Children.Add(new Toolbar());

            DbConnectionFactory connectionFactory = new();
            _plc = new Plc();
            _logService = new LogService(connectionFactory);
            _modeService = new ModeService(this, _logService);
            _plcService = new PlcService(_plc);
            _doorService = new DoorService(this, _modeService, _plc, _plcService);

            SetDoors();

            if (!BarcodeReaderService.IsBarcodeReaderConnected())
                SetMode(); // Manual mode will be set

            InitializePlc();
        }

        private void SetDoors()
        {
            _doorService.SetDoors([door1, door2]);
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

        private async void VPChanged(object sender, RoutedEventArgs e)
        {
            string vp = VPInput.Text;

            if (IsValidVp(vp))
                await Api.SendMessageAsync("barcodeData", vp);
        }

        private async void CISChanged(object sender, RoutedEventArgs e)
        {
            string cis = CISInput.Text;

            if (IsValidCis(cis))
                await Api.SendMessageAsync("barcodeData", cis);
        }

        private async void ChassisChanged(object sender, RoutedEventArgs e)
        {
            string chassis = ChassisInput.Text;

            if (IsValidChassis(chassis))
                await Api.SendMessageAsync("barcodeData", chassis);
        }

        private static bool IsValidVp(string vp) => vp.Length == 14;

        private static bool IsValidCis(string cis) => cis.Length == 8;

        private static bool IsValidChassis(string chassis) => chassis.Length == 17;

        private async Task<bool> ConnectPlc() => await _plcService.Connect();

        private void SubscribeDoors() => _doorService.SubscribeDoors();

        private void UnsubscribeAll(object sender, RoutedEventArgs e) =>
            _doorService.UnsubscribeAll(sender, e);

        private async Task<bool> IsPlcConnected() => await _plc.GetPlcStatus();

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
