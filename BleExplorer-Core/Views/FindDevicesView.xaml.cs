using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views
{
    public partial class FindDevicesView : IViewFor<FindDevicesViewModel>
    {
        public FindDevicesView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.ToggleScanningForDevices, v => v.ScanToggle.Command);
            this.OneWayBind(ViewModel, vm => vm.IsScanning, v => v.StateLabel.Text, state => state ? "Scanning..." : "Not scanning");
        }

        #region IViewFor<T>

        public FindDevicesViewModel ViewModel
        {
            get { return (FindDevicesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<FindDevicesView, FindDevicesViewModel>(x => x.ViewModel,
                default(FindDevicesViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (FindDevicesViewModel) value; }
        }

        #endregion
    }
}