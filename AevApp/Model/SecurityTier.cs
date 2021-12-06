using System;
using System.Collections.Generic;

namespace AevApp.Model
{
    public class SecurityScanPolicyDto
    {
        public int Id { get; set; }
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public int RequiredScans { get; set; }
        public int ExtensionThreshold { get; set; }
        public int ExtensionAmount { get; set; }
        public IEnumerable<SecurityScanPolicySecurityTierGroupingDto> TierGroupings { get; set; }
    }

    public class SecurityScanPolicySecurityTierGroupingDto
    {
        public int Id { get; set; }

        public int Percentage { get; set; }

        public int DisplayIndex { get; set; }

        public string Name { get; set; }

        public List<SecurityScanPolicySecurityTierDto> SecurityScanPolicySecurityTiers { get; set; }

        public bool Default { get; set; }
    }

    public class SecurityTierDto
    {
        public int Id { get; set; }

        public int AirportId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }
    }

    public class SecurityScanPolicySecurityTierDto
    {
        public int Id { get; set; }

        // Security Tier is at wrong level. Should be on grouping.
        public SecurityTierDto SecurityTier { get; set; }

        public int DisplayIndex { get; set; }

        public string Name { get; set; }

        public List<SecurityScanPolicyContainerDto> SecurityChecks { get; set; }
    }

    public class SecurityScanPolicyContainerDto
    {
        public string Type { get; set; }

        public int DisplayIndex { get; set; }

        public string DisplayStyle { get; set; }

        public SecurityCheckDto SecurityCheck { get; set; }

        public List<SecurityScanPolicyContainerDto> Population { get; set; }

        public int Id { get; set; }
    }

    public class SecurityTierResponse
    {
        public SecurityScanPolicyDto Result { get; set; }
    }
}