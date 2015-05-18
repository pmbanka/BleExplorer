using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    public interface IBleAdapter
    {
        IObservable<IBleDevice> DiscoverDevices();
    }

    public sealed class BleAdapter : IBleAdapter, IEnableLogger
    {
        [NotNull] private readonly IAdapter _adapter;

        public BleAdapter(IAdapter adapter)
        {
            _adapter = Ensure.NotNull(adapter, "adapter");
        }

        public IObservable<IBleDevice> DiscoverDevices()
        {
            return Observable.Create<IBleDevice>(obs =>
            {
                var deviceDiscoveredStream = Observable.FromEventPattern<DeviceDiscoveredEventArgs>(
                    ev => _adapter.DeviceDiscovered += ev,
                    ev => _adapter.DeviceDiscovered -= ev);
                var deviceDiscoveredSub = deviceDiscoveredStream
                    .Select(p => p.EventArgs.Device)
                    .Select(dev => new BleDevice(dev))
                    .Subscribe(obs);
                _adapter.StartScanningForDevices();

                var timeoutSub = Observable.FromEventPattern(
                    ev => _adapter.ScanTimeoutElapsed += ev,
                    ev => _adapter.ScanTimeoutElapsed -= ev)
                    .Select(_ => 0L)
                    .Amb(Observable.Timer(TimeSpan.FromSeconds(10)))
                    .Subscribe(_ => obs.OnCompleted());
                return new CompositeDisposable(
                    deviceDiscoveredSub,
                    timeoutSub,
                    Disposable.Create(() => _adapter.StopScanningForDevices()));
            });
        }
    }
}