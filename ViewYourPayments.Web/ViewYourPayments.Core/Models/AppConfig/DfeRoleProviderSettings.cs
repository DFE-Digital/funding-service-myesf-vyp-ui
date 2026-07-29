namespace ViewYourPayments.Core.Models.AppConfig
{
    public class DfeRoleProviderSettings
    {
        public string OidcClientId { get; set; }
        public string OidcClientSecret { get; set; }
        public string OidcAudience { get; set; }
        public string DfeSignInRolesApiUrl { get; set; }
    }
}
