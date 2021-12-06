using AevApp.Model.Enum;

namespace AevApp.Model
{
    public class LoginInfo
    {
        public string Token { get; set; }

        public UserDto Profile { get; set; }

        public string GetRole(int permission)
        {
            switch ((Permissions)permission)
            {
                case Permissions.ROOT:
                    return "ROOT";
                case Permissions.SUPER_ADMIN:
                    return "Super Administrator";
                case Permissions.ADMIN:
                    return "Administrator";
                case Permissions.AIRPORT_ADMIN:
                    return "Airport Administrator";
                case Permissions.AIRPORT_OFFICER:
                    return "Security Officer";
                default:
                    return null;
            }
        }
    }
}