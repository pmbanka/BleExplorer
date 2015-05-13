using System;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBleService
    {
        Guid Id { get; }

        string Name { get; }

        bool IsPrimary { get; }

        IObservable<IBleCharacteristic> DiscoverCharacteristics();
    }
}