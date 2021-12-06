namespace AevApp.Model.Requests
{
    public class AccessTokenRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string RefreshToken { get; set; }
    }
}
