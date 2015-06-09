using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace BleExplorer.Core.Utils
{
    public static class Extensions
    {
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> self)
        {
            return self.Select(_ => Unit.Default);
        }

        public static IObservable<object> ToObject(this IObservable<Unit> self)
        {
            return self.Select(_ => (object) null);
        }

        public static ReactiveCommand<Unit> NavigateCommandFor<T>(this RoutingState router, T viewModel)
            where T : IRoutableViewModel
        {
            return ReactiveCommand.CreateAsyncObservable(
                router.Navigate.CanExecuteObservable,
                _ => router.Navigate.ExecuteAsync(viewModel).ToUnit());
        }

        public static ReactiveCommand<Unit> NavigateCommandFor<T>(this RoutingState router, Func<T> viewModelFactory)
            where T : IRoutableViewModel
        {
            return ReactiveCommand.CreateAsyncObservable(
                router.Navigate.CanExecuteObservable,
                _ => router.Navigate.ExecuteAsync(viewModelFactory()).ToUnit());
        }
    }
}