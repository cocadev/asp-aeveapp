using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AevApp.Helper;
using AevApp.Helper.Interface;
using AevApp.Model;
using AevApp.Model.Enum;
using AevApp.Model.NavArgs;
using AevApp.Service.Interface;
using AevApp.View;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.SDK.Helper.Interface;
using Xamarin.SDK.Service.Interface;

namespace AevApp.ViewModel
{
    public class OfficerDashboardViewModel : OfficerViewModelBase
    {
        private readonly IAppSettings _appSettings;
        private readonly IConfigManager _configManager;
        private readonly IApiClientService _apiClient;
        private DateTimeOffset _commencedAt;

        public OfficerDashboardViewModel(IMessenger messenger, INavigationService navigationService, IAppDialogService appDialogService,
            AuthManager authManager, IAppSettings appSettings, IConfigManager configManager, IApiClientService apiClient)
            : base(messenger, navigationService, appDialogService, authManager)
        {
            _appSettings = appSettings;
            _configManager = configManager;
            _apiClient = apiClient;
            ManualScanCommand = new RelayCommand(TakePhotoInApp);
            GetAsicInfoCommand = new RelayCommand(async () => await GetCredentialAsync());
            CancelCommand = new RelayCommand(async () => await CancelAsync());
            SavePhotoCommand = new RelayCommand<Photo>(async p => await SavePhotoAsync(p));
            TriggerPhotoTakingCommand = new RelayCommand(TriggerPhotoTaking);
            AddVpCommand = new RelayCommand(async () => await AddVp());
            SwitchFlashCommand = new RelayCommand(SwitchFlash);
            ResetCommand = new RelayCommand(ConfirmResetForm);
        }

        private void ConfirmResetForm()
        {
            ShowDialog("Are you sure you want to restart?", action: () =>
            {
                IsTakingPhoto = false;
                ResetForm();
            });
        }

        private void SwitchFlash()
        {
            //TODO: flash auto
            FlashSwitcher = !FlashSwitcher;
            FlashCurrentState = FlashSwitcher ? FlashState.On : FlashState.Off;
        }

        private async Task AddVp()
        {
            await NavigationService.NavigateTo(typeof(AddVpPage));
        }

        private void TriggerPhotoTaking()
        {
            CameraCurrentState = CameraButtonState.Taking;
            //Update the value will trigger a property change in camera custom renderer, which will call TakePhoto
            PhotoTrigger++;
        }

        private async Task SavePhotoAsync(Photo photo)
        {
            switch (photo.Index)
            {
                case 0:
                    PersonPhotoImageSource = SavePhoto("face", photo.Data);
                    CameraCurrentState = CameraButtonState.Complete;
                    await Task.Delay(500);
                    CameraCurrentState = CameraButtonState.Default;
                    UpdateCurrentStage(2);
                    PhotoInstruction = "IDENTIFICATION";
                    break;
                case 1:
                    AsicPhotoImageSource = SavePhoto("id", photo.Data);
                    CameraCurrentState = CameraButtonState.Complete;
                    await Task.Delay(500);
                    CameraCurrentState = CameraButtonState.Default;
                    UpdateCurrentStage(3);
                    IsTakingPhoto = false;
                    PhotoTaken = true;
                    PhotoInstruction = string.Empty;
                    break;
                default:
                    IsTakingPhoto = false;
                    break;
            }
        }

        private void UpdateCurrentStage(int stage)
        {
            ProgressImage = $"progressBar{stage}.png";
        }

