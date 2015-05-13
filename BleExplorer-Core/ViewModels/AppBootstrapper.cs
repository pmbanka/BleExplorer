using System.Reactive.Linq;
using Akavache;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using BleExplorer.Core.ViewModels.Devices;
using BleExplorer.Core.Views;
using BleExplorer.Core.Views.Devices;
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
            this.Log().Debug("AppBootstrapper created.");

            Router = new RoutingState();

            BlobCache.ApplicationName = "BleExplorer";

            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new DevicesView(), typeof(IViewFor<DevicesViewModel>));

            var adapter = Locator.Current.GetService<Robotics.Mobile.Core.Bluetooth.LE.IAdapter>();
            var device = Locator.Current.GetService<XLabs.Platform.Device.IDevice>();
            var btStatusProvider = new BluetoothStatusProvider(device.BluetoothHub);
            var btLeAdapter = new BleAdapter(adapter);

            Router.Navigate.Execute(new DevicesViewModel(btStatusProvider, btLeAdapter, this));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHostWithUserErrorHandler();
        }
    }
}