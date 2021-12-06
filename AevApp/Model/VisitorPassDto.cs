using System;

namespace AevApp.Model
{
    public class VisitorPassDto : CredentialDto
    {
        public string IdentificationNumber { get; set; }
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }
    }
}
