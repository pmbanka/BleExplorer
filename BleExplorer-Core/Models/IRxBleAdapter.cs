using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using JetBrains.Annotations;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace BleExplorer.Core.Models
{
    public interface IRxBleAdapter
    {
        IObservable<bool> IsScanning { get; }

        void StartScanningForDevices();

        void StartScanningForDevices(Guid serviceUuid);

        void StopScanningForDevices();

        IObservable<IList<IDevice>> DiscoveredDevices { get; }

        IObservable<IList<IDevice>> ConnectedDevices { get; }

        void ConnectToDevice(IDevice device);

        void DisconnectDevice(IDevice device);
    }

    public class RxBleAdapter : IRxBleAdapter
    {
        [NotNull] private readonly IAdapter _adapter;
        [NotNull] private readonly BehaviorSubject<bool> _isScanningSubject;
        private readonly IObservable<IList<IDevice>> _discoveredDevices;
        private readonly IObservable<IList<IDevice>> _connectedDevices;

        public RxBleAdapter(IAdapter adapter)
        {
            _adapter = Ensure.NotNull(adapter, "adapter");
            _isScanningSubject = new BehaviorSubject<bool>(false);

            var deviceConnectedStream = Observable.FromEventPattern<DeviceConnectionEventArgs>(
                ev => _adapter.DeviceConnected += ev,
                ev => _adapter.DeviceConnected -= ev);

            var deviceDisconnectedStream = Observable.FromEventPattern<DeviceConnectionEventArgs>(
                ev => _adapter.DeviceDisconnected += ev,
                ev => _adapter.DeviceDisconnected -= ev);

            // TODO implement IDisposable
            Observable.FromEventPattern(
                ev => _adapter.ScanTimeoutElapsed += ev,
                ev => _adapter.ScanTimeoutElapsed -= ev)
                .Subscribe(_ => updateIsScanning(false));

            _connectedDevices = deviceConnectedStream
                .Merge(deviceDisconnectedStream)
                .Select(_ => _adapter.ConnectedDevices)
                .Publish()
                .RefCount();

            var deviceDiscoveredStream = Observable.FromEventPattern<DeviceDiscoveredEventArgs>(
                ev => _adapter.DeviceDiscovered += ev,
                ev => _adapter.DeviceDiscovered -= ev);

            _discoveredDevices = deviceDiscoveredStream
                .Select(_ => _adapter.ConnectedDevices)
                .Publish()
                .RefCount();
        }

        public IObservable<bool> IsScanning
        {
            get { return _isScanningSubject.AsObservable(); }
        }

        public void StartScanningForDevices()
        {
            _adapter.StartScanningForDevices();
            updateIsScanning();
        }

        public void StartScanningForDevices(Guid serviceUuid)
        {
            _adapter.StartScanningForDevices(serviceUuid);
            updateIsScanning();
        }

        public void StopScanningForDevices()
        {
            _adapter.StopScanningForDevices();
            updateIsScanning();
        }

        public IObservable<IList<IDevice>> DiscoveredDevices
        {
            get { return _discoveredDevices; }
        }

        public IObservable<IList<IDevice>> ConnectedDevices
        {
            get { return _connectedDevices; }
        }

        public void ConnectToDevice(IDevice device)
        {
            _adapter.ConnectToDevice(device);
        }

        public void DisconnectDevice(IDevice device)
        {
            _adapter.DisconnectDevice(device);
        }

        private void updateIsScanning(bool? state = null)
        {
            _isScanningSubject.OnNext(state ?? _adapter.IsScanning);
        }
    }
}