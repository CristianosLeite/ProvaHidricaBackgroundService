using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Types;

namespace ProvaHidrica.Services
{
    public class PlcService(Plc plc) : IPlcService
    {
        private readonly Plc _plc = plc;

        public async Task<bool> Connect() => await _plc.Connect();

        public async Task<bool> EnsureConnection() => await _plc.GetPlcStatus();

        public async Task WriteToPlc(int door, string? target, string? van, Event @event)
        {

        }
    }
}
