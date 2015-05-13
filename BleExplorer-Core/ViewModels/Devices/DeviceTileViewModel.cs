using BleExplorer.Core.Bluetooth;

namespace BleExplorer.Core.ViewModels.Devices
{
    public interface IDeviceTileViewModel
    {
    }

    public sealed class DeviceTileViewModel : IDeviceTileViewModel
    {
        public DeviceTileViewModel(IBleDevice device)
        {
        }
    }
}