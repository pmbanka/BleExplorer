using System;
using System.Reactive.Linq;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels.Services
{
    public interface IServicesViewModel : IRoutableViewModel
    {
        ReactiveCommand<IBleService> DiscoverServices { get; }
        IReadOnlyReactiveList<IServiceTileViewModel> Services { get; }
    }

    public sealed class ServicesViewModel : ReactiveObject, IServicesViewModel
    {
        private readonly IBleDevice _device;
        private readonly ReactiveList<IBleService> _services;
        private readonly IReadOnlyReactiveList<IServiceTileViewModel> _serviceTiles;

        public ServicesViewModel(IBleDevice device, IScreen screen = null)
        {
            HostScreen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            _device = Ensure.NotNull(device, "device");
            _services = new ReactiveList<IBleService>();
            _serviceTiles = _services.CreateDerivedCollection(p => new ServiceTileViewModel(p));

            DiscoverServices = ReactiveCommand
                .CreateAsyncObservable(discoverServicesImpl);
            DiscoverServices.Subscribe(_services.Add);
            DiscoverServices.ThrownExceptions
                .Select(discoverServicesUserError)
                .SelectMany(UserError.Throw)
                .Where(p => p == RecoveryOptionResult.RetryOperation)
                .InvokeCommand(DiscoverServices);
        }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment
        {
            get { return "Services"; }
        }

        public ReactiveCommand<IBleService> DiscoverServices { get; private set; }

        public IReadOnlyReactiveList<IServiceTileViewModel> Services
        {
            get { return _serviceTiles; }
        }

        private UserError discoverServicesUserError(Exception ex)
        {
            return new UserError("Services could not be discovered", ex.Message,
                new[]
                {
                    RecoveryCommand.Cancel,
                    new RecoveryCommand("Retry", _ => RecoveryOptionResult.RetryOperation)
                });
        }

        private IObservable<IBleService> discoverServicesImpl(object _)
        {
            _services.Clear();
            return _device.DiscoverServices();
        }
    }
}