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
    public sealed class FindDevicesViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IRxBleAdapter _adapter;
        private readonly ObservableAsPropertyHelper<bool> _isScanning;

        public FindDevicesViewModel(IRxBleAdapter adapter = null, IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            _adapter = Ensure.NotNull(adapter ?? Locator.Current.GetService<IRxBleAdapter>(), "adapter");


            _isScanning = this.WhenAnyObservable(vm => vm._adapter.IsScanning)
                .ToProperty(this, vm => vm.IsScanning, false, RxApp.MainThreadScheduler);

            ToggleScanningForDevices = ReactiveCommand.CreateAsyncTask(_ =>
            {
                if (!IsScanning)
                {
                    _adapter.StartScanningForDevices();
                }
                else
                {
                    _adapter.StopScanningForDevices();
                }
                return Task.FromResult(Unit.Default);
            });
        }

        public ReactiveCommand<Unit> ToggleScanningForDevices { get; private set; }

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