using System.Collections.Generic;

namespace AevApp.Model
{
    public class SecurityScanPolicySecurityTierSecurityCheckDto
    {
        public int Id { get; set; }

        public int AirportId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public bool Active { get; set; }

        public int SecurityScanPolicySecurityTierId { get; set; }

        public int DisplayIndex { get; set; }

        public string DisplayStyle { get; set; }

        public int SecurityCheckId { get; set; }

        public SecurityCheckDto SecurityCheck { get; set; }
    }

    public class SecurityCheckDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public bool? Active { get; set; }

        public string DefaultResult { get; set; }

        public List<string> ValidResults { get; set; }
    }

    public class SecurityCheckResponse
    {
        public List<SecurityScanPolicySecurityTierSecurityCheckDto> Result { get; set; }
    }
}