using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels.Services
{
    public interface IServicesViewModel : IRoutableViewModel
    {
    }

    public sealed class ServicesViewModel : ReactiveObject, IServicesViewModel
    {
        public ServicesViewModel(IBleDevice device, IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            Ensure.NotNull(device, "device");
            UrlPathSegment = String.Format("Services from {0}", device.Name);
        }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment { get; private set; }
    }
}