using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views
{
    public partial class FindDevicesView : IViewFor<IFindDevicesViewModel>
    {
        public FindDevicesView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Devices.Count, v => v.DetectedDevicesLabel.Text,
                count => string.Format("Detected {0} devices", count));
            this.OneWayBind(ViewModel, vm => vm.IsBluetoothOn, v => v.BluetoothState.Text,
                state => string.Format("BL state: {0}", state));
            this.WhenActivated(d => d(Disposable.Empty));
        }

        #region IViewFor<T>

        public IFindDevicesViewModel ViewModel
        {
            get { return (IFindDevicesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<FindDevicesView, IFindDevicesViewModel>(x => x.ViewModel,
                default(IFindDevicesViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IFindDevicesViewModel) value; }
        }

        #endregion
    }
}