        private string SavePhoto(string filename, byte[] data)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"{filename}.jpg");
            File.WriteAllBytes(fileName, data);
            return fileName;
        }

        private void TakePhotoInApp()
        {
            _commencedAt = DateTimeOffset.Now;

            UpdateCurrentStage(1);
            IsTakingPhoto = true;
            PhotoInstruction = "PERSON'S FACE";
        }

        private async Task CancelAsync()
        {
            ShowDialog("Are you sure you want to retake photos?", "No", "Yes", ResetForm);
        }

        private void ResetForm()
        {
            PhotoTaken = false;
            PersonPhotoImageSource = string.Empty;
            AsicPhotoImageSource = string.Empty;
            IsAsicIdValid = true;
            AsicId = string.Empty;
        }

        private async Task ManualScanAsync()
        {
            var useSampleimage = _configManager.GetBoolean("useSampleImages");
            if (useSampleimage)
            {
                PersonPhotoImageSource =
                    "/data/user/0/com.companyname.AevApp/files/face.jpg";
                AsicPhotoImageSource =
                    "/data/user/0/com.companyname.AevApp/files/id.jpg";
            }
            else
            {
                await InitCrossMedia();
                //Take photo of the person
                var personImage = await TakeAPhotoAsync();
                if (string.IsNullOrEmpty(personImage))
                {
                    await AppDialogService.ShowMessage("Failed to take a photo of the person", "Error");
                }
                PersonPhotoImageSource = personImage;

                //Take photo of the Asic Id
                var asicImage = await TakeAPhotoAsync();
                if (string.IsNullOrEmpty(asicImage))
                {
                    await AppDialogService.ShowMessage("Failed to take a photo of the ASIC ID", "Error");
                }
                AsicPhotoImageSource = asicImage;
            }

            PhotoTaken = true;
        }

        private async Task GetCredentialAsync()
        {
#if DEBUG
            //AsicId = string.IsNullOrWhiteSpace(AsicId) ? "ML64156" : AsicId;
#endif
            if (!ValidateInput())
                return;
            try
            {
                ShowActivityIndicator("Loading Asic Info...");
                var credential =
                    await _apiClient.GetCredential(AsicId, AuthManager.AuthToken);

                if (Global.IsPortrait)
                {
                    await NavigationService.NavigateTo(typeof(SecurityCheckSubmissionPage_Portrait),
                        new SecurityCheckSubmissionNavArg
                        {
                            CommencedAt = _commencedAt,
                            Credential = credential ?? new CredentialDto(),
                            PersonImageSource = PersonPhotoImageSource,
                            AsicImageSource = AsicPhotoImageSource
                        });
                }
                else
                {
                    await NavigationService.NavigateTo(typeof(SecurityCheckSubmissionPage),
                        new SecurityCheckSubmissionNavArg
                        {
                            CommencedAt = _commencedAt,
                            Credential = credential ?? new CredentialDto(),
                            PersonImageSource = PersonPhotoImageSource,
                            AsicImageSource = AsicPhotoImageSource
                        });
                }
                await Task.Delay(1000);
                ResetForm();
            }
            catch (Exception ex)
            {
                await AppDialogService.ShowError($"Error Reason: {ex.Message}. Please try again.",
                    "Error", "OK", () => { });
            }
            finally
            {
                DismissActivityIndicator();
            }
        }

        private bool ValidateInput()
        {
            IsAsicIdValid = true;

            if (string.IsNullOrWhiteSpace(AsicId))
            {
                IsAsicIdValid = false;
                return false;
            }

            return true;
        }

        private async Task<string> TakeAPhotoAsync()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return string.Empty;
            }

            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(
                    new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                    {
                        SaveToAlbum = false,
                        PhotoSize = PhotoSize.Small
                    });

                return file == null ? string.Empty : file.Path;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static async Task InitCrossMedia()
        {
            await CrossMedia.Current.Initialize();
        }

        public ICommand ManualScanCommand { get; set; }
        public ICommand GetAsicInfoCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SavePhotoCommand { get; set; }
        public ICommand TriggerPhotoTakingCommand { get; set; }
        public ICommand AddVpCommand { get; set; }
        public ICommand SwitchFlashCommand { get; set; }
        public ICommand ResetCommand { get; set; }

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

        private bool _photoTaken;

        public bool PhotoTaken
        {
            get => _photoTaken;
            set
            {
                if (_photoTaken == value) return;

                _photoTaken = value;
                RaisePropertyChanged(() => PhotoTaken);
            }
        }

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

        private bool _isAsicIdValid = true;

        public bool IsAsicIdValid
        {
            get => _isAsicIdValid;
            set
            {
                if (_isAsicIdValid == value) return;

                _isAsicIdValid = value;
                RaisePropertyChanged(() => IsAsicIdValid);
            }
        }

        private bool _isTakingPhoto;

        public bool IsTakingPhoto
        {
            get => _isTakingPhoto;
            set
            {
                if (_isTakingPhoto == value) return;

                _isTakingPhoto = value;
                RaisePropertyChanged(() => IsTakingPhoto);
            }
        }

        private bool _flashSwitcher = false;

        public bool FlashSwitcher
        {
            get => _flashSwitcher;
            set
            {
                if (_flashSwitcher == value) return;

                _flashSwitcher = value;
                RaisePropertyChanged(() => FlashSwitcher);
            }
        }

        private string _photoInstruction;
        public string PhotoInstruction
        {
            get => _photoInstruction;
            set
            {
                if (_photoInstruction == value) return;

                _photoInstruction = value;
                RaisePropertyChanged(() => PhotoInstruction);
            }
        }

        /// <summary>
        /// This is used for an external button to trigger taking a photo inside the custom renderer
        /// </summary>
        private int _photoTrigger;
        public int PhotoTrigger
        {
            get => _photoTrigger;
            set
            {
                if (_photoTrigger == value) return;
                _photoTrigger = value;
                RaisePropertyChanged(() => PhotoTrigger);
            }
        }

        private CameraButtonState _cameraCurrentState;
        public CameraButtonState CameraCurrentState
        {
            get => _cameraCurrentState;
            set
            {
                if (_cameraCurrentState == value) return;
                _cameraCurrentState = value;
                RaisePropertyChanged(() => CameraCurrentState);
                ShowCompleteCameraBorder = value == CameraButtonState.Complete;
                ShowDefaultCameraBorder = value == CameraButtonState.Default;
                ShowTakingCameraBorder = value == CameraButtonState.Taking;
            }
        }

        private FlashState _flashCurrentState = FlashState.Off;
        public string FlashCurrentStateDisplay => _flashCurrentState.ToString();
        public FlashState FlashCurrentState
        {
            get => _flashCurrentState;
            set
            {
                if (_flashCurrentState == value) return;
                _flashCurrentState = value;
                RaisePropertyChanged(() => FlashCurrentState);
                //Also update display
                RaisePropertyChanged(() => FlashCurrentStateDisplay);
            }
        }

        private string _progressImage = "progressBar1.png";
        public string ProgressImage
        {
            get => _progressImage;
            set
            {
                if (_progressImage == value) return;
                _progressImage = value;
                RaisePropertyChanged(() => ProgressImage);
            }
        }

        private bool _showDefaultCameraBorder = true;
        public bool ShowDefaultCameraBorder
        {
            get => _showDefaultCameraBorder;
            set
            {
                if (_showDefaultCameraBorder == value) return;
                _showDefaultCameraBorder = value;
                RaisePropertyChanged(() => ShowDefaultCameraBorder);
            }
        }

        private bool _showTakingCameraBorder;
        public bool ShowTakingCameraBorder
        {
            get => _showTakingCameraBorder;
            set
            {
                if (_showTakingCameraBorder == value) return;
                _showTakingCameraBorder = value;
                RaisePropertyChanged(() => ShowTakingCameraBorder);
            }
        }

        private bool _showCompleteCameraBorder;
        public bool ShowCompleteCameraBorder
        {
            get => _showCompleteCameraBorder;
            set
            {
                if (_showCompleteCameraBorder == value) return;
                _showCompleteCameraBorder = value;
                RaisePropertyChanged(() => ShowCompleteCameraBorder);
            }
        }
    }
}