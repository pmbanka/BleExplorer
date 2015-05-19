using System;
using System.Reactive.Linq;
using BleExplorer.Core.Bluetooth;
using ReactiveUI;

namespace BleExplorer.Core.Simulator
{
    public sealed class BleDeviceSim : IBleDevice
    {
        private readonly Guid _id;
        private readonly string _name;

        public BleDeviceSim(string name = "Simulated device", Guid? id = null)
        {
            _id = id ?? Guid.NewGuid();
            _name = name;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IObservable<IBleService> DiscoverServices()
        {
            return new[]
            {
                new BleServiceSim(),
                new BleServiceSim()
            }.ToObservable().Delay(TimeSpan.FromMilliseconds(200), RxApp.TaskpoolScheduler);
        }
    }
}