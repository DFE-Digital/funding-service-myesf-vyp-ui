using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Services
{
    public class UserAuthorisationService : IUserAuthorisationService
    {
        readonly IHttpContextAccessor _httpContext;
        readonly IProviderSearchService _providerSearchService;
        readonly IApplicationLogger _applicationLogger;
        private const string UkprnQueryStringName = "ukprn";

        public UserAuthorisationService(
            IHttpContextAccessor httpContext,
            IProviderSearchService provideSearchService,
            IApplicationLogger applicationLogger)
        {
            _providerSearchService = provideSearchService;
            _httpContext = httpContext;
            _applicationLogger = applicationLogger;
        }

        public PaymentsApplicationSession GetValueFromCookies(string keyName)
        {
            return _httpContext.HttpContext.GetCookieValue<PaymentsApplicationSession>(keyName);
        }

        public void UpsertApplicationCookie(string keyName, PaymentsApplicationSession applicationSession)
        {
            _httpContext.HttpContext.UpsertCookie(keyName, applicationSession);
        }

        public async Task<bool> UpdateClaimWithUkprnAndProviderName(ClaimsPrincipal principal)
        {
            var cp = (ClaimsIdentity)principal.Identity;
            var user = new ClaimsUser(principal);
            if (user.IsInternalUser)
            {
                return await UpdateInternalUser(cp, user);
            }
            return await UpdateExternalUser(cp, user);
        }

        private async Task<bool> UpdateInternalUser(ClaimsIdentity cp, ClaimsUser user)
        {
            int.TryParse(_httpContext.HttpContext.Request.Query[UkprnQueryStringName], out int ukPrnFromQuery);
            if (!ShouldUpdateUkprnForInternalUser(user, ukPrnFromQuery)) return false;
            string providerName = await GetProviderName(ukPrnFromQuery);
            if (ukPrnFromQuery > 0 && !string.IsNullOrWhiteSpace(providerName))
            {
                UpdateUkprn(cp, user, ukPrnFromQuery);
                UpdateProviderName(cp, providerName);
                return true;
            }
            return false;
        }

        private async Task<bool> UpdateExternalUser(ClaimsIdentity cp, ClaimsUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.ProviderName)) return false;
            string providerName = await GetProviderName(user.Ukprn.Value);

            if (string.IsNullOrWhiteSpace(providerName)) return false;
            UpdateProviderName(cp, providerName);

            return true;
        }

        private async Task<string> GetProviderName(int ukprn)
        {
            if (ukprn <= 0)
            {
                _applicationLogger.LogWarn($"Ukprn is not valid -{ukprn}");
                return string.Empty;
            }

            var result = await _providerSearchService.GetActiveProvidersByUkprn(ukprn);
            var providerName = result.FirstOrDefault()?.Name;
            //If provider name not found for selected ukprn then log warning
            if (string.IsNullOrWhiteSpace(providerName))
            {
                _applicationLogger.LogWarn($"Provider name is empty or null for ukprn - {ukprn}");
                return string.Empty;
            }
            return providerName;
        }

        private void UpdateProviderName(ClaimsIdentity cp, string providerName)
        {
            var providerNameClaim = cp.FindFirst(ClaimsUser.OrganisationNameClaimType);
            if (providerNameClaim != null) cp.RemoveClaim(providerNameClaim);
            cp.AddClaim(new Claim(ClaimsUser.OrganisationNameClaimType, providerName));
        }

        private bool ShouldUpdateUkprnForInternalUser(ClaimsUser userClaim, int ukPrnFromQuery)
        {
            if (IsCurrentUkPrnIsSameAsNewUkprn(userClaim.Ukprn, ukPrnFromQuery)) return false;
            //If ukprn is already in claim and no value in query string i.e no change in ukprn 
            if (userClaim.Ukprn > 0 && ukPrnFromQuery == 0) return false;
            //If there is no valid ukprn in query string then log warning  and return false
            if (!ValidateUkprnNumber(ukPrnFromQuery)) return false;
            //If claim doesn't have new ukprn and query string also doesn't have ukprn, then it need to be validated and log warning
            return true;
        }
        private bool UpdateUkprn(ClaimsIdentity cp, ClaimsUser userClaim, int ukprn)
        {
            var ukprnClaim = cp.FindFirst(ClaimsUser.UkprnClaimType);
            if (ukprnClaim != null) cp.RemoveClaim(ukprnClaim);
            cp.AddClaim(new Claim(ClaimsUser.UkprnClaimType, ukprn.ToString()));
            return true;
        }
        private bool IsCurrentUkPrnIsSameAsNewUkprn(int? existingUkPrn, int newUkprn)
        {
            //If current ukprn is same as in query string than no need to update
            return (existingUkPrn.HasValue && existingUkPrn.Value > 0 && newUkprn == existingUkPrn.Value);
        }
        private bool ValidateUkprnNumber(int? ukprnNumber)
        {
            if (!ukprnNumber.HasValue || ukprnNumber.Value <= 0)
            {
                _applicationLogger.LogWarn($"Ukprn is not valid for current session ukprn -{ukprnNumber}");
                return false;
            }
            return true;
        }
    }
}
