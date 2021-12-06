using System;

namespace AevApp.Model
{
    public class SecuritySubmission
    {
        public int Id { get; set; }

        public int AirportId { get; set; }

        public int LocationId { get; set; }

        public int TierId { get; set; }

        public string AsicId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AirportCode { get; set; }

        public string Company { get; set; }

        public DateTime? Expires { get; set; }

        public bool Identical { get; set; }

        public decimal Match { get; set; }

        public bool Pass { get; set; }

        public bool? Suspended { get; set; }

        public string Note { get; set; }

        public int? SubmittedBy { get; set; }

        public DateTime SubmittedOn { get; set; }

        public DateTime Created { get; set; }

        public bool Found { get; set; }

        public bool Manual { get; set; }

        public bool Cancelled { get; set; }

        public long ClientId { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationVersion { get; set; }

        public string ApplicationBuild { get; set; }
    }
}