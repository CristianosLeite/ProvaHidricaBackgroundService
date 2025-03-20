using System.Diagnostics;
using ProvaHidrica.Devices.Nfc;
using ProvaHidrica.Interfaces;

namespace ProvaHidrica.Services
{
    public class NfcService : INfcService
    {
        public event EventHandler<string>? CardInserted;

        public string? Acr122u_CardInserted(PCSC.ICardReader reader)
        {
            var uid = ACR122U.GetUID(reader);
            if (uid != null)
            {
                string uidHex = Convert.ToHexString(uid);
                CardInserted?.Invoke(this, uidHex);
                return uidHex;
            }
            return null;
        }

        public void Acr122u_CardRemoved()
        {
            Debug.WriteLine("Card removed");
        }

        public void HandleNfcInitializationError()
        {
            throw new Exception("NFC reader initialization error");
        }

        public void InitializeNfcReader()
        {
            ACR122U acr122u = new();

            try
            {
                acr122u.Init(true);
                acr122u.CardInserted += Acr122u_CardInserted;
                acr122u.CardRemoved += Acr122u_CardRemoved;

                // wait for a signal
                ManualResetEvent waitHandle = new(false);
                waitHandle.WaitOne();

                acr122u.CardInserted -= Acr122u_CardInserted;
                acr122u.CardRemoved -= Acr122u_CardRemoved;
                Thread.Sleep(1000);
            }
            catch
            {
                HandleNfcInitializationError();
            }
        }
    }
}
