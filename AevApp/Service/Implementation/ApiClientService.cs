using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AevApp.Helper.Interface;
using AevApp.Model;
using AevApp.Model.Requests;
using AevApp.Model.Responses;
using AevApp.Service.Interface;
using Newtonsoft.Json;

namespace AevApp.Service.Implementation
{
    public class ApiClientService : BaseApiClientService, IApiClientService
    {
        public ApiClientService(IConfigManager configManager) : base(configManager)
        {
        }

        public async Task<string> GetAsync(string apiUrl, string authToken = "")
        {
            using (var httpClient = new HttpClient(HttpMessageHandler))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                SetDefaultHeaders(httpClient);
                AddAuthTokenIfNeeded(authToken, httpClient);
                try
                {
                    apiUrl = GetApiEndpoint(apiUrl);
                    var httpResult = await httpClient.GetAsync(apiUrl);
                    httpResult.EnsureSuccessStatusCode();
                    return await httpResult.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Internal Server Error", ex);
                }
            }
        }

        public async Task<T> GetAsync<T>(string apiUrl, string authToken = "")
        {
            using (var httpClient = new HttpClient(HttpMessageHandler))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                SetDefaultHeaders(httpClient);
                AddAuthTokenIfNeeded(authToken, httpClient);
                try
                {
                    apiUrl = GetApiEndpoint(apiUrl);
                    var httpResult = await httpClient.GetAsync(apiUrl);
                    httpResult.EnsureSuccessStatusCode();
                    return await httpResult.Content.ReadAsAsync<T>();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Internal Server Error", ex);
                }
            }
        }

        public async Task PostAsync(string apiUrl, object payload, string authToken = "")
        {
            using (HttpClient httpClient = new HttpClient(HttpMessageHandler))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(90); //set timeout to 1.5 minute
                SetDefaultHeaders(httpClient);
                AddAuthTokenIfNeeded(authToken, httpClient);
                try
                {
                    await PostAsync(apiUrl, payload, httpClient);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Internal Server Error", ex);
                }
            }
        }

        public async Task<T> PostAsync<T>(string apiUrl, object payload, string authToken = "")
        {
            using (HttpClient httpClient = new HttpClient(HttpMessageHandler))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(90); //set timeout to 1.5 minute
                SetDefaultHeaders(httpClient);
                AddAuthTokenIfNeeded(authToken, httpClient);
                try
                {
                    var response = await PostAsync(apiUrl, payload, httpClient);
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Internal Server Error", ex);
                }
            }
        }

        private async Task<HttpResponseMessage> PostAsync(string apiUrl, object payload, HttpClient httpClient)
        {
            apiUrl = GetApiEndpoint(apiUrl);
            var messageContent = JsonConvert.SerializeObject(payload);
            var message = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(messageContent, Encoding.UTF8, "application/json")
            };

            var httpResult = await httpClient.SendAsync(message);

            httpResult.EnsureSuccessStatusCode();

            return httpResult;
        }

        public async Task<string> UploadFile(string apiUrl, string filePath)
        {
            apiUrl = GetApiEndpoint(apiUrl);
            //read file into upfilebytes array
            var upfilebytes = File.ReadAllBytes(filePath);

            var fileName = Path.GetFileName(filePath);

            //create new HttpClient and MultipartFormDataContent and add our file, and StudentId
            using (HttpClient client = new HttpClient(HttpMessageHandler))
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
                content.Add(baContent, "File", fileName);

                //upload MultipartFormDataContent content async and store response in response var
                var httpResult = await client.PostAsync(apiUrl, content);

                httpResult.EnsureSuccessStatusCode();
                //read response result as a string async into json var
                var responsestr = await httpResult.Content.ReadAsAsync<string>();
                return responsestr;
            }
        }

        public async Task<AccessTokenResponse> LoginAsync(string username, string password)
        {
            var loginUrl = GetApiEndpoint(Constants.Uris.Login);
            using (var httpClient = new HttpClient(HttpMessageHandler))
            {
                var response = await httpClient.PostAsJsonAsync(loginUrl,
                    new AccessTokenRequest() { ClientId = ClientId, ClientSecret = ClientSecret, Username = username, Password = password, GrantType = "password" });

                response.EnsureSuccessStatusCode();

                var loginResponse = await response.Content.ReadAsAsync<AccessTokenResponse>();

                return loginResponse;
            }
        }

        public async Task<UserDto> GetProfile(string authToken)
        {
            var asicInfo = await GetAsync<UserDto>(GetApiEndpoint($"/security/user/current"), authToken);
            return asicInfo;
        }

        public async Task<CredentialDto> GetCredential(string credentialId, string authToken)
        {
            var response = (await GetAsync(GetApiEndpoint($"/credential/{credentialId}"), authToken));

            var credentials = JsonConvert.DeserializeObject<CollectionDtoWrapper<CredentialDto>>(response, new JsonSerializerSettings()
            {
                Converters = { new CredentialConverter() }
            });

            return credentials.Items.FirstOrDefault();
        }

        public async Task<SecurityScanPolicyDto> GetLocationCurrentSecurityPolicyCompiled(int locationId, string authToken)
        {
            var securityTier = await GetAsync<SecurityTierResponse>(GetApiEndpoint($"/location/{locationId}/currentsecuritypolicy?compile=true"), authToken);
            return securityTier.Result;
        }

        public async Task<List<SecurityScanPolicySecurityTierSecurityCheckDto>> GetSecurityChecksAsync(int airportId, int locationId, int tierId, string authToken)
        {
            var securityChecks =
                await GetAsync<SecurityCheckResponse>(
                    GetApiEndpoint($"/security-check/get/{airportId}/{locationId}/{tierId}"), authToken);

            return securityChecks.Result;
        }

        public async Task<SumitSecuritySubmission> SubmitSecurityCheckAsync(SubmitSecuritySubmissionRequest securitySubmission, string authManagerAuthToken)
        {
            var securitySubmissionResponse =
                await PostAsync<SubmitSecuritySubmissionResponse>(
                    GetApiEndpoint("/security-submission/submit"), securitySubmission, authManagerAuthToken);

            return securitySubmissionResponse.Result;
        }

        public async Task UploadFaceAsync(byte[] data, Guid uid, string authToken)
        {
            var url = GetApiEndpoint($"/security-submission/submit/{uid}/face?uploadedAt={DateTimeOffset.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}");

            await UploadImage(data, authToken, url);
        }

        public async Task UploadFrontAsync(byte[] data, Guid uid, string authToken)
        {
            var url = GetApiEndpoint($"/security-submission/submit/{uid}/front?uploadedAt={DateTimeOffset.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}");

            await UploadImage(data, authToken, url);
        }

        public async Task<List<Location>> GetAllLocationAsync(int airportId, string authToken = "")
        {
            var url = GetApiEndpoint($"{Constants.Uris.GetAllLocations}?active=true");
            return (await GetAsync<GetLocationsResponse>(url, authToken)).Items;
        }

        public async Task<AddVpResponse> AddVpAsync(AddVpRequest addVpRequest, string authToken)
        {
            var response = await PostAsync<AddVpResponse>(GetApiEndpoint("/visitor-pass"), addVpRequest, authToken);

            return response;
        }

        private async Task UploadImage(byte[] data, string authToken, string url)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (HttpClient client = new HttpClient(HttpMessageHandler))
                {
                    //client.Timeout = TimeSpan.FromSeconds(10);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("bearer", authToken);
                    request.Content = new StreamContent(stream, 4096);

                    HttpResponseMessage response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task<ClientConfigurationDto> GetClientConfigurationDto(int clientId, string authToken)
        {
            var clientConfiguration = await GetAsync<ClientConfigurationDto>(GetApiEndpoint($"/client/{clientId}"), authToken);
            return clientConfiguration;
        }

        private void AddAuthTokenIfNeeded(string authToken, HttpClient httpClient)
        {
            if (!string.IsNullOrWhiteSpace(authToken))
            {
                AddAuthHeader(httpClient, authToken);
            }
        }
    }
}