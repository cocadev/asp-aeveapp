using AevApp.Model;

namespace AevApp.Event
{
    public class UserAuthenticatedEvent
    {
        public LoginInfo User { get; set; }
    }
}