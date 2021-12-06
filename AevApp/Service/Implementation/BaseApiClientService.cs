using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AevApp.Helper.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AevApp.Service.Implementation
{
    public class BaseApiClientService
    {
        public HttpMessageHandler HttpMessageHandler { get; set; }
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public BaseApiClientService(IConfigManager configManager)
        {
            ApiBaseUrl = configManager.Get(Constants.ConfigNames.ApiBaseUrl);
            ClientId = configManager.Get(Constants.ConfigNames.ClientId);
            ClientSecret = configManager.Get(Constants.ConfigNames.ClientSecret);
        }

        protected virtual void SetDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected virtual void AddAuthHeader(HttpClient httpClient, string authToken)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authToken);
        }

        protected virtual string GetApiEndpoint(string apiUrl)
        {
            if (!apiUrl.ToLower().StartsWith("http"))
            {
                apiUrl = ApiBaseUrl + apiUrl;
            }

            return apiUrl;
        }
    }
}