using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Helper;
using AevApp.Model.Requests;
using AevApp.Service.Interface;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.SDK.Service.Interface;
using Xamarin.SDK.ViewModel;

namespace AevApp.ViewModel
{
    public class AddVpViewModel : OfficerViewModelBase
    {
        private readonly IApiClientService _apiClient;
        protected readonly AuthManager AuthManager;

        public AddVpViewModel(IApiClientService apiClient, AuthManager authManager,
            IMessenger messenger, INavigationService navigationService, IAppDialogService appDialogService) :
            base(messenger, navigationService, appDialogService, authManager)
        {
            _apiClient = apiClient;
            AuthManager = authManager;

            CancelCommand = new RelayCommand(async () => await CancelAddVpAsync());
            SubmitCommand = new RelayCommand(async () => await AddVpAsync());
            SelectAirportCommand = new RelayCommand(ToggleAirportPanel);
            TapAirportCommand = new RelayCommand<CheckedItem>(TapAnAirport);
            InitAirportCodes();
        }

        private void TapAnAirport(CheckedItem selectedAirport)
        {
            foreach (var l in Airports)
            {
                l.Checked = l == selectedAirport;
            }

            AirportCode = selectedAirport.Title;
            ToggleAirportPanel();
        }

        private void ToggleAirportPanel()
        {
            IsSelectingAirport = !IsSelectingAirport;
        }

        private void InitAirportCodes()
        {
            Airports = new ObservableCollection<CheckedItem>
            {
                new CheckedItem(){ Id = "MEL", Title = "MEL"},
                new CheckedItem(){ Id ="AUS", Title = "AUS"}
            };
        }

        private async Task AddVpAsync()
        {
            if (!ValidateForm())
            {
                ShowDialog("Please enter all mandatory fields.", "Cancel","OK", () => { });
                return;
            }

            ShowActivityIndicator("Adding VP");

            try
            {
                var addResult = await _apiClient.AddVpAsync(GetAddVpRequest(), AuthManager.AuthToken);
                DismissActivityIndicator();
                ShowDialog($"VP for {FirstName} {LastName} was created successfully.", "", "OK",
                    async () =>
                    {
                        await NavigationService.GoBack();
                    });
            }
            catch (Exception ex)
            {
                DismissActivityIndicator();
                ShowDialog(ex.Message, "", "OK", () => { });
            }
        }

        private AddVpRequest GetAddVpRequest()
        {
            // The VP is valid from now until the same time on the provided expiry date.
            // Do not be shocked if this changes...
            var validFrom = DateTimeOffset.Now;
            var validTo = new DateTimeOffset(DateTime.ParseExact(Expires, "yyyy-MM-dd", null)).Add(validFrom.TimeOfDay);

            return new AddVpRequest
            {
                VpId = AsicId,
                LastName = LastName,
                FirstName = FirstName,
                IdentificationNumber = IdentificationNumber,
                Company = Company,
                ValidFrom = validFrom,
                ValidTo = validTo
            };
        }

        private bool ValidateForm()
        {
            ValidateFormTrigger++;
            return !string.IsNullOrWhiteSpace(AsicId) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(IdentificationNumber) &&
                   !string.IsNullOrWhiteSpace(Company) &&
                   !string.IsNullOrWhiteSpace(AirportCode);
        }

        private async Task CancelAddVpAsync()
        {
            await NavigationService.GoBack();
        }

        public ICommand CancelCommand { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand SelectAirportCommand { get; set; }
        public ICommand TapAirportCommand { get; set; }

        private string _asicId;
        public string AsicId
        {
            get => _asicId;
            set
            {
                if (_asicId == value) return;
                _asicId = value;
                RaisePropertyChanged(() => AsicId);
            }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName == value) return;
                _firstName = value;
                RaisePropertyChanged(() => FirstName);
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName == value) return;
                _lastName = value;
                RaisePropertyChanged(() => LastName);
            }
        }

        private string _identificationNumber;
        public string IdentificationNumber
        {
            get => _identificationNumber;
            set
            {
                if (_identificationNumber == value) return;
                _identificationNumber = value;
                RaisePropertyChanged(() => IdentificationNumber);
            }
        }

        private string _airportCode;
        public string AirportCode
        {
            get => _airportCode;
            set
            {
                _airportCode = value;
                RaisePropertyChanged(() => AirportCode);
            }
        }

        private string _company;
        public string Company
        {
            get => _company;
            set
            {
                if (_company == value) return;
                _company = value;
                RaisePropertyChanged(() => Company);
            }
        }

        private string _expires = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        public string Expires
        {
            get => _expires;
            set
            {
                if (_expires == value) return;
                _expires = value;
                RaisePropertyChanged(() => Expires);
            }
        }

        private ObservableCollection<CheckedItem> _airports = new ObservableCollection<CheckedItem>();
        public ObservableCollection<CheckedItem> Airports
        {
            get => _airports;
            set
            {
                if (_airports == value) return;
                _airports = value;
                RaisePropertyChanged(() => Airports);
            }
        }

        private int _validateFormTrigger;

        public int ValidateFormTrigger
        {

            get => _validateFormTrigger;
            set
            {
                if (_validateFormTrigger == value) return;
                _validateFormTrigger = value;
                RaisePropertyChanged(() => ValidateFormTrigger);
            }
        }

        private bool _isSelectingAirport;

        public bool IsSelectingAirport
        {
            get => _isSelectingAirport;
            set
            {
                if (_isSelectingAirport == value) return;

                _isSelectingAirport = value;
                RaisePropertyChanged(() => IsSelectingAirport);
            }
        }
    }
}