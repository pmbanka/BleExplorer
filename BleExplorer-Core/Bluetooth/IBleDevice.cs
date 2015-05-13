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
        // IObservable<int> Rssi { get; }
        // IObservable<DeviceState> State { get; }
    }

    public sealed class BleDevice : IBleDevice
    {
        private readonly IDevice _device;

        public BleDevice(Robotics.Mobile.Core.Bluetooth.LE.IDevice device)
        {
            _device = Ensure.NotNull(device, "device");
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
            get { return _device.Name; }
        }
    }
}