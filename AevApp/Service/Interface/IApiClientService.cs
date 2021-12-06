using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AevApp.Model;
using AevApp.Model.Requests;
using AevApp.Model.Responses;

namespace AevApp.Service.Interface
{
    public interface IApiClientService
    {
        Task<T> GetAsync<T>(string apiUrl, string authToken = "");
        Task PostAsync(string apiUrl, object payload, string authToken = "");
        Task<T> PostAsync<T>(string apiUrl, object payload, string authToken = "");
        Task<string> UploadFile(string apiUrl, string filePath);
        Task<AccessTokenResponse> LoginAsync(string username, string password);
        Task<UserDto> GetProfile(string authToken);
        Task<CredentialDto> GetCredential(string credentialId, string authToken);
        Task<SecurityScanPolicyDto> GetLocationCurrentSecurityPolicyCompiled(int locationId, string authToken);
        Task<List<SecurityScanPolicySecurityTierSecurityCheckDto>> GetSecurityChecksAsync(int airportId, int locationId, int tierId, string authManagerAuthToken);
        Task<SumitSecuritySubmission> SubmitSecurityCheckAsync(SubmitSecuritySubmissionRequest securitySubmission, string authManagerAuthToken);
        Task UploadFaceAsync(byte[] data, Guid uid, string authToken);
        Task UploadFrontAsync(byte[] data, Guid uid, string authToken);
        Task<List<Location>> GetAllLocationAsync(int airportId, string authToken = "");
        Task<AddVpResponse> AddVpAsync(AddVpRequest addVpRequest, string authToken);
        Task<ClientConfigurationDto> GetClientConfigurationDto(int clientId, string authToken);
    }
}