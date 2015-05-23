using BleExplorer.Core.ViewModels.Services;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views.Services
{
    public partial class ServiceTileView : IViewFor<IServiceTileViewModel>
    {
        public ServiceTileView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameLabel.Text);
            this.OneWayBind(ViewModel, vm => vm.Id, v => v.GuidLabel.Text, p => p.ToString());
            this.OneWayBind(ViewModel, vm => vm.IsPrimary, v => v.PrimaryLabel.Text,
                isPrimary => isPrimary ? "Primary" : "", () => "");
        }

        #region IViewFor<T>

        public IServiceTileViewModel ViewModel
        {
            get { return (IServiceTileViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<ServiceTileView, IServiceTileViewModel>(x => x.ViewModel,
                default(IServiceTileViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IServiceTileViewModel) value; }
        }

        #endregion
    }
}