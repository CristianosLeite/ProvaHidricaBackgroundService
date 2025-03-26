using System.IO.Ports;

namespace ProvaHidrica.Devices.BarcodeReader
{
    internal class BarcodeReader
    {
        SerialPort port;
        string data;

        public BarcodeReader()
        {
            port = new();
            data = "";
        }

        private static bool CheckPort(string COM)
        {
            string[] ports = SerialPort.GetPortNames();
            return ports.Contains(COM);
        }

        public void Connect(string COM)
        {
            try
            {
                if (!CheckPort(COM))
                {
                    throw new Exception($"Porta {COM} não está disponível.");
                }

                port = new SerialPort(COM);
                port.DataReceived += Port_DataReceived;
                port.Open();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(Exception))
                {
                    throw new Exception($"Erro ao conectar na porta {COM}: {e.Message}");
                }
            }
        }

        public string GetData()
        {
            return data;
        }

        public void ClearComPort()
        {
            if (port.IsOpen)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                data = "";
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data = port.ReadExisting();
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                ClearComPort();
            });
        }

        public void Close()
        {
            port.Close();
        }
    }
}
