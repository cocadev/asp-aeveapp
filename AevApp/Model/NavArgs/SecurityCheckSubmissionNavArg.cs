using System;
using Xamarin.SDK.Service.Interface;

namespace AevApp.Model.NavArgs
{
    public class SecurityCheckSubmissionNavArg : INavigationArgs
    {
        public DateTimeOffset CommencedAt { get; set; }
        public CredentialDto Credential { get; set; }
        public string PersonImageSource { get; set; }
        public string AsicImageSource { get; set; }
    }
}