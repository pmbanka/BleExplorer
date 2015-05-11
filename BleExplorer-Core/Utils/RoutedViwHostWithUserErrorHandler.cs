using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace BleExplorer.Core.Utils
{
    internal class RoutedViwHostWithUserErrorHandler : RoutedViewHost
    {
        public RoutedViwHostWithUserErrorHandler()
        {
            this.WhenActivated(d =>
                d(UserError.RegisterHandler(async error =>
                {
                    int recoveryCount = error.RecoveryOptions.Count;
                    if (recoveryCount == 0)
                    {
                        throw new InvalidOperationException("Cannot handle UserError without RecoveryOptions");
                    }
                    if (recoveryCount == 1)
                    {
                        return await displayAlertWithOneOption(error);
                    }
                    if (recoveryCount == 2)
                    {
                        return await displayAlertWithTwoOptions(error);
                    }
                    return await displayAlertWithMultipleOptions(error);
                })));
        }

        private async Task<RecoveryOptionResult> displayAlertWithOneOption(UserError error)
        {
            await DisplayAlert(
                error.ErrorMessage,
                error.ErrorCauseOrResolution,
                error.RecoveryOptions[0].CommandName);
            return RecoveryOptionResult.CancelOperation;
        }

        private async Task<RecoveryOptionResult> displayAlertWithTwoOptions(UserError error)
        {
            var result = await DisplayAlert(
                error.ErrorMessage,
                error.ErrorCauseOrResolution,
                error.RecoveryOptions[0].CommandName,
                error.RecoveryOptions[1].CommandName);
            var recoveryOption = error.RecoveryOptions[result ? 1 : 0].RecoveryResult;
            Debug.Assert(recoveryOption != null);
            return recoveryOption.Value;
        }

        private async Task<RecoveryOptionResult> displayAlertWithMultipleOptions(UserError error)
        {
            var result = await DisplayActionSheet(
                error.ErrorMessage,
                error.RecoveryOptions[0].CommandName,
                null,
                error.RecoveryOptions.Skip(1).Select(p => p.CommandName).ToArray());
            var recoveryOption = error.RecoveryOptions.Single(x => x.CommandName == result).RecoveryResult;
            Debug.Assert(recoveryOption != null);
            return recoveryOption.Value;
        }
    }
}