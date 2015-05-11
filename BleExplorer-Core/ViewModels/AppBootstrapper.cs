using System.Reactive.Linq;
using Akavache;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Views;
using ReactiveUI;
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

            BlobCache.ApplicationName = "BleExplorer";

            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new FindDevicesView(), typeof(IViewFor<FindDevicesViewModel>));

            var adapter = Locator.Current.GetService<Robotics.Mobile.Core.Bluetooth.LE.IAdapter>();
            var device = Locator.Current.GetService<XLabs.Platform.Device.IDevice>();
            var btStatusProvider = new BluetoothStatusProvider(device.BluetoothHub);
            var btLeAdapter = new BluetoothLeAdapter(adapter);

            Router.Navigate.Execute(new FindDevicesViewModel(btStatusProvider, btLeAdapter, this));
        }

        public Page CreateMainPage()
        {
            return new RoutedViwHostWithUserErrorHandler();
        }
    }
}