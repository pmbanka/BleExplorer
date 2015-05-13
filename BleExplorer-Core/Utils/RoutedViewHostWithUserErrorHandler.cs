using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace BleExplorer.Core.Utils
{
    internal class RoutedViewHostWithUserErrorHandler : RoutedViewHost
    {
        public RoutedViewHostWithUserErrorHandler()
        {
            this.WhenActivated(d =>
                d(UserError.RegisterHandler(async error =>
                {
                    int recoveryCount = error.RecoveryOptions.Count;
                    switch (recoveryCount)
                    {
                        case 0:
                            throw new InvalidOperationException("Cannot handle UserError without RecoveryOptions");
                        case 1:
                            return await displayAlertWithOneOption(error);
                        case 2:
                            return await displayAlertWithTwoOptions(error);
                        default:
                            return await displayAlertWithMultipleOptions(error);
                    }
                })));
        }

        private async Task<RecoveryOptionResult> displayAlertWithOneOption(UserError error)
        {
            var recoveryOption = error.RecoveryOptions[0];
            await DisplayAlert(
                error.ErrorMessage,
                error.ErrorCauseOrResolution,
                recoveryOption.CommandName);
            recoveryOption.Execute(null);
            return RecoveryOptionResult.CancelOperation;
        }

        private async Task<RecoveryOptionResult> displayAlertWithTwoOptions(UserError error)
        {
            var result = await DisplayAlert(
                error.ErrorMessage,
                error.ErrorCauseOrResolution,
                error.RecoveryOptions[0].CommandName,
                error.RecoveryOptions[1].CommandName);
            var recoveryOption = error.RecoveryOptions[result ? 0 : 1];
            recoveryOption.Execute(null);
            return recoveryOption.RecoveryResult ?? RecoveryOptionResult.CancelOperation;
        }

        private async Task<RecoveryOptionResult> displayAlertWithMultipleOptions(UserError error)
        {
            var result = await DisplayActionSheet(
                error.ErrorMessage,
                error.RecoveryOptions[0].CommandName,
                null,
                error.RecoveryOptions.Skip(1).Select(p => p.CommandName).ToArray());
            var recoveryOption = error.RecoveryOptions.Single(x => x.CommandName == result);
            recoveryOption.Execute(null);
            return recoveryOption.RecoveryResult ?? RecoveryOptionResult.CancelOperation;
        }
    }
}