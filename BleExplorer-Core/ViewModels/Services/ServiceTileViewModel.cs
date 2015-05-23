using System;
using System.Reactive;
using System.Reactive.Linq;
using BleExplorer.Core.Bluetooth;
using BleExplorer.Core.Utils;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels.Services
{
    public interface IServiceTileViewModel : IReactiveObject
    {
        Guid Id { get; }
        string Name { get; }
        bool IsPrimary { get; }
        ReactiveCommand<Unit> GoToCharacteristicsView { get; }
    }

    public sealed class ServiceTileViewModel : ReactiveObject, IServiceTileViewModel
    {
        private Guid _id;
        private bool _isPrimary;
        private string _name;

        public ServiceTileViewModel(IBleService service, IScreen screen = null)
        {
            Ensure.NotNull(service, "service");
            screen = Ensure.NotNull(screen ?? Locator.Current.GetService<IScreen>(), "screen");
            Id = service.Id;
            Name = service.Name;
            IsPrimary = service.IsPrimary;

            GoToCharacteristicsView = ReactiveCommand.CreateAsyncObservable(Observable.Return(false),
                _ => Observable.Return(Unit.Default));
        }

        public ReactiveCommand<Unit> GoToCharacteristicsView { get; private set; }

        public Guid Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            private set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public bool IsPrimary
        {
            get { return _isPrimary; }
            private set { this.RaiseAndSetIfChanged(ref _isPrimary, value); }
        }
    }
}