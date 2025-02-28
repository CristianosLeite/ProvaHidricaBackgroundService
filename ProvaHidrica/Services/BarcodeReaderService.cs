using ProvaHidrica.Devices.CodebarsReader;
using ProvaHidrica.Interfaces;
using System.Windows.Threading;

namespace ProvaHidrica.Services
{
    public class BarcodeReaderService : IBarcodeReaderService
    {
        private readonly BarcodeReader _codebarsReader;
        private readonly DispatcherTimer _timer;
        public bool IsConnected = false;

        public event EventHandler<string>? DataReceived;

        public BarcodeReaderService()
        {
            _codebarsReader = new BarcodeReader();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        public void InitializeCodeBarsReader()
        {
            try
            {
                _codebarsReader.Connect("COM3");
                _timer.Start();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
                throw;
            }
        }

        public bool IsCodeBarsReaderConnected()
        {
            return IsConnected;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var data = _codebarsReader.GetData().TrimEnd();
            OnDataReceived(data);
        }

        protected virtual void OnDataReceived(string data)
        {
            if (data.Length > 0)
                DataReceived?.Invoke(this, data);
        }

        public void SubscribeReader(EventHandler<string> handler)
        {
            DataReceived += handler;
        }

        public void ClearData()
        {
            _codebarsReader.ClearComPort();
        }
    }
}
