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
        private readonly BehaviorSubject<bool> _btOnSubject;

        public BluetoothStatusProvider(IBluetoothHub bluetoothHub)
        {
            _bluetoothHub = Ensure.NotNull(bluetoothHub, "bluetoothHub");

            _btOnSubject = new BehaviorSubject<bool>(false);

            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Select(_ => _bluetoothHub.Enabled)
                .DistinctUntilChanged()
                .Subscribe(_btOnSubject);
        }

        public IObservable<bool> IsBluetoothOn
        {
            get { return _btOnSubject.AsObservable(); }
        }

        public Task OpenSettings()
        {
            return _bluetoothHub.OpenSettings();
        }
    }
}