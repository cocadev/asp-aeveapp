using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Helper;
using AevApp.Model;
using AevApp.Model.NavArgs;
using AevApp.Service.Interface;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.SDK.Helper.Interface;
using Xamarin.SDK.Service.Interface;
using Xamarin.SDK.ViewModel;

namespace AevApp.ViewModel
{
    public class SetupViewModel : OfficerViewModelBase
    {
        private readonly IAppSettings _appSettings;
        private readonly IApiClientService _apiClient;
        //default airport id to 1 now
        private string _airport = "1";
        private string _location;
        private bool _isAirportValid = true;
        private bool _isLocationValid = true;
        private string _token;

        public SetupViewModel(IAppSettings appSettings, IMessenger messenger, INavigationService navigationService, AuthManager authManager,
            IAppDialogService appDialogService, IApiClientService apiClient) :
            base(messenger, navigationService, appDialogService, authManager)
        {
            _appSettings = appSettings;
            _apiClient = apiClient;
            SetCommand = new RelayCommand(async () => await DoSetupAsync());
            SelectLocationCommand = new RelayCommand<Location>(UpdateLocationSelection);
        }

        private void UpdateLocationSelection(Location location)
        {
            foreach (var l in Locations)
            {
                l.IsSelected = l == location;
            }

            SelectedLocation = location;
        }

        public override async Task OnNavigated(INavigationArgs parameter)
        {
            _token = ((SetupNavArg)parameter).Token;
            try
            {
                int.TryParse(Airport, out var airportId);
                await Task.Run(async () =>
                {
                    var locations = await _apiClient.GetAllLocationAsync(airportId, _token);
                    Locations = new ObservableCollection<Location>(locations);
                });
            }
            catch (Exception ex)
            {

            }
        }

        private async Task DoSetupAsync()
        {
            IsAirportValid = true;
            IsLocationValid = true;
            if (!ValidateInput()) return;

            _appSettings.Set(Constants.Airport, Airport);
            _appSettings.Set(Constants.Location, SelectedLocation.Id.ToString());

            await NavigationService.NavigateTo(typeof(OfficerDashboardPage), null, true);
        }

        private bool ValidateInput()
        {
            IsAirportValid = int.TryParse(Airport, out var airportId);

            //            IsLocationValid = int.TryParse(Location, out var locationId);

            IsLocationValid = SelectedLocation != null;


            return IsAirportValid && IsLocationValid;
        }

        public string Airport
        {
            get => _airport;
            set
            {
                _airport = value;
                RaisePropertyChanged(() => Airport);
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                RaisePropertyChanged(() => Location);
            }
        }

        public bool IsAirportValid
        {
            get => _isAirportValid;
            set
            {
                _isAirportValid = value;
                RaisePropertyChanged(() => IsAirportValid);
            }
        }

        public bool IsLocationValid
        {
            get => _isLocationValid;
            set
            {
                _isLocationValid = value;
                RaisePropertyChanged(() => IsLocationValid);
            }
        }

        private ObservableCollection<Location> _locations;

        public ObservableCollection<Location> Locations
        {
            get => _locations;
            set
            {
                _locations = value;
                RaisePropertyChanged(() => Locations);
            }
        }

        private Location _selectedLocation;

        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                RaisePropertyChanged(() => SelectedLocation);
            }
        }

        public ICommand SetCommand { get; set; }
        public ICommand SelectLocationCommand { get; set; }
    }
}