using System;
using System.Reactive;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using BleExplorer.Core.ViewModels.Services;
using BleExplorer.Core.Views.Services;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels.Devices
{
    public interface IDeviceTileViewModel : IReactiveObject
    {
        Guid Id { get; }
        string Name { get; }
        ReactiveCommand<Unit> GoToServicesView { get; }
    }

    public sealed class DeviceTileViewModel : ReactiveObject, IDeviceTileViewModel
    {
        private Guid _id;
        private string _name;

        public DeviceTileViewModel(IBleDevice device, IScreen screen = null)
        {
            Ensure.NotNull(device, "device");
            screen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            Id = device.Id;
            Name = device.Name;
            GoToServicesView = screen.Router.NavigateCommandFor(() => new ServicesViewModel(device, screen));
        }

        public Guid Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            private set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public ReactiveCommand<Unit> GoToServicesView { get; private set; }
    }
}