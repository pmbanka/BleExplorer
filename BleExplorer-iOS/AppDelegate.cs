using BleExplorer.Core.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using Robotics.Mobile.Core.Bluetooth.LE;
using Splat;
using Xamarin.Forms;

namespace BleExplorer
{
    [Register("AppDelegate")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow _window;
        AutoSuspendHelper _suspendHelper;

        public AppDelegate()
        {
            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            RxApp.SuspensionHost.SetupDefaultSuspendResume();

            _suspendHelper = new AutoSuspendHelper(this);
            _suspendHelper.FinishedLaunching(app, options);


            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            var bootstrapper = RxApp.SuspensionHost.GetAppState<AppBootstrapper>();

            _window.RootViewController = bootstrapper.CreateMainPage().CreateViewController();
            _window.MakeKeyAndVisible();

            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            _suspendHelper.DidEnterBackground(application);
        }

        public override void OnActivated(UIApplication application)
        {
            _suspendHelper.OnActivated(application);
        }
    }
}