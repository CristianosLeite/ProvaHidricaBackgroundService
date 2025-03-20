using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ProvaHidrica.Settings;

namespace ProvaHidrica.Models
{
    public partial class PlcSettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SettingsModel> PlcConfigurations { get; set; }
        public ICommand SaveCommand { get; }

        public PlcSettingsViewModel()
        {
            PlcConfigurations = InitializePlcConfigurations();
            SaveCommand = new RelayCommand(SaveSettings);
        }

        private static ObservableCollection<SettingsModel> InitializePlcConfigurations()
        {
            return
            [
                new SettingsModel { Name = "Ip", Value = SPlc.Default.Ip },
                new SettingsModel { Name = "Rack", Value = SPlc.Default.Rack.ToString() },
                new SettingsModel { Name = "Slot", Value = SPlc.Default.Slot.ToString() },
                new SettingsModel
                {
                    Name = "ReadIsOpen1",
                    Value = SPlcAddresses.Default.ReadIsOpen1,
                },
                new SettingsModel
                {
                    Name = "ReadIsOpen2",
                    Value = SPlcAddresses.Default.ReadIsOpen2,
                },
                new SettingsModel { Name = "WriteOpen1", Value = SPlcAddresses.Default.WriteOpen1 },
                new SettingsModel { Name = "WriteOpen2", Value = SPlcAddresses.Default.WriteOpen2 },
            ];
        }

        private void SaveSettings()
        {
            foreach (var config in PlcConfigurations)
            {
                if (config.Name.StartsWith("Read") || config.Name.StartsWith("Write"))
                {
                    SPlcAddresses.Default[config.Name] = config.Value;
                }
                else if (config.Name == "Rack" || config.Name == "Slot")
                {
                    SPlc.Default[config.Name] = int.Parse(config.Value);
                }
                else
                {
                    SPlc.Default[config.Name] = config.Value;
                }
            }
            SPlc.Default.Save();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
