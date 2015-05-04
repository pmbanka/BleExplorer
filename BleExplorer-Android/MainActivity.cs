using Android.App;
using Android.OS;
using BleExplorer.Core.ViewModels;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace BleExplorer
{
    [Activity(Label = "BLE Explorer", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            var bootstrapper = RxApp.SuspensionHost.GetAppState<AppBootstrapper>();


            SetPage(bootstrapper.CreateMainPage());
        }
    }
}