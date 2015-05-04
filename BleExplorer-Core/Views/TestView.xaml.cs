using BleExplorer.Core.ViewModels;
using ReactiveUI;
using Xamarin.Forms;

namespace BleExplorer.Core.Views
{
    public partial class TestView : IViewFor<TestViewModel>
    {
        public TestView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.TheGuid, x => x.TheGuid.Text);
        }

        #region IViewFor<T>

        public TestViewModel ViewModel
        {
            get { return (TestViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<TestView, TestViewModel>(x => x.ViewModel, default(TestViewModel));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TestViewModel) value; }
        }

        #endregion
    }
}

