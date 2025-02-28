namespace ProvaHidrica.Interfaces
{
    public interface IBarcodeReaderService
    {
        void InitializeCodeBarsReader();
        void SubscribeReader(EventHandler<string> handler);
    }
}
