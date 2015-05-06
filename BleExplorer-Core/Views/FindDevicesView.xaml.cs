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
    public partial class FindDevicesView : IViewFor<IFindDevicesViewModel>
    {
        public FindDevicesView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.ScanForDevices, v => v.ScanToolbarButton.Command);
            this.OneWayBind(ViewModel, vm => vm.IsScanning, v => v.StateLabel.Text,
                scanning => scanning ? "Scanning..." : "Not scanning");
            this.OneWayBind(ViewModel, vm => vm.DetectedDevices, v => v.DetectedDevicesLabel.Text,
                count => string.Format("Detected {0} devices", count));
        }

        #region IViewFor<T>

        public IFindDevicesViewModel ViewModel
        {
            get { return (FindDevicesViewModel) GetValue(ViewModelProperty); }
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