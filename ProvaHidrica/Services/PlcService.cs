using System.Diagnostics;
using ProvaHidrica.Devices.Plc;
using ProvaHidrica.Interfaces;
using ProvaHidrica.Settings;
using ProvaHidrica.Types;

namespace ProvaHidrica.Services
{
    public class PlcService(Plc plc) : IPlcService
    {
        private readonly Plc _plc = plc;

        public async Task<bool> Connect() => await _plc.Connect();

        public async Task<bool> EnsureConnection() => await _plc.GetPlcStatus();

        public async Task WriteToPlc(int door, string? target, string? cis, Event @event)
        {
            switch (door)
            {
                case 1:
                    await _plc.WriteToPlc(SPlcAddresses.Default.WriteOpen1, "true");
                    break;
                case 2:
                    await _plc.WriteToPlc(SPlcAddresses.Default.WriteOpen2, "true");
                    break;
                default:
                    Debug.WriteLine("Door not found.");
                    break;
            }
        }
    }
}
