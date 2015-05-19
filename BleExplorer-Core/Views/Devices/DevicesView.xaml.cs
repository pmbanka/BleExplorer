using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BleExplorer.Core.ViewModels.Devices;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views.Devices
{
    public partial class DevicesView : IViewFor<IDevicesViewModel>
    {
        public DevicesView()
        {
            InitializeComponent();
            this.WhenActivated(onActivated);
        }

        private IEnumerable<IDisposable> onActivated()
        {
            yield return this.OneWayBind(ViewModel, vm => vm.Devices.IsEmpty, v => v.EmptyListLabel.IsVisible, () => true);
            yield return this.OneWayBind(ViewModel, vm => vm.Devices.IsEmpty, v => v.DeviceTilesList.IsVisible, () => false,
                vmToViewConverterOverride: NegatingConverter.Instance);
            yield return this.BindCommand(ViewModel, vm => vm.DiscoverDevices, v => v.ScanToolbarButton);
            yield return this.WhenAnyObservable(v => v.ViewModel.DiscoverDevices.IsExecuting)
                .BindTo(this, v => v.ActivityIndicator.IsRunning, () => false);
            yield return this.WhenAnyObservable(v => v.ViewModel.DiscoverDevices.IsExecuting)
                .BindTo(this, v => v.ActivityIndicator.IsVisible, () => false);
            yield return this.OneWayBind(ViewModel, vm => vm.Devices, v => v.DeviceTilesList.ItemsSource);
            yield return DeviceTilesList.Events().ItemTapped
                .Select(p => (IDeviceTileViewModel) p.Item)
                .SelectMany(p => p.GoToServicesView.ExecuteAsync())
                .Subscribe(_ => DeviceTilesList.SelectedItem = null);

            if (ViewModel.DiscoverDevices.CanExecute(null) && ViewModel.Devices.IsEmpty)
            {
                ViewModel.DiscoverDevices.ExecuteAsync().Subscribe();
            }
        }

        #region IViewFor<T>

        public IDevicesViewModel ViewModel
        {
            get { return (IDevicesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<DevicesView, IDevicesViewModel>(x => x.ViewModel,
                default(IDevicesViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IDevicesViewModel) value; }
        }

        #endregion
    }
}