namespace AevApp
{
    public static class Constants
    {
        public static class ConfigNames
        {
            public const string ApiBaseUrl = "ApiBaseUrl";
            public const string ClientId = "clientId";
            public const string ClientSecret = "clientSecret";
            public const string EnvName = "EnvName";
            public const string UploadImageWebUri = "UploadImageWebUri";
            public const string AzureOcrWebUri = "AzureOcrWebUri";
            public const string AzureFaceServiceSubscriptionKey = "AzureFaceServiceSubscriptionKey";
            public const string AzureFaceServiceApiRoot = "AzureFaceServiceApiRoot";
        }

        public static class Uris
        {
            public const string Login = "/auth/token";
            public const string GetAllLocations = "/location";
        }


        public static class Orientation
        {
            public const string Portrait = "Portrait";
            public const string Landscape = "Landscape";
        }

        public const string Airport = "airport";
        public const string Location = "location";
        public const string AzureFaceServiceSubscriptionKey = "AzureFaceServiceSubscriptionKey";
        public const string AzureFaceServiceApiRoot = "AzureFaceServiceApiRoot";
    }
}