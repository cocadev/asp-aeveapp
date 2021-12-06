using System;
using System.Collections.Generic;

namespace AevApp.Model.Requests
{
    public class SubmitSecuritySubmissionRequest
    {
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

        public bool Found { get; set; }

        public bool Manual { get; set; }

        public bool Cancelled { get; set; }

        public List<SubmitSecuritySubmissionCheck> Checks { get; set; }

        public int? SecurityScanId { get; set; }

        public int SecurityScanPolicySecurityTierGroupingId { get; set; }

        public Guid? TemplateID { get; set; }

        public string TemplateName { get; set; }

        public Guid? UId { get; set; }

        public DateTimeOffset? UploadedOn { get; set; }

        public int? UploadAttempts { get; set; }

        public DateTimeOffset? ReceivedAt { get; set; }

        public bool? UsedDefaultSecurityScanPolicySecurityTierGrouping { get; set; }

        public int? AsicStatusCodeId { get; set; }

        public DateTimeOffset? CommencedAt { get; set; }

        public string AsicSource { get; set; }

        public bool? AsicEdited { get; set; }

        public string CredentialType { get; set; }

        public long ClientId { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationVersion { get; set; }

        public string ApplicationBuild { get; set; }

        public string CardImageSource { get; set; }

        public string CardScanOriginalOrientation { get; set; }

        public string DeviceIdentifier { get; set; }

        public string DeviceName { get; set; }

        public string DeviceOperationSystem { get; set; }

        public string DeviceOperationSystemVersion { get; set; }

        public string DeviceBrand { get; set; }

        public string DeviceModel { get; set; }

        public string LocationProvider { get; set; }

        public decimal? LocationLatitude { get; set; }

        public decimal? LocationLongitude { get; set; }

        public DateTimeOffset? LocationTimestamp { get; set; }

        public decimal? LocationAccuracy { get; set; }

        public string LocationServiceState { get; set; }

        public decimal? LocationAltitude { get; set; }

        public decimal? LocationAltitudeAccuracy { get; set; }

        public decimal? LocationBearing { get; set; }

        public decimal? LocationBearingAccuracy { get; set; }

        public decimal? LocationSpeed { get; set; }

        public decimal? LocationSpeedAccuracy { get; set; }
    }

    public class SubmitSecuritySubmissionCheck
    {
        public int Id { get; set; }

        public int SecurityCheckId { get; set; }

        public int SecurityScanPolicySecurityTierSecurityCheckId { get; set; }

        public bool Pass { get; set; }

        public string Result { get; set; }

        public bool Manual { get; set; }

        public List<SubmitSecuritySubmissionCheckExtraData> ExtraData { get; set; }
    }

    public class SubmitSecuritySubmissionCheckExtraData
    {
        public string ExtraDataKey { get; set; }

        public string Value { get; set; }
    }
}