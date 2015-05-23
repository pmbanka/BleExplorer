using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBleDevice
    {
        Guid Id { get; }
        string Name { get; }
        IObservable<int> Rssi { get; }
        IObservable<BleDeviceState> State { get; }
        IObservable<IBleService> DiscoverServices();
    }

    public enum BleDeviceState
    {
        Disconnected,
        Connecting,
        Connected
    }

    public sealed class BleDevice : IBleDevice
    {
        private readonly IAdapter _adapter;
        private readonly IDevice _device;
        private readonly string _name;
        private readonly IObservable<int> _rssi;
        private readonly IObservable<BleDeviceState> _state;

        public BleDevice(IDevice device, IAdapter adapter)
        {
            _adapter = adapter;
            _device = Ensure.NotNull(device, "device");
            _name = string.IsNullOrWhiteSpace(_device.Name) ? "Unknown" : _device.Name;
            _rssi = Observable
                .Return(_device.Rssi)
                .Concat(Observable.Never<int>())
                .Multicast(new BehaviorSubject<int>(0))
                .RefCount();

            _state = Observable
                .Interval(TimeSpan.FromMilliseconds(200))
                .Select(_ => _device.State)
                .Select(toBleDeviceState)
                .Multicast(new BehaviorSubject<BleDeviceState>(BleDeviceState.Disconnected))
                .RefCount();
        }

        public IObservable<IBleService> DiscoverServices()
        {
            return Observable.Create<IBleService>(async obs =>
            {
                await connectAsync();
                var servicesDiscoveredStream = Observable.FromEventPattern(
                    ev => _device.ServicesDiscovered += ev,
                    ev => _device.ServicesDiscovered -= ev);

                var sub = servicesDiscoveredStream
                    .Subscribe(_ =>
                    {
                        foreach (var service in _device.Services)
                        {
                            obs.OnNext(new BleService(service));
                        }
                        obs.OnCompleted();
                    });
                _device.DiscoverServices();
                return sub;
            });
        }

        public IObservable<BleDeviceState> State
        {
            get { return _state; }
        }

        public Guid Id
        {
            get { return _device.ID; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IObservable<int> Rssi
        {
            get { return _rssi; }
        }

        private Task connectAsync()
        {
            if (_device.State == DeviceState.Connected)
            {
                return Task.FromResult(0);
            }
            var connectedDevicesStream = Observable.FromEventPattern<DeviceConnectionEventArgs>(
                ev => _adapter.DeviceConnected += ev,
                ev => _adapter.DeviceConnected -= ev);
            var ret = connectedDevicesStream
                .Where(p => p.EventArgs.Device.ID == _device.ID)
                .ToTask();
            _adapter.ConnectToDevice(_device);
            return ret;
        }

        private BleDeviceState toBleDeviceState(DeviceState state)
        {
            switch (state)
            {
                case DeviceState.Disconnected:
                    return BleDeviceState.Disconnected;
                case DeviceState.Connecting:
                    return BleDeviceState.Connecting;
                case DeviceState.Connected:
                    return BleDeviceState.Connected;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
            }
        }
    }
}