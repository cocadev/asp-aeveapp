using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Dependency;
using AevApp.Helper;
using AevApp.Helper.Interface;
using AevApp.Model;
using AevApp.Model.NavArgs;
using AevApp.Model.Requests;
using AevApp.Model.Responses;
using AevApp.Service.Interface;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.SDK.Helper.Interface;
using Xamarin.SDK.Service.Interface;
using Xamarin.Essentials;
using Location = AevApp.Model.Location;

namespace AevApp.ViewModel
{
    public class SecurityCheckSubmissionViewModel : OfficerViewModelBase
    {
        private CredentialDto _credential;
        private decimal _confidence;
        private bool _faceIsIdentical;
        private DateTimeOffset _commencedAt;

        private readonly IAppSettings _appSettings;
        private readonly IApiClientService _apiClient;
        private readonly IFaceService _faceService;
        private readonly IConfigManager _configManager;

        private SubmitSecuritySubmissionRequest SecuritySubmission { get; set; }
        private SecurityScanPolicySecurityTierGroupingDto _securityScanPolicySecurityTierGrouping;

        public SecurityCheckSubmissionViewModel(IMessenger messenger, INavigationService navigationService, IAppDialogService appDialogService,
            AuthManager authManager, IAppSettings appSettings, IApiClientService apiClient, IFaceService faceService, IStorageService storageService, IConfigManager configManager)
            : base(messenger, navigationService, appDialogService, authManager)
        {
            _appSettings = appSettings;
            _apiClient = apiClient;
            _faceService = faceService;
            SubmitCommand = new RelayCommand(async () => await SubmitAsync());
            CancelCommand = new RelayCommand(Cancel);
            HideVehicleDetailsDialogCommand = new RelayCommand(() => IsVehicleDetailsDialogVisible = false);
            ToggleSecurityCheckCommand = new RelayCommand<SecurityTierItem>(ToggleSecurityTierItem);
            UpdateAsicIdCommand = new RelayCommand(async () => await UpdateAsicInfo());
            SelectNotes = new RelayCommand(ToggleNotesPanel);
            ConfirmSelectedNotes = new RelayCommand(ConfirmNotes);
            SelectYear = new RelayCommand(DoSelectYear);
            SelectMonth = new RelayCommand(DoSelectMonth);
            SelectVehicleType = new RelayCommand(DoSelectVehicleType);
            SelectAirport = new RelayCommand(DoSelectAirport);
            TapNoteCommand = new RelayCommand<CheckedItem>(TapANote);
            TapMonthCommand = new RelayCommand<CheckedItem>(TapAMonth);
            TapYearCommand = new RelayCommand<CheckedItem>(TapAYear);
            TapVehicleTypeCommand = new RelayCommand<CheckedItem>(TapAVehicleType);
            TapAirportCommand = new RelayCommand<CheckedItem>(TapAAirport);
            _configManager = configManager;

            InitYearsAndMonths();
            InitAirportCodes();
            InitNotes();
            InitVehicleTypes();

            SelectedYearValue = "Year";
            SelectedMonthValue = "Month";
            SelectedVehicleTypeValue = "Vehicle Type";
            SelectedAirportValue = "Airport";

            try
            {
                // Prime location
                Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private void DoSelectMonth()
        {
            ToggleMonthPanel();
            Task.Run(async () =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedMonth != null)
                    {
                        SelectedMonth.Checked = true;
                    }
                });
            });
        }

        private void DoSelectYear()
        {
            ToggleYearPanel();
            Task.Run(async () =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedYear != null)
                    {
                        SelectedYear.Checked = true;
                    }
                });
            });
        }

        private void DoSelectAirport()
        {
            ToggleAirportPanel();
            Task.Run(async () =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedAirport != null)
                    {
                        SelectedAirport.Checked = true;
                    }
                });
            });
        }

        private void DoSelectVehicleType()
        {
            ToggleVehicleTypePanel();
            Task.Run(async () =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (SelectedVehicleType != null)
                    {
                        SelectedVehicleType.Checked = true;
                    }
                });
            });
        }

        private void TapAYear(CheckedItem selectedYear)
        {
            foreach (var l in Years)
            {
                l.Checked = l == selectedYear;
            }

            SelectedYear = selectedYear;
            ToggleYearPanel();
        }

        private void TapAMonth(CheckedItem selectedMonth)
        {
            foreach (var l in Months)
            {
                l.Checked = l == selectedMonth;
            }

            SelectedMonth = selectedMonth;
            ToggleMonthPanel();
        }

        private void TapAVehicleType(CheckedItem selectedVehicleType)
        {
            foreach (var l in VehicleTypes)
            {
                l.Checked = l == selectedVehicleType;
            }

            SelectedVehicleType = selectedVehicleType;
            ToggleVehicleTypePanel();
        }

        private void TapANote(CheckedItem note)
        {
            note.Checked = !note.Checked;
        }

        private void TapAAirport(CheckedItem selectedAirport)
        {
            foreach (var l in Airports)
            {
                l.Checked = l == selectedAirport;
            }

            SelectedAirport = selectedAirport;
            ToggleAirportPanel();
        }

        private void ToggleMonthPanel()
        {
            IsSelectingMonth = !IsSelectingMonth;
        }

        private void ToggleYearPanel()
        {
            IsSelectingYear = !IsSelectingYear;
        }

        private void ToggleVehicleTypePanel()
        {
            IsSelectingVehicleType = !IsSelectingVehicleType;
        }

        private void ToggleAirportPanel()
        {
            IsSelectingAiport = !IsSelectingAiport;
        }

        private void ConfirmNotes()
        {
            UpdateSelectedNotes();
            ToggleNotesPanel();
        }

        private void InitNotes()
        {
            Notes = new ObservableCollection<CheckedItem>(new List<CheckedItem>()
            {
                new CheckedItem(){Title = "1. Insufficient Facial Match % - Secondary ID provided"},
                new CheckedItem(){Title = "2. Insufficient Facial Match % - Secondary ID not available"},
                new CheckedItem(){Title = "3. ASIC holder is escorting person with a visitors pass"},
                new CheckedItem(){Title = "4. Expired ASIC"},
                new CheckedItem(){Title = "5. Incorrect Airport code"},
                new CheckedItem(){Title = "6. Operational need not acceptable"},
                new CheckedItem(){Title = "7. Incorrect holograph on ASIC"},
                new CheckedItem(){Title = "8. Airside Screening Refused"},
                new CheckedItem(){Title = "9. Vehicle processed with ETD"}
            });
        }

        private void ToggleNotesPanel()
        {
            this.IsSelectingNotes = !this.IsSelectingNotes;
        }

        private void UpdateSelectedNotes()
        {
            SelectedNotes = string.Join(", ", this.Notes.Where(n => n.Checked).Select(n => n.Title));
        }

        private async Task UpdateAsicInfo()
        {
            ShowActivityIndicator("Loading Asic Info...");

            try
            {
                var credential =
                    await _apiClient.GetCredential(AsicId, AuthManager.AuthToken);
                _credential = credential ?? new CredentialDto();

                SetCredential(_credential);

                DismissActivityIndicator();
                if (credential == null)
                {
                    ShowDialog(
                        "Cannot find any Asic info associated with the ID, please enter manually or search with another ID",
                        "No ASIC Info");
                }
            }
            catch (Exception)
            {
                DismissActivityIndicator();
                ShowMessage("Failed to retrieve Asic info. Please try again.", "OK");
            }
        }

        private void InitYearsAndMonths()
        {
            var startingYear = DateTime.UtcNow.AddYears(-1).Year;
            var endingYear = DateTime.UtcNow.AddYears(20).Year;
            for (var i = startingYear; i < endingYear; i++)
            {
                Years.Add(new CheckedItem() { Id = i.ToString(), Title = i.ToString() });
            }

            var months = Enumerable.Range(1, 12)
                .Select(i => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i));
            foreach (var month in months)
            {
                Months.Add(new CheckedItem() { Id = (months.IndexOf(month) + 1).ToString(), Title = month });
            }
        }

        private void InitVehicleTypes()
        {
            VehicleTypes.Add(new CheckedItem { Id = "Bus", Title = "Bus" });
            VehicleTypes.Add(new CheckedItem { Id = "Car", Title = "Car" });
            VehicleTypes.Add(new CheckedItem { Id = "Airfield Equipment", Title = "Airfield Equipment" });
            VehicleTypes.Add(new CheckedItem { Id = "Tug", Title = "Tug" });
            VehicleTypes.Add(new CheckedItem { Id = "Truck", Title = "Truck" });
        }

        private void InitAirportCodes()
        {
            Airports = new ObservableCollection<CheckedItem>
            {
                new CheckedItem { Id = "MEL", Title = "MEL" },
                new CheckedItem { Id = "AUS", Title = "AUS" }
            };
        }

        private void ToggleSecurityTierItem(SecurityTierItem item)
        {
            item?.ToggleResult();

            if (item.CheckType.Equals("ExtraDetailsSecurityCheckSecurityScanPolicyContainer",
                StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(item.Result) && item.Result != item.DefaultResult)
            {
                IsVehicleDetailsDialogVisible = true;
            }
        }

        private bool _isVehicleDetailsDialogVisible = false;

        public bool IsVehicleDetailsDialogVisible
        {
            get => _isVehicleDetailsDialogVisible;
            set
            {
                _isVehicleDetailsDialogVisible = value;
                RaisePropertyChanged(() => IsVehicleDetailsDialogVisible);
            }
        }

        private void Cancel()
        {
            ShowDialog("You are about to cancel this check, are you sure?", "No", "Yes",
                async () =>
                {
                    await NavigationService.GoBack();
                });
        }

        private async Task SubmitAsync()
        {
            UpdateExpiresDate();
            if (Expires.Equals(DateTime.MinValue))
            {
                ShowMessage("Please enter a valid expiring date", "OK");
                return;
            }

            var locationInfo = DependencyService.Get<ILocationInfo>();
            AevApp.Dependency.Location geoLocation = null;

            try
            {
                geoLocation = await locationInfo.GetLastLocation();
            }
            catch (Exception ex)
            {
                // Unable to get location
            }

            var appInfo = DependencyService.Get<IAppInfo>();
            var deviceInfo = DependencyService.Get<IDeviceInfo>();

            SecuritySubmission = new SubmitSecuritySubmissionRequest
            {
                AirportId = _appSettings.Get<int>(Constants.Airport),
                LocationId = _appSettings.Get<int>(Constants.Location),
                AsicId = AsicId,
                FirstName = FirstName,
                LastName = LastName,
                AirportCode = SelectedAirport?.Id,
                Company = Company ?? string.Empty,
                Identical = _faceIsIdentical,
                Match = _confidence,
                Pass = false,
                Note = SelectedNotes,
                SubmittedBy = AuthManager.AuthenticatedUser.Id,
                SubmittedOn = DateTime.UtcNow,
                Found = true,
                Manual = true,
                SecurityScanPolicySecurityTierGroupingId = _securityScanPolicySecurityTierGrouping.Id,
                Checks = new List<SubmitSecuritySubmissionCheck>(),
                UId = Guid.NewGuid(),
                CommencedAt = _commencedAt,
                AsicSource = _credential?.Source,
                ClientId = long.Parse(_configManager.Get(Constants.ConfigNames.ClientId)),
                ApplicationName = appInfo.ApplicationName,
                ApplicationVersion = appInfo.ApplicationVersion,
                ApplicationBuild = appInfo.ApplicationBuild,
                CardImageSource = "Camera",
                CardScanOriginalOrientation = null,
                DeviceIdentifier = deviceInfo.DeviceIdentifier,
                DeviceName = deviceInfo.DeviceName,
                DeviceOperationSystem = deviceInfo.DeviceOperatingSystem,
                DeviceOperationSystemVersion = deviceInfo.DeviceOperatingSystemVersion,
                DeviceBrand = deviceInfo.DeviceBrand,
                DeviceModel = deviceInfo.DeviceModel,
                LocationProvider = geoLocation?.Provider,
                LocationLatitude = geoLocation?.Latitude,
                LocationLongitude = geoLocation?.Longitude,
                LocationTimestamp = geoLocation?.Timestamp,
                LocationAccuracy = geoLocation?.Accuracy
            };

            // Maybe test the current screen "mode" 
            if (_credential is AsicDto)
            {
                SecuritySubmission.Expires = new DateTime(Expires.Year, Expires.Month, 1);
                SecuritySubmission.AsicEdited = (_credential as AsicDto)?.Edited;
                SecuritySubmission.CredentialType = "ASIC";
                SecuritySubmission.Suspended = (_credential as AsicDto).Suspended;
                SecuritySubmission.AsicStatusCodeId = (_credential as AsicDto).Status?.Id;
            }
            else if (_credential is VisitorPassDto)
            {
                SecuritySubmission.Expires = null;
                SecuritySubmission.AsicEdited = null;
                SecuritySubmission.CredentialType = "VisitorPass";
                SecuritySubmission.Suspended = null;
            }

            if (Suspended)
            {
                SecuritySubmission.Pass = false;
            }
            else if (SecurityCheckItems != null && !_asicId.ToUpper().StartsWith("VP"))
            {
                foreach (var securityCheckItem in SecurityCheckItems)
                {
                    var submitSecuritySubmissionCheck = new SubmitSecuritySubmissionCheck
                    {
                        Id = securityCheckItem.Id,
                        SecurityCheckId = securityCheckItem.SecurityCheckId,
                        Pass = securityCheckItem.Result.Equals("Pass", StringComparison.OrdinalIgnoreCase),
                        Result = securityCheckItem.Result,
                        Manual = securityCheckItem.Manual,
                        SecurityScanPolicySecurityTierSecurityCheckId = securityCheckItem.Id
                    };

                    if (securityCheckItem.CheckType.Equals("ExtraDetailsSecurityCheckSecurityScanPolicyContainer",
                            StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(securityCheckItem.Result) &&
                        securityCheckItem.Result != securityCheckItem.DefaultResult)
                    {
                        submitSecuritySubmissionCheck.ExtraData = new List<SubmitSecuritySubmissionCheckExtraData>();

                        if (!(string.IsNullOrWhiteSpace(SelectedVehicleTypeValue) ||
                              SelectedVehicleTypeValue.Equals("Vehicle Type",
                                  StringComparison.InvariantCultureIgnoreCase)))
                        {
                            submitSecuritySubmissionCheck.ExtraData.Add(new SubmitSecuritySubmissionCheckExtraData { ExtraDataKey = "VehicleDetails.VehicleType", Value = SelectedVehicleTypeValue });
                        }

                        if (!string.IsNullOrWhiteSpace(VehicleRego))
                        {
                            submitSecuritySubmissionCheck.ExtraData.Add(new SubmitSecuritySubmissionCheckExtraData
                                {ExtraDataKey = "VehicleDetails.RegistrationNumber", Value = VehicleRego});
                        }
                    }

                    SecuritySubmission.Checks.Add(submitSecuritySubmissionCheck);
                }

                SecuritySubmission.Pass = !SecuritySubmission.Checks.Any(c => c.Result.Equals("Fail", StringComparison.OrdinalIgnoreCase));
            }


            if (!SecuritySubmission.Pass)
            {
                DismissActivityIndicator();
                //Submit a failed check
                ShowDialog("Are you sure you wish to submit a failed security submission?",
                    "No", "Yes", async () =>
                    {
                        await DoSubmissionAsync();
                    });
            }
            else
            {
                await DoSubmissionAsync();
            }
        }

        private void UpdateExpiresDate()
        {
            if (SelectedYear?.Title == null || SelectedMonth?.Title == null)
            {
                Expires = DateTime.MinValue;
            }
            else
            {
                var selectedDate = $"{SelectedYear.Title} {SelectedMonth.Title} 01";
                Expires = DateTime.Parse(selectedDate);
            }
        }

        private async Task DoSubmissionAsync()
        {
            SumitSecuritySubmission result;
            try
            {
                ShowActivityIndicator("Submitting security submission, please wait...");

                SecuritySubmission.UploadedOn = DateTimeOffset.Now;

                result = await _apiClient.SubmitSecurityCheckAsync(SecuritySubmission, AuthManager.AuthToken);

                DismissActivityIndicator();
            }
            catch (Exception ex)
            {
                DismissActivityIndicator();
                ShowMessage("Security submission failed, please try again", "OK");
                return;
            }

            var successfullyUploadedImages = await SubmitImages(SecuritySubmission.UId.Value);

            var navArg = new SubmissionResultNavArg()
            {
                SubmissionResult = successfullyUploadedImages
                    ? "Submission was successful"
                    : "Submission was successful but failed to upload images with 3 attempts."
            };

            await NavigationService.NavigateTo(typeof(RestartAgainPage), navArg, true);
        }

        private async Task<bool> SubmitImages(Guid submissionUid)
        {
            ShowActivityIndicator("Uploading images, please wait...");

            var successful = false;
            var attempts = 0;

            while (!successful && attempts < 3)
            {
                successful = await DoSubmitImages(submissionUid);
                attempts++;
            }

            DismissActivityIndicator();
            return successful;
        }

        private async Task<bool> DoSubmitImages(Guid submissionUid)
        {
            try
            {
                var airportId = _appSettings.Get<int>(Constants.Airport);
                await _apiClient.UploadFaceAsync(GetImageBytes(PersonPhotoImageSource), submissionUid, AuthManager.AuthToken);
                await _apiClient.UploadFrontAsync(GetImageBytes(AsicPhotoImageSource), submissionUid, AuthManager.AuthToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private byte[] GetImageBytes(string imageSource)
        {
            return File.ReadAllBytes(imageSource);
        }

        public override async Task OnNavigated(INavigationArgs parameter)
        {
            if (!(parameter is SecurityCheckSubmissionNavArg arg))
                return;

            _commencedAt = arg.CommencedAt;
            _credential = arg.Credential;
            PersonPhotoImageSource = arg.PersonImageSource;
            AsicPhotoImageSource = arg.AsicImageSource;
        }

        public override async Task OnAppearing()
        {
            SetCredential(_credential);
            await GetSecurityTier();
            //Get confidence
            FaceMatchConfidence = await GetFaceMatchingConfidence();
        }

        public void SetCredential(CredentialDto credential)
        {
            if (_credential is AsicDto)
            {
                SetAsic(_credential as AsicDto);
            }
            if (_credential is VisitorPassDto)
            {
                SetVisitorPass(_credential as VisitorPassDto);
            }
        }

        private void SetAsic(AsicDto asic)
        {
            AsicId = asic.CredentialId;
            FirstName = asic.FirstName;
            LastName = asic.LastName;
            SelectedAirport = Airports.SingleOrDefault(i => i.Id == asic.AirportCode);
            Expires = asic.Expires;
            Company = asic.Company;
            Suspended = asic.Suspended;
            SetYearAndMonth();
        }

        private void SetVisitorPass(VisitorPassDto visitorPass)
        {
            AsicId = visitorPass.CredentialId;
            FirstName = visitorPass.FirstName;
            LastName = visitorPass.LastName;
            SelectedAirport = null;
            Expires = DateTime.MinValue; // No expiry month and year.
            // ValidTo = 
            Company = visitorPass.Company;
            Suspended = false; // No concept of suspended yet.
            SelectedMonth = null;
            SelectedYear = null;
        }

        private void SetYearAndMonth()
        {
            if (Expires == DateTime.MinValue)
                return;

            var year = Expires.Year;
            this.SelectedYear = this.Years.SingleOrDefault(y => y.Id == year.ToString());

            var month = Expires.Month;
            this.SelectedMonth = this.Months.SingleOrDefault(y => y.Id == month.ToString());
        }

        private async Task<string> GetFaceMatchingConfidence()
        {
            //#if DEBUG
            //            _faceIsIdentical = false;
            //            _confidence = 0.6511m;
            //            return "65.11%";
            //#endif
            try
            {
                var confidence = "0%";
                ShowActivityIndicator("Detecting Face and Performing Face Matching...");
                using (var personImageMemStream = GetImageMemStreamFromPath(PersonPhotoImageSource))
                using (var asicImageMemStream = GetImageMemStreamFromPath(AsicPhotoImageSource))
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(5));

                    var task1 = Task.Run(async () => await _faceService.DetectFaceAsync(personImageMemStream), cts.Token);
                    var task2 = Task.Run(async () => await _faceService.DetectFaceAsync(asicImageMemStream), cts.Token);

                    var faces = await Task.WhenAll(task1, task2);
                    var personFace = faces[0];
                    var asicFace = faces[1];
                    if (personFace.face.FirstOrDefault()?.FaceId != null && asicFace.face.FirstOrDefault()?.FaceId != null)
                    {
                        var verifyResult =
                            await _faceService.VerifyFaceAsync(personFace.face[0].FaceId, asicFace.face[0].FaceId);

                        confidence = $"{Math.Round(verifyResult.Confidence * 100)}%";
                        _faceIsIdentical = verifyResult.IsIdentical;
                        _confidence = (decimal)verifyResult.Confidence;
                        UpdateFaceSecurityCheck(verifyResult.Confidence);
                    }

                    DismissActivityIndicator();

                    return confidence;
                }
            }
            catch (Exception e)
            {
                DismissActivityIndicator();
                ShowMessage($"Facial matching failed...Error: {e.Message}", "OK");
                return "0%";
            }
        }

        private MemoryStream GetImageMemStreamFromPath(string imageSource)
        {
            var bytes = File.ReadAllBytes(imageSource);
            return new MemoryStream(bytes);
        }

        private async Task GetSecurityTier()
        {
            try
            {
                var securityScanPolicyDto = await _apiClient.GetLocationCurrentSecurityPolicyCompiled(_appSettings.Get<int>(Constants.Location), AuthManager.AuthToken);

                _securityScanPolicySecurityTierGrouping = securityScanPolicyDto?.TierGroupings?.FirstOrDefault();

                TierName = "Security Checks";

                var checks =
                    _securityScanPolicySecurityTierGrouping.SecurityScanPolicySecurityTiers.SelectMany(t =>
                        t.SecurityChecks);

                SecurityCheckItems = new ObservableCollection<SecurityTierItem>();

                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (var check in checks)
                    {
                        if (check.Type.Equals("SecurityCheckSecurityScanPolicyContainer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            SecurityCheckItems.Add(
                                new SecurityTierItem
                                {
                                    Id = check.Id,
                                    CheckType = check.Type,
                                    Title = check.SecurityCheck.Name,
                                    Manual = true,
                                    SecurityCheckId = check.SecurityCheck.Id,
                                    DefaultResult = check.SecurityCheck.DefaultResult,
                                    ValidResults = check.SecurityCheck.ValidResults,
                                    DisplayStyle = check.DisplayStyle,
                                    Result = check.SecurityCheck.DefaultResult
                                });
                        }

                        if (check.Type.Equals("ExtraDetailsSecurityCheckSecurityScanPolicyContainer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            SecurityCheckItems.Add(
                                new SecurityTierItem
                                {
                                    Id = check.Id,
                                    CheckType = check.Type,
                                    Title = check.SecurityCheck.Name,
                                    Manual = true,
                                    SecurityCheckId = check.SecurityCheck.Id,
                                    DefaultResult = check.SecurityCheck.DefaultResult,
                                    ValidResults = check.SecurityCheck.ValidResults,
                                    DisplayStyle = check.DisplayStyle,
                                    Result = check.SecurityCheck.DefaultResult
                                });
                        }

                        if (check.Type.Equals("RandomSampleSecurityScanPolicyContainer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (check.Population != null)
                            {
                                foreach (var pop in check.Population)
                                {
                                    SecurityCheckItems.Add(
                                        new SecurityTierItem
                                        {
                                            Id = pop.Id,
                                            CheckType = check.Type,
                                            Title = pop.SecurityCheck.Name,
                                            Manual = true,
                                            SecurityCheckId = pop.SecurityCheck.Id,
                                            DefaultResult = pop.SecurityCheck.DefaultResult,
                                            ValidResults = pop.SecurityCheck.ValidResults,
                                            DisplayStyle = pop.DisplayStyle,
                                            Result = pop.SecurityCheck.DefaultResult
                                        });
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to get random security check: {ex.Message}", "OK");
                TierName = "Failed to get Security Tier";
                SecurityCheckItems = new ObservableCollection<SecurityTierItem>();
            }
        }

        private void UpdateFaceSecurityCheck(double verifyResultConfidence)
        {
            var faceToAsicItem = SecurityCheckItems.SingleOrDefault(s => s.Title.Equals("Face to ASIC"));
            if (faceToAsicItem != null)
            {
                faceToAsicItem.Result = verifyResultConfidence >= 0.5 ? "Pass" : "Fail";
                //automatic set to true, so not manual
                faceToAsicItem.Manual = verifyResultConfidence < 0.5;
            }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand HideVehicleDetailsDialogCommand { get; set; }
        public ICommand ToggleSecurityCheckCommand { get; set; }
        public ICommand UpdateAsicIdCommand { get; set; }
        public ICommand SelectNotes { get; set; }
        public ICommand ConfirmSelectedNotes { get; set; }
        public ICommand SelectYear { get; set; }
        public ICommand SelectMonth { get; set; }
        public ICommand SelectVehicleType { get; set; }
        public ICommand SelectAirport { get; set; }
        public ICommand TapNoteCommand { get; set; }
        public ICommand TapMonthCommand { get; set; }
        public ICommand TapYearCommand { get; set; }
        public ICommand TapVehicleTypeCommand { get; set; }
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

        private DateTime _expires;
        public DateTime Expires
        {
            get => _expires;
            set
            {
                if (_expires == value) return;
                _expires = value;
                RaisePropertyChanged(() => Expires);
            }
        }

        private ObservableCollection<CheckedItem> _years = new ObservableCollection<CheckedItem>();

        public ObservableCollection<CheckedItem> Years
        {
            get => _years;
            set
            {
                if (_years == value) return;
                _years = value;
                RaisePropertyChanged(() => Years);
            }
        }

        private ObservableCollection<CheckedItem> _months = new ObservableCollection<CheckedItem>();

        public ObservableCollection<CheckedItem> Months
        {
            get => _months;
            set
            {
                if (_months == value) return;
                _months = value;
                RaisePropertyChanged(() => Months);
            }
        }

        private ObservableCollection<CheckedItem> _vehicleTypes = new ObservableCollection<CheckedItem>();

        public ObservableCollection<CheckedItem> VehicleTypes
        {
            get => _vehicleTypes;
            set
            {
                if (_vehicleTypes == value) return;
                _vehicleTypes = value;
                RaisePropertyChanged(() => VehicleTypes);
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

        private CheckedItem _selectedYear;

        public CheckedItem SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear == value) return;
                _selectedYear = value;
                RaisePropertyChanged(() => SelectedYear);
                if (SelectedYear == null)
                {
                    SelectedYearValue = "Year";
                }
                else
                {
                    SelectedYearValue = value.Title;
                }
            }
        }

        private string _selectedYearValue;
        public string SelectedYearValue
        {
            get => _selectedYearValue;
            set
            {
                if (_selectedYearValue == value) return;
                _selectedYearValue = value;
                RaisePropertyChanged(() => SelectedYearValue);
            }
        }

        private CheckedItem _selectedMonth;

        public CheckedItem SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth == value) return;
                _selectedMonth = value;
                RaisePropertyChanged(() => SelectedMonth);
                if (SelectedMonth == null)
                {
                    SelectedMonthValue = "Month";
                }
                else
                {
                    SelectedMonthValue = value.Title;
                }
            }
        }

        private string _selectedMonthValue;
        public string SelectedMonthValue
        {
            get => _selectedMonthValue;
            set
            {
                if (_selectedMonthValue == value) return;
                _selectedMonthValue = value;
                RaisePropertyChanged(() => SelectedMonthValue);
            }
        }

        private CheckedItem _selectedVehicleType;

        public CheckedItem SelectedVehicleType
        {
            get => _selectedVehicleType;
            set
            {
                if (_selectedVehicleType == value) return;
                _selectedVehicleType = value;
                RaisePropertyChanged(() => SelectedVehicleType);
                if (SelectedVehicleType == null)
                {
                    SelectedVehicleTypeValue = "Vehicle Type";
                }
                else
                {
                    SelectedVehicleTypeValue = value.Title;
                }
            }
        }

        private string _selectedVehicleTypeValue;
        public string SelectedVehicleTypeValue
        {
            get => _selectedVehicleTypeValue;
            set
            {
                if (_selectedVehicleTypeValue == value) return;
                _selectedVehicleTypeValue = value;
                RaisePropertyChanged(() => SelectedVehicleTypeValue);
            }
        }

        private string _vehicleRego;
        public string VehicleRego
        {
            get => _vehicleRego;
            set
            {
                if (_vehicleRego == value) return;
                _vehicleRego = value;
                RaisePropertyChanged(() => VehicleRego);
            }
        }

        private CheckedItem _selectedAirport;
        public CheckedItem SelectedAirport
        {
            get => _selectedAirport;
            set
            {
                if (_selectedAirport == value) return;
                _selectedAirport = value;
                RaisePropertyChanged(() => SelectedAirport);
                if (SelectedAirport == null)
                {
                    SelectedAirportValue = "Airport";
                }
                else
                {
                    SelectedAirportValue = value.Title;
                }
            }
        }

        private string _selectedAirportValue;
        public string SelectedAirportValue
        {
            get => _selectedAirportValue;
            set
            {
                if (_selectedAirportValue == value) return;
                _selectedAirportValue = value;
                RaisePropertyChanged(() => SelectedAirportValue);
            }
        }


        private string _faceMatchConfidence;
        public string FaceMatchConfidence
        {
            get => _faceMatchConfidence;
            set
            {
                if (_faceMatchConfidence == value) return;
                _faceMatchConfidence = value;
                RaisePropertyChanged(() => FaceMatchConfidence);
            }
        }

        private string _personPhotoImageSource;
        public string PersonPhotoImageSource
        {
            get => _personPhotoImageSource;
            set
            {
                if (_personPhotoImageSource == value) return;

                _personPhotoImageSource = value;
                RaisePropertyChanged(() => PersonPhotoImageSource);
            }
        }

        private string _asicPhotoImageSource;
        public string AsicPhotoImageSource
        {
            get => _asicPhotoImageSource;
            set
            {
                if (_asicPhotoImageSource == value) return;

                _asicPhotoImageSource = value;
                RaisePropertyChanged(() => AsicPhotoImageSource);
            }
        }

        private string _tierName;
        public string TierName
        {
            get => _tierName;
            set
            {
                if (_tierName == value) return;

                _tierName = value;
                RaisePropertyChanged(() => TierName);
            }
        }

        private ObservableCollection<SecurityTierItem> _securityTierItems = new ObservableCollection<SecurityTierItem>();

        public ObservableCollection<SecurityTierItem> SecurityCheckItems
        {
            get => _securityTierItems;
            set
            {
                if (_securityTierItems == value) return;

                _securityTierItems = value;
                RaisePropertyChanged(() => SecurityCheckItems);
            }
        }

        private ObservableCollection<CheckedItem> _notes = new ObservableCollection<CheckedItem>();

        public ObservableCollection<CheckedItem> Notes
        {
            get => _notes;
            set
            {
                if (_notes == value) return;

                _notes = value;
                RaisePropertyChanged(() => Notes);
            }
        }

        private string _selectedNotes = string.Empty;

        public string SelectedNotes
        {
            get => _selectedNotes;
            set
            {
                if (_selectedNotes == value) return;

                _selectedNotes = value;
                RaisePropertyChanged(() => SelectedNotes);
            }
        }

        private bool _isSelectingNotes;

        public bool IsSelectingNotes
        {
            get => _isSelectingNotes;
            set
            {
                if (_isSelectingNotes == value) return;

                _isSelectingNotes = value;
                RaisePropertyChanged(() => IsSelectingNotes);
            }
        }

        private bool _isSelectingYear;

        public bool IsSelectingYear
        {
            get => _isSelectingYear;
            set
            {
                if (_isSelectingYear == value) return;

                _isSelectingYear = value;
                RaisePropertyChanged(() => IsSelectingYear);
            }
        }

        private bool _isSelectingMonth;

        public bool IsSelectingMonth
        {
            get => _isSelectingMonth;
            set
            {
                if (_isSelectingMonth == value) return;

                _isSelectingMonth = value;
                RaisePropertyChanged(() => IsSelectingMonth);
            }
        }

        private bool _isSelectingVehicleType;

        public bool IsSelectingVehicleType
        {
            get => _isSelectingVehicleType;
            set
            {
                if (_isSelectingVehicleType == value) return;

                _isSelectingVehicleType = value;
                RaisePropertyChanged(() => IsSelectingVehicleType);
            }
        }

        private bool _isSelectingAirport;

        public bool IsSelectingAiport
        {
            get => _isSelectingAirport;
            set
            {
                if (_isSelectingAirport == value) return;

                _isSelectingAirport = value;
                RaisePropertyChanged(() => IsSelectingAiport);
            }
        }

        private bool _suspended;

        public bool Suspended
        {
            get => _suspended;
            set
            {
                if (_suspended == value) return;

                _suspended = value;
                RaisePropertyChanged(() => Suspended);
            }
        }
    }
}