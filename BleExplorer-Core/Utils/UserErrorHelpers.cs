using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.Utils
{
    public static class UserErrorHelpers
    {
        public static IObservable<RecoveryOptionResult> CatchLogRepeat(
            this IObservable<RecoveryOptionResult> self,
            IEnableLogger log,
            string streamName,
            TimeSpan? timeout = null)
        {
            return self.LoggedCatch(
                log,
                (Exception _) => Observable
                    .Return(RecoveryOptionResult.CancelOperation)
                    .Delay(timeout ?? TimeSpan.FromSeconds(2)),
                streamName)
                .Repeat();
        }
    }
}