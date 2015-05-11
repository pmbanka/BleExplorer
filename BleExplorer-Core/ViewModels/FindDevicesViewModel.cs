using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels
{
    public interface IFindDevicesViewModel : IRoutableViewModel
    {
        int DetectedDevices { get; }
        bool IsBluetoothOn { get; }
    }

    public sealed class FindDevicesViewModel : ReactiveObject, IFindDevicesViewModel
    {
        private readonly IBluetoothLeAdapter _adapter;
        private readonly IBluetoothStatusProvider _statusProvider;

        private readonly ObservableAsPropertyHelper<int> _detectedDevices;
        private readonly ObservableAsPropertyHelper<bool> _isBluetoothOn;

        public FindDevicesViewModel(IBluetoothStatusProvider btStatusProvider = null,
            IBluetoothLeAdapter btLeAdapter = null, IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            _adapter = Ensure.NotNull(btLeAdapter ?? Locator.Current.GetService<IBluetoothLeAdapter>(), "btLeAdapter");
            _statusProvider = Ensure.NotNull(
                btStatusProvider ?? Locator.Current.GetService<IBluetoothStatusProvider>(), "btStatusProvider");

            var btOn =
                this.WhenAnyObservable(vm => vm._statusProvider.IsBluetoothOn).ObserveOn(RxApp.MainThreadScheduler);

            btOn
                .Where(p => p == false)
                .Select(_ => new UserError(
                    "Bluetooth is not turned on",
                    "Turn on bluetooth",
                    new[]
                    {
                        new RecoveryCommand("Open settings", __ =>
                        {
                            _statusProvider.OpenSettings();
                            return RecoveryOptionResult.CancelOperation;
                        }) {IsDefault = true},
                        RecoveryCommand.Cancel
                    }))
                .SelectMany(UserError.Throw)
                .LoggedCatch(this,
                    (Exception _) =>
                        Observable.Return(RecoveryOptionResult.CancelOperation).Delay(TimeSpan.FromSeconds(2)),
                    "Is BLE on stream")
                .Repeat()
                .Subscribe();

            _isBluetoothOn = btOn.ToProperty(this, vm => vm.IsBluetoothOn);

            _detectedDevices = this.WhenAnyObservable(vm => vm._adapter.DiscoveredDevices)
                .Select(p => p.Count)
                .ToProperty(this, vm => vm.DetectedDevices, 0, RxApp.MainThreadScheduler);
        }

        public bool IsBluetoothOn
        {
            get { return _isBluetoothOn.Value; }
        }

        public int DetectedDevices
        {
            get { return _detectedDevices.Value; }
        }

        public string UrlPathSegment
        {
            get { return "Find devices"; }
        }

        public IScreen HostScreen { get; private set; }
    }
}