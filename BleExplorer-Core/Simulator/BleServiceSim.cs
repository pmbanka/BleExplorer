using System;
using System.Reactive.Linq;
using BleExplorer.Core.Bluetooth;

namespace BleExplorer.Core.Simulator
{
    public sealed class BleServiceSim : IBleService
    {
        private readonly Guid _id;
        private readonly string _name;
        private readonly bool _isPrimary;

        public BleServiceSim(string name = "Simulated service", Guid? id = null, bool isPrimary = true)
        {
            _name = name;
            _id = id ?? Guid.NewGuid();
            _isPrimary = isPrimary;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsPrimary
        {
            get { return _isPrimary; }
        }

        public IObservable<IBleCharacteristic> DiscoverCharacteristics()
        {
            return Observable.Empty<IBleCharacteristic>();
        }
    }
}