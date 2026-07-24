namespace ViewYourPayments.Core.Models.AppConfig
{
    public class OAuthHttpClientSettings
    {
        public string Resource { get; set; }
        public string ClientId { get; set; }
        public string GrantType { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
    }
}
