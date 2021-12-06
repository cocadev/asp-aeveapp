using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Dependency;
using AevApp.Event;
using AevApp.Helper;
using AevApp.Helper.Interface;
using AevApp.Model;
using AevApp.Model.NavArgs;
using AevApp.Service.Interface;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using Xamarin.SDK.Helper.Interface;
using Xamarin.SDK.Service.Interface;
using Xamarin.SDK.ViewModel;

namespace AevApp.ViewModel
{
    public class HomeViewModel : AppViewModelBase
    {
        private readonly IApiClientService _apiClient;
        private readonly IAppSettings _appSettings;
        //need this _authManager to initialize the event subscription
        private readonly AuthManager _authManager;
        private readonly IConfigManager _configManager;
        private string _username;
        private string _password;
        private bool _isUsernameValid = true;
        private bool _isPasswordValid = true;
        private string _appVersion;

        public HomeViewModel(IMessenger messenger,
            INavigationService navigationService,
            IAppDialogService appDialogService,
            IApiClientService apiClient,
            IAppSettings appSettings, 
            IConfigManager configManager,
            AuthManager authManager)
            : base(messenger, navigationService, appDialogService)
        {
            _apiClient = apiClient;
            _appSettings = appSettings;
            _authManager = authManager;
            _configManager = configManager;
            LoginCommand = new RelayCommand(async () => await HandleLogin());
            TestCommand = new RelayCommand<byte[]>(async (imgSrc) => await HandleTest(imgSrc));
            AppVersion = $"Version: {DependencyService.Get<IAppInfo>().ApplicationVersion}";
        }

        private async Task HandleTest(byte[] bytes)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "temp.jpg");
            File.WriteAllBytes(fileName, bytes);
        }

        private async Task HandleLogin()
        {
            var appInfo = DependencyService.Get<IAppInfo>();
            if (!appInfo.HasCameraPermission)
            {
                await AppDialogService.ShowError($"Camera permission is required to proceed", "Permission Required", "OK", () => { });
                return;
            }

            try
            {
#if DEBUG
                //Username = "jonespm";
                //Password = "g00gly@ASIC!";
#endif

                ShowActivityIndicator("Authenticating with Server. Please Wait...");

                if (!ValidateInput())
                    return;

                //Do Login
                var accessTokenResponse = await _apiClient.LoginAsync(Username, Password);

                if (accessTokenResponse != null)
                {
                    var profile = await _apiClient.GetProfile(accessTokenResponse.AccessToken);

                    // Get configuration from server
                    var clientId = int.Parse(_configManager.Get(Constants.ConfigNames.ClientId));
                    var clientConfiguration = await _apiClient.GetClientConfigurationDto(clientId, accessTokenResponse.AccessToken);

                    var login = new LoginInfo
                    {
                        Token = accessTokenResponse.AccessToken,
                        Profile = profile
                    };

                    if (clientConfiguration != null)
                    {
                        if (clientConfiguration.ConfigurationValues.ContainsKey("azureFaceServiceApiRoot"))
                        {
                            _appSettings.Set(Constants.AzureFaceServiceApiRoot,
                                clientConfiguration.ConfigurationValues["azureFaceServiceApiRoot"]);
                        }
                        if (clientConfiguration.ConfigurationValues.ContainsKey("azureFaceServiceSubscriptionKey"))
                        {
                            _appSettings.Set(Constants.AzureFaceServiceSubscriptionKey,
                                clientConfiguration.ConfigurationValues["azureFaceServiceSubscriptionKey"]);
                        }
                    }

                    // Avoid using local settings
                    if (!string.IsNullOrWhiteSpace(_configManager.Get(Constants.ConfigNames.AzureFaceServiceApiRoot)))
                    {
                        _appSettings.Set(Constants.AzureFaceServiceApiRoot, _configManager.Get(Constants.ConfigNames.AzureFaceServiceApiRoot));
                    }

                    if (!string.IsNullOrWhiteSpace(_configManager.Get(Constants.ConfigNames.AzureFaceServiceSubscriptionKey)))
                    {
                        _appSettings.Set(Constants.AzureFaceServiceSubscriptionKey, _configManager.Get(Constants.ConfigNames.AzureFaceServiceSubscriptionKey));
                    }

                    MessengerInstance.Send(new UserAuthenticatedEvent { User = login });
                    //always go to set up page for setting location
                    await NavigationService.NavigateTo(typeof(SetupPage), new SetupNavArg
                    {
                        Token = login.Token
                    }, true);

                    Username = string.Empty;
                    Password = string.Empty;
                }
                else
                {
                    throw new Exception("Error login response returned from server. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                await AppDialogService.ShowError($"Error reason: {ex.Message}", "Failed to login", "OK", () => { });
            }
            finally
            {
                DismissActivityIndicator();
            }
        }

        private bool ValidateInput()
        {
            IsUsernameValid = true;
            IsPasswordValid = true;
            if (string.IsNullOrWhiteSpace(Username))
            {
                IsUsernameValid = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                IsPasswordValid = false;
            }

            return IsUsernameValid && IsPasswordValid;
        }

        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set
            {
                _isPasswordValid = value;
                RaisePropertyChanged(() => IsPasswordValid);
            }
        }

        public bool IsUsernameValid
        {
            get => _isUsernameValid;
            set
            {
                _isUsernameValid = value;
                RaisePropertyChanged(() => IsUsernameValid);
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChanged(() => Username);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public string AppVersion
        {
            get => _appVersion;
            set
            {
                _appVersion = value;
                RaisePropertyChanged(() => AppVersion);
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand TestCommand { get; set; }
    }
}