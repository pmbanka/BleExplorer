using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using JetBrains.Annotations;
using ReactiveUI;
using Robotics.Mobile.Core.Bluetooth.LE;
using Splat;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBluetoothLeAdapter
    {
        IObservable<IList<IDevice>> DiscoveredDevices { get; }
    }

    public sealed class BluetoothLeAdapter : IBluetoothLeAdapter, IEnableLogger
    {
        [NotNull] private readonly IAdapter _adapter;
        [NotNull] private readonly IObservable<IList<IDevice>> _discoveredDevices;

        public BluetoothLeAdapter(IAdapter adapter)
        {
            _adapter = Ensure.NotNull(adapter, "adapter");

            var deviceDiscoveredStream = Observable.FromEventPattern<DeviceDiscoveredEventArgs>(
                ev => _adapter.DeviceDiscovered += ev,
                ev => _adapter.DeviceDiscovered -= ev);

            _discoveredDevices = Observable.Create<List<IDevice>>(obs =>
            {
                var devices = new List<DeviceUpdateInfo>();
                var tokenSource = new CancellationTokenSource();
                var subscription = deviceDiscoveredStream
                    .Select(p => p.EventArgs.Device)
                    .Subscribe(dev =>
                    {
                        var existingDevice = devices.SingleOrDefault(p => p.Device.ID == dev.ID);
                        if (existingDevice == null)
                        {
                            devices.Add(new DeviceUpdateInfo(dev));
                            obs.OnNext(devices.Select(p => p.Device).ToList());
                        }
                        else
                        {
                            existingDevice.IsDetected = true;
                        }
                    });
                // ReSharper disable once UnusedVariable - task variable exists to remove unnecessary warning 
                var task = Task.Run(async () =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        foreach (var dev in devices) dev.IsDetected = false;
                        _adapter.StartScanningForDevices();
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        _adapter.StopScanningForDevices();
                        devices.RemoveAll(p => !p.IsDetected);
                        obs.OnNext(devices.Select(p => p.Device).ToList());
                    }
                }, tokenSource.Token);
                return () =>
                {
                    tokenSource.Cancel();
                    subscription.Dispose();
                };
            })
                .LoggedCatch(this, (Exception _) => Observable.Empty<List<IDevice>>().Delay(TimeSpan.FromSeconds(0.5)))
                .Repeat()
                .Publish()
                .RefCount();
        }

        public IObservable<IList<IDevice>> DiscoveredDevices
        {
            get { return _discoveredDevices; }
        }

        private class DeviceUpdateInfo
        {
            private readonly IDevice _device;

            public DeviceUpdateInfo(IDevice device)
            {
                _device = device;
                IsDetected = true;
            }

            public bool IsDetected { get; set; }

            public IDevice Device
            {
                get { return _device; }
            }
        }
    }
}