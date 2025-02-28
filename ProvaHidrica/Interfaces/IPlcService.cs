using ProvaHidrica.Types;

namespace ProvaHidrica.Interfaces
{
    public interface IPlcService
    {
        Task<bool> Connect();
        Task<bool> EnsureConnection();
        Task WriteToPlc(int door, string target, string van, Event @event);
    }
}
