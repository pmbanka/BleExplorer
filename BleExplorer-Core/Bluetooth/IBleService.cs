using System;
using BleExplorer.Core.Utils;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace BleExplorer.Core.Bluetooth
{
    public interface IBleService
    {
        Guid Id { get; }

        string Name { get; }

        bool IsPrimary { get; }

        IObservable<IBleCharacteristic> DiscoverCharacteristics();
    }

    public sealed class BleService : IBleService
    {
        private readonly IService _service;

        public BleService(IService service)
        {
            _service = Ensure.NotNull(service, "service");
        }

        public Guid Id
        {
            get { return _service.ID; }
        }

        public string Name
        {
            get { return _service.Name; }
        }

        public bool IsPrimary
        {
            get { return _service.IsPrimary; }
        }

        public IObservable<IBleCharacteristic> DiscoverCharacteristics()
        {
            throw new NotImplementedException();
        }
    }
}