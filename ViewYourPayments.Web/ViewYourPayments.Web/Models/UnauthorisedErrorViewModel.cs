using System.Collections.Generic;

namespace ViewYourPayments.Web.Models
{
    public class UnauthorisedErrorViewModel : BasePageViewModel
    {
        public IEnumerable<string> requiredRoles { get; internal set; }

        public string DfeSignInUrl { get; set; }
    }
}
