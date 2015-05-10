using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
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

            UserError.RegisterHandler(ue =>
            {
                var toast = Toast.MakeText(this, ue.ErrorMessage, ToastLength.Long);
                toast.Show();
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            var bootstrapper = RxApp.SuspensionHost.GetAppState<AppBootstrapper>();

            SetPage(bootstrapper.CreateMainPage());
        }
    }
}