using System.Windows.Threading;
using ProvaHidrica.Devices.BarcodeReader;

namespace ProvaHidrica.Services
{
    public static class BarcodeReaderService
    {
        private static readonly BarcodeReader _barcodeReader;
        private static readonly DispatcherTimer _timer;
        private static bool IsConnected = false;

        public static event EventHandler<string>? DataReceived;

        static BarcodeReaderService()
        {
            _barcodeReader = new BarcodeReader();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        public static void InitializeCodeBarsReader()
        {
            try
            {
                _barcodeReader.Connect("COM3");
                _timer.Start();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
                throw;
            }
        }

        public static bool IsBarcodeReaderConnected()
        {
            return IsConnected;
        }

        private static void Timer_Tick(object? sender, EventArgs e)
        {
            var data = _barcodeReader.GetData().TrimEnd();
            OnDataReceived(data);
        }

        private static void OnDataReceived(string data)
        {
            if (data.Length > 0)
                DataReceived?.Invoke(null, data);
        }

        public static void SubscribeReader(EventHandler<string> handler)
        {
            DataReceived += handler;
        }

        public static void ClearData()
        {
            _barcodeReader.ClearComPort();
        }
    }
}
