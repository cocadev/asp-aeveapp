using System;

namespace AevApp.Model
{
    public class AsicDto : CredentialDto
    {
        public string AirportCode { get; set; }
        public DateTime Expires { get; set; }
        public bool Suspended { get; set; }
        public AsicStatusCodeDto Status { get; set; }
        public bool? Active { get; set; }
        public bool? Edited { get; set; }
    }
}
