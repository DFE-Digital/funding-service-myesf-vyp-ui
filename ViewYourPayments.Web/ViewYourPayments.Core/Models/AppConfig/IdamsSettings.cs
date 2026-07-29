namespace ViewYourPayments.Core.Models.AppConfig
{
    public class IdamsSettings
    {
        public string ClaimsIssuer { get; set; }
        public string MetadataAddress { get; set; }
        public string CallbackPath { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string Wtrealm { get; set; }
        public string RedirectUrl { get; set; }
    }
}
