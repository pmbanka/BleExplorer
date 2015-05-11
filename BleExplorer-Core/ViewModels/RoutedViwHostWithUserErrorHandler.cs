using System.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace BleExplorer.Core.ViewModels
{
    internal class RoutedViwHostWithUserErrorHandler : RoutedViewHost
    {
        public RoutedViwHostWithUserErrorHandler()
        {
            var ueHandler = UserError.RegisterHandler(async error =>
            {
                await
                    DisplayAlert(error.ErrorMessage, error.ErrorCauseOrResolution,
                        error.RecoveryOptions.First().CommandName);

                return RecoveryOptionResult.CancelOperation;
            });
        }
    }
}