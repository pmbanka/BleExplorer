using System;
using Android.App;
using Android.Runtime;
using BleExplorer.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BleExplorer
{
    [Application(Label = "BleExplorer-Android")]
    public class AndroidApplication : Application
    {
        // ReSharper disable once NotAccessedField.Local
        AutoSuspendHelper _autoSuspendHelper;
        public AndroidApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle,transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _autoSuspendHelper = new AutoSuspendHelper(this);
            RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper();

            RxApp.SuspensionHost.SetupDefaultSuspendResume();
        }
    }
}

