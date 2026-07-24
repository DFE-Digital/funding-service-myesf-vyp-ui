using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.Models.AppConfig
{
    public class DfeSettings
    {
        public string OidcAuthority { get; set; }
        public string OidcRedirectUrl { get; set; }
        public string OidcPostLogOutUrl { get; set; }
        public string OidcClientId { get; set; }
        public string OidcClientSecret { get; set; }
        public string OidcAudience { get; set; }
        public string DfeSignInRolesapiUrl { get; set; }
        public string DfeLegacyCodeId { get; set; }
        public string DfeSignInUrl { get; set; }
    }
}
