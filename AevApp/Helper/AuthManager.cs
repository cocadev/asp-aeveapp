using AevApp.Event;
using AevApp.Model;
using GalaSoft.MvvmLight.Messaging;

namespace AevApp.Helper
{
    public class AuthManager
    {
        private LoginInfo _userInfo;

        public AuthManager(IMessenger messenger)
        {
            messenger.Register<UserAuthenticatedEvent>(this, e => SetUserInfo(e));
        }

        private void SetUserInfo(UserAuthenticatedEvent userAuthenticatedEvent)
        {
            _userInfo = userAuthenticatedEvent.User;
        }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_userInfo?.Token);

        public UserDto AuthenticatedUser => _userInfo?.Profile;

        public string UserRole
        {
            get
            {
                if (AuthenticatedUser == null)
                {
                    return "Unauthenticated";
                }

                if (!string.IsNullOrWhiteSpace(AuthenticatedUser.GlobalPermission))
                {
                    return _userInfo.GetRole(int.Parse(AuthenticatedUser.GlobalPermission));
                }

                if (!string.IsNullOrWhiteSpace(AuthenticatedUser.AirportPermission))
                {
                    return _userInfo.GetRole(int.Parse(AuthenticatedUser.AirportPermission));
                }

                return string.Empty;
            }
        }

        public string AuthToken => _userInfo.Token;

        public void Logout()
        {
            this._userInfo = null;
        }
    }
}
