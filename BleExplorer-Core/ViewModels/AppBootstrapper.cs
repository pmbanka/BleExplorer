using Akavache;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Robotics.Mobile.Core.Bluetooth.LE;
using Splat;
using Xamarin.Forms;

namespace BleExplorer.Core.ViewModels
{
    public class AppBootstrapper : ReactiveObject, IScreen, IEnableLogger
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            BlobCache.ApplicationName = "BleExplorer";

            var adapter = Locator.Current.GetService<IAdapter>();
            var device = Locator.Current.GetService<XLabs.Platform.Device.IDevice>();
            var btStatusProvider = new BluetoothStatusProvider(device.BluetoothHub);
            var btLeAdapter = new BluetoothLeAdapter(adapter, btStatusProvider.IsBluetoothOn);

            Locator.CurrentMutable.Register(() => new FindDevicesView(), typeof(IViewFor<FindDevicesViewModel>));

            Router.Navigate.Execute(new FindDevicesViewModel(btStatusProvider, btLeAdapter, this));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}