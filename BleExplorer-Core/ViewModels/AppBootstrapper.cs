using Akavache;
using BleExplorer.Core.Models;
using BleExplorer.Core.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Robotics.Mobile.Core.Bluetooth.LE;
using Splat;
using Xamarin.Forms;

namespace BleExplorer.Core.ViewModels
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            BlobCache.ApplicationName = "BleExplorer";

            var adapter = Locator.Current.GetService<IAdapter>();
            var rxAdapter = new RxBleAdapter(adapter);

            Locator.CurrentMutable.Register(() => rxAdapter, typeof(IRxBleAdapter));
            Locator.CurrentMutable.Register(() => new FindDevicesView(), typeof(IViewFor<FindDevicesViewModel>));

            Router.Navigate.Execute(new FindDevicesViewModel(rxAdapter, this));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}