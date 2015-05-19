using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.ViewModels.Devices;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views.Devices
{
    public partial class DeviceTileView : IViewFor<IDeviceTileViewModel>
    {
        public DeviceTileView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameLabel.Text);
            this.OneWayBind(ViewModel, vm => vm.Id, v => v.GuidLabel.Text, p => p.ToString());
        }

        #region IViewFor<T>

        public IDeviceTileViewModel ViewModel
        {
            get { return (IDeviceTileViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<DeviceTileView, IDeviceTileViewModel>(x => x.ViewModel,
                default(IDeviceTileViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IDeviceTileViewModel) value; }
        }

        #endregion
    }
}