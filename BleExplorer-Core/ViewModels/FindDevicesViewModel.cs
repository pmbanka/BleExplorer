using System;
using System.Reactive;
using System.Reactive.Disposables;
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

    public sealed class FindDevicesViewModel : ReactiveObject, IFindDevicesViewModel, ISupportsActivation
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

            var bluetoothOn =
                this.WhenAnyObservable(vm => vm._statusProvider.IsBluetoothOn).ObserveOn(RxApp.MainThreadScheduler);

            bluetoothOn.ToProperty(this, vm => vm.IsBluetoothOn, out _isBluetoothOn);

            this.WhenAnyObservable(vm => vm._adapter.DiscoveredDevices)
                .Select(p => p.Count)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, vm => vm.DetectedDevices, out _detectedDevices);

            Activator = new ViewModelActivator();
            this.WhenActivated(d =>
            {
                d(bluetoothOn
                    .Where(p => p == false)
                    .Select(_ => new UserError(
                        "Bluetooth is not turned on",
                        "Turn on bluetooth",
                        new[]
                        {
                            RecoveryCommand.Cancel,
                            new RecoveryCommand("Open settings", __ =>
                            {
                                _statusProvider.OpenSettings();
                                return RecoveryOptionResult.CancelOperation;
                            }) {IsDefault = true}
                        }))
                    .SelectMany(UserError.Throw)
                    .Subscribe());
            });
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

        public ViewModelActivator Activator { get; private set; }
    }
}