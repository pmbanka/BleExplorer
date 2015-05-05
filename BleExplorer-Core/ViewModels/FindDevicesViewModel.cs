using System;
using System.Reactive;
using System.Threading.Tasks;
using BleExplorer.Core.Models;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Robotics.Mobile.Core.Bluetooth.LE;
using Splat;

namespace BleExplorer.Core.ViewModels
{
    public interface IFindDevicesViewModel : IRoutableViewModel
    {
        ReactiveCommand<Unit> ScanForDevices { get; }

        bool IsScanning { get; }
    }

    public sealed class FindDevicesViewModel : ReactiveObject, IFindDevicesViewModel
    {
        private readonly IRxBleAdapter _adapter;
        private readonly ObservableAsPropertyHelper<bool> _isScanning;

        public FindDevicesViewModel(IRxBleAdapter adapter = null, IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            _adapter = Ensure.NotNull(adapter ?? Locator.Current.GetService<IRxBleAdapter>(), "adapter");

            _isScanning = this.WhenAnyObservable(vm => vm._adapter.IsScanning)
                .ToProperty(this, vm => vm.IsScanning, false, RxApp.MainThreadScheduler);

            ScanForDevices = ReactiveCommand.CreateAsyncTask(_ =>
            {
                if (IsScanning)
                {
                    _adapter.StopScanningForDevices();
                }
                _adapter.StartScanningForDevices();
                return Task.FromResult(Unit.Default);
            });
        }

        public ReactiveCommand<Unit> ScanForDevices { get; private set; }

        public bool IsScanning
        {
            get { return _isScanning.Value; }
        }

        public string UrlPathSegment
        {
            get { return "Find devices"; }
        }

        public IScreen HostScreen { get; private set; }
    }
}