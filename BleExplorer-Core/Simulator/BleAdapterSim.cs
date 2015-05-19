using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.Bluetooth;
using ReactiveUI;

namespace BleExplorer.Core.Simulator
{
    public sealed class BleAdapterSim : IBleAdapter
    {
        public IObservable<IBleDevice> DiscoverDevices()
        {
            return new[]
            {
                new BleDeviceSim(),
                new BleDeviceSim()
            }.ToObservable().Delay(TimeSpan.FromMilliseconds(200), RxApp.TaskpoolScheduler);
        }
    }
}