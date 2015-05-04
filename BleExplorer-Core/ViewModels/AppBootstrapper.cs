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

            // TODO: Register new views here, then navigate to the first page 
            // in your app
            Locator.CurrentMutable.Register(() => new TestView(), typeof(IViewFor<TestViewModel>));

            Router.Navigate.Execute(new TestViewModel(this));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}