using System;
using System.Runtime.Serialization;
using ReactiveUI;
using Splat;

namespace BleExplorer.Core.ViewModels
{
    [DataContract]
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment
        {
            get { return "State Serialization Test"; }
        }

        public IScreen HostScreen { get; protected set; }

        string _theGuid;
        [DataMember]
        public string TheGuid
        {
            get { return _theGuid; }
            set { this.RaiseAndSetIfChanged(ref _theGuid, value); }
        }

        public TestViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
            TheGuid = Guid.NewGuid().ToString();
        }
    }
}