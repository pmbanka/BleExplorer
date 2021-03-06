﻿using System;
using System.Reactive.Linq;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels.Devices
{
    public interface IDevicesViewModel : IRoutableViewModel
    {
        bool IsBluetoothOn { get; }
        ReactiveCommand<IBleDevice> DiscoverDevices { get; }
        IReadOnlyReactiveList<IDeviceTileViewModel> Devices { get; }
    }

    public sealed class DevicesViewModel : ReactiveObject, IDevicesViewModel, ISupportsActivation
    {
        private readonly IBleAdapter _adapter;
        private readonly ReactiveList<IBleDevice> _devices;
        private readonly IReactiveDerivedList<IDeviceTileViewModel> _deviceTiles;
        private readonly ObservableAsPropertyHelper<bool> _isBluetoothOn;
        private readonly IBluetoothStatusProvider _statusProvider;

        public DevicesViewModel(
            IBluetoothStatusProvider statusProvider = null,
            IBleAdapter adapter = null,
            IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            _adapter = Ensure.NotNull(adapter ?? Locator.Current.GetService<IBleAdapter>(), "adapter");
            _statusProvider = Ensure.NotNull(
                statusProvider ?? Locator.Current.GetService<IBluetoothStatusProvider>(), "statusProvider");
            _devices = new ReactiveList<IBleDevice>();
            _deviceTiles = _devices.CreateDerivedCollection(p => new DeviceTileViewModel(p));
            Activator = new ViewModelActivator();

            var bluetoothOn =
                this.WhenAnyObservable(vm => vm._statusProvider.IsBluetoothOn)
                    .ObserveOn(RxApp.MainThreadScheduler);
            bluetoothOn.ToProperty(this, vm => vm.IsBluetoothOn, out _isBluetoothOn);

            DiscoverDevices = ReactiveCommand.CreateAsyncObservable(bluetoothOn, discoverDevicesImpl);
            DiscoverDevices.Subscribe(_devices.Add);
            DiscoverDevices.ThrownExceptions
                .Select(discoverDevicesUserError)
                .SelectMany(UserError.Throw)
                .Where(p => p == RecoveryOptionResult.RetryOperation)
                .InvokeCommand(DiscoverDevices);

            this.WhenActivated(d => d(bluetoothOn
                .Where(p => p == false)
                .Select(bluetoothOffUserError)
                .SelectMany(UserError.Throw)
                .Subscribe(_ => _devices.Clear())));
        }

        public bool IsBluetoothOn
        {
            get { return _isBluetoothOn.Value; }
        }

        public string UrlPathSegment
        {
            get { return "BLE Devices"; }
        }

        public IScreen HostScreen { get; private set; }
        public ReactiveCommand<IBleDevice> DiscoverDevices { get; private set; }

        public IReadOnlyReactiveList<IDeviceTileViewModel> Devices
        {
            get { return _deviceTiles; }
        }

        public ViewModelActivator Activator { get; private set; }

        private UserError bluetoothOffUserError(bool _)
        {
            return new UserError("Bluetooth is not turned on", "Turn on bluetooth",
                new[]
                {
                    RecoveryCommand.Cancel,
                    new RecoveryCommand("Open settings", __ =>
                    {
                        _statusProvider.OpenSettings();
                        return RecoveryOptionResult.CancelOperation;
                    })
                });
        }

        private UserError discoverDevicesUserError(Exception ex)
        {
            return new UserError("Devices could not be discovered", null,
                new[]
                {
                    RecoveryCommand.Cancel,
                    new RecoveryCommand("Retry", _ => RecoveryOptionResult.RetryOperation)
                });
        }

        private IObservable<IBleDevice> discoverDevicesImpl(object _)
        {
            _devices.Clear();
            return _adapter.DiscoverDevices();
        }
    }
}