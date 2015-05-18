using System;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBleDevice
    {
        Guid Id { get; }
        string Name { get; }
        IObservable<IBleService> DiscoverServices();
    }

    public sealed class BleDevice : IBleDevice
    {
        private readonly IDevice _device;
        private readonly string _name;

        public BleDevice(Robotics.Mobile.Core.Bluetooth.LE.IDevice device)
        {
            _device = Ensure.NotNull(device, "device");
            _name = string.IsNullOrWhiteSpace(_device.Name) ? "Unknown" : _device.Name;
            // IObservable<int> Rssi { get; }
            // IObservable<DeviceState> State { get; }
        }

        public IObservable<IBleService> DiscoverServices()
        {
            throw new NotImplementedException();
        }

        public Guid Id
        {
            get { return _device.ID; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}