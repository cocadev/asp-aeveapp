using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Helper;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.SDK.Service.Interface;
using Xamarin.SDK.ViewModel;

namespace AevApp.ViewModel
{
    public class OfficerViewModelBase : AppViewModelBase
    {
        private string _username;
        private string _role;
        protected readonly AuthManager AuthManager;

        public OfficerViewModelBase(IMessenger messenger, INavigationService navigationService, IAppDialogService appDialogService, AuthManager authManager) :
            base(messenger, navigationService, appDialogService)
        {
            AuthManager = authManager;
            GetUserInfo();
            LogoutCommand = new RelayCommand(Logout);
        }

        private void Logout()
        {
            ShowDialog("Are you sure you want to logout?", action: async () =>
            {
                AuthManager.Logout();
                await NavigationService.GoHome(typeof(HomePage));
            });

            //            await AppDialogService.ShowMessage("Are you sure you want to logout?", "Logout", "Yes", "No",
            //                async logout =>
            //                {
            //                    if (logout)
            //                    {
            //                        AuthManager.Logout();
            //                        await NavigationService.GoHome(typeof(HomePage));
            //                    }
            //                });
        }

        private void GetUserInfo()
        {
            if (AuthManager.AuthenticatedUser == null)
                return;

            Username = AuthManager.AuthenticatedUser.FirstName + " " + AuthManager.AuthenticatedUser.LastName;
            Role = AuthManager.UserRole;
        }


        public ICommand LogoutCommand { get; set; }


        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChanged(() => Username);
            }
        }
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                RaisePropertyChanged(() => Role);
            }
        }
    }
}