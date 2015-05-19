using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using BleExplorer.Core.Utils;
using XLabs.Platform.Device;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBluetoothStatusProvider
    {
        IObservable<bool> IsBluetoothOn { get; }
        Task OpenSettings();
    }

    public sealed class BluetoothStatusProvider : IBluetoothStatusProvider
    {
        private readonly IBluetoothHub _bluetoothHub;
        private readonly IObservable<bool> _isBluetoothOn;

        public BluetoothStatusProvider(IBluetoothHub bluetoothHub)
        {
            _bluetoothHub = Ensure.NotNull(bluetoothHub, "bluetoothHub");

            _isBluetoothOn = Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Select(_ => _bluetoothHub.Enabled)
                .DistinctUntilChanged()
                .Multicast(new BehaviorSubject<bool>(true))
                .RefCount()
                .DistinctUntilChanged();
        }

        public IObservable<bool> IsBluetoothOn
        {
            get { return _isBluetoothOn; }
        }

        public Task OpenSettings()
        {
            return _bluetoothHub.OpenSettings();
        }
    }
}