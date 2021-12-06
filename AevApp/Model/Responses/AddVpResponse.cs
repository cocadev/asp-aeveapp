using System;

namespace AevApp.Model.Responses
{
    public class AddVpResponse
    {
        public long Id { get; set; }
        public string AsicId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AirportCode { get; set; }
        public string Company { get; set; }
        public DateTime Expires { get; set; }
        public bool Suspended { get; set; }
        public bool? Active { get; set; }
    }
}