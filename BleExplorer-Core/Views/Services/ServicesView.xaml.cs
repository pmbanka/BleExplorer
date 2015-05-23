using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using BleExplorer.Core.ViewModels.Services;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views.Services
{
    public partial class ServicesView : IViewFor<IServicesViewModel>
    {
        public ServicesView()
        {
            InitializeComponent();

            this.WhenActivated(onActivated);
        }

        private IEnumerable<IDisposable> onActivated()
        {
            yield return this.WhenAnyObservable(v => v.ViewModel.DiscoverServices.IsExecuting)
                .BindTo(this, v => v.ActivityIndicator.IsRunning, () => false);
            yield return this.WhenAnyObservable(v => v.ViewModel.DiscoverServices.IsExecuting)
                .BindTo(this, v => v.ActivityIndicator.IsVisible, () => false);
            yield return this.OneWayBind(ViewModel, vm => vm.Services, v => v.ServiceTilesList.ItemsSource);
            yield return ServiceTilesList.Events().ItemTapped
                .Select(p => (IServiceTileViewModel) p.Item)
                .SelectMany(p => p.GoToCharacteristicsView.ExecuteAsync())
                .Subscribe(_ => ServiceTilesList.SelectedItem = null);

            if (ViewModel.DiscoverServices.CanExecute(null) && ViewModel.Services.IsEmpty)
            {
                ViewModel.DiscoverServices.ExecuteAsync().Subscribe();
            }
        }

        #region IViewFor<T>

        public IServicesViewModel ViewModel
        {
            get { return (IServicesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<ServicesView, IServicesViewModel>(x => x.ViewModel, default(IServicesViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IServicesViewModel) value; }
        }

        #endregion
    }
}