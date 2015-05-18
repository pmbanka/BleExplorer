using System;
using System.Collections.Generic;
using System.Linq;
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