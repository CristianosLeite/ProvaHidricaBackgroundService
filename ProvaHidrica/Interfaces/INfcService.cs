namespace ProvaHidrica.Interfaces
{
    public interface INfcService
    {
        void InitializeNfcReader();
        void HandleNfcInitializationError();
        string? Acr122u_CardInserted(PCSC.ICardReader reader);
        void Acr122u_CardRemoved();
    }
}
