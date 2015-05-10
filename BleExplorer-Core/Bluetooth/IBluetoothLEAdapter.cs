using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using JetBrains.Annotations;
using Robotics.Mobile.Core.Bluetooth.LE;
using XLabs.Platform.Device;

namespace BleExplorer.Core.Bluetooth
{
    using IDevice = Robotics.Mobile.Core.Bluetooth.LE.IDevice;

    public interface IBluetoothLeAdapter
    {
        IObservable<bool> IsScanning { get; }

        void StartScanningForDevices();

        void StopScanningForDevices();

        IObservable<IList<IDevice>> DiscoveredDevices { get; }
    }

    public sealed class BluetoothLeAdapter : IBluetoothLeAdapter, IDisposable
    {
        [NotNull] private readonly IAdapter _adapter;
        [NotNull] private readonly BehaviorSubject<bool> _isScanningSubject;
        [NotNull] private readonly IObservable<IList<IDevice>> _discoveredDevices;
        [NotNull] private readonly CompositeDisposable _disposables;

        public BluetoothLeAdapter(IAdapter adapter, IObservable<bool> bluetoothOn)
        {
            _adapter = Ensure.NotNull(adapter, "adapter");
            _isScanningSubject = new BehaviorSubject<bool>(false);

            var deviceConnectedStream = Observable.FromEventPattern<DeviceConnectionEventArgs>(
                ev => _adapter.DeviceConnected += ev,
                ev => _adapter.DeviceConnected -= ev)
                .Subscribe(_ => { });

            var deviceDisconnectedStream = Observable.FromEventPattern<DeviceConnectionEventArgs>(
                ev => _adapter.DeviceDisconnected += ev,
                ev => _adapter.DeviceDisconnected -= ev)
                .Subscribe(_ => { });

            var deviceDiscoveredStream = Observable.FromEventPattern<DeviceDiscoveredEventArgs>(
                ev => _adapter.DeviceDiscovered += ev,
                ev => _adapter.DeviceDiscovered -= ev);

            _discoveredDevices = deviceDiscoveredStream
                .Select(_ => _adapter.DiscoveredDevices)
                .Publish()
                .RefCount();

            _disposables = new CompositeDisposable
            {
                Observable.FromEventPattern(
                    ev => _adapter.ScanTimeoutElapsed += ev,
                    ev => _adapter.ScanTimeoutElapsed -= ev)
                    .Subscribe(_ => updateIsScanning(false)),
                _isScanningSubject
            };
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

        public void StopScanningForDevices()
        {
            _adapter.StopScanningForDevices();
            updateIsScanning();
        }

        public IObservable<IList<IDevice>> DiscoveredDevices
        {
            get { return _discoveredDevices; }
        }

        private void updateIsScanning(bool? state = null)
        {
            _isScanningSubject.OnNext(state ?? _adapter.IsScanning);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}