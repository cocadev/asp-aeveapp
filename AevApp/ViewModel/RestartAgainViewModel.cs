using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Helper;
using AevApp.Model.NavArgs;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.SDK.Service.Interface;

namespace AevApp.ViewModel
{
    public class RestartAgainViewModel : OfficerViewModelBase
    {
        public RestartAgainViewModel(IMessenger messenger, INavigationService navigationService, IAppDialogService appDialogService, 
            AuthManager authManager) : base(messenger, navigationService, appDialogService, authManager)
        {
            StartAgainCommand = new RelayCommand(async () => await StartAgainAsync());
        }

        private async Task StartAgainAsync()
        {
            await NavigationService.NavigateBackTo(typeof(OfficerDashboardPage));
        }

        public override async Task OnNavigated(INavigationArgs parameter)
        {
            if (!(parameter is SubmissionResultNavArg arg))
                return;

            SubmissionResultMessage = arg.SubmissionResult;
        }

        public ICommand StartAgainCommand { get; set; }

        private string _submissionResultMessage;
        public string SubmissionResultMessage
        {
            get => _submissionResultMessage;
            set
            {
                if (_submissionResultMessage == value) return;

                _submissionResultMessage = value;
                RaisePropertyChanged(() => SubmissionResultMessage);
            }
        }
    }
}