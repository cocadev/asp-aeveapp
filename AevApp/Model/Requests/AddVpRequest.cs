using System;

namespace AevApp.Model.Requests
{
    public class AddVpRequest
    {
        public string VpId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IdentificationNumber { get; set; }

        public string Company { get; set; }

        public DateTimeOffset ValidFrom { get; set; }

        public DateTimeOffset ValidTo { get; set; }
    }

    public class AsicStatusCodeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public bool Active { get; set; }
    }
}