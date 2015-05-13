using System;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBleCharacteristic
    {
        Guid Id { get; }

        string Uuid { get; }

        string Name { get; }
    }
}