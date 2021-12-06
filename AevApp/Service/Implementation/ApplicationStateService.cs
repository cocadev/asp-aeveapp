using AevApp.Event;
using AevApp.Service.Interface;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.SDK.Service.Interface;

namespace AevApp.Service.Implementation
{
    public class ApplicationStateService : IApplicationStateService
    {
        private readonly IMessenger _messenger;
        private readonly INavigationService _navigationService;

        public ApplicationStateService(INavigationService navigationService, IMessenger messenger)
        {
            _messenger = messenger;
            _navigationService = navigationService;
        }

        public void HandleOnResume()
        {
            _messenger.Send(new AppResumedEvent());
        }
    }
}
