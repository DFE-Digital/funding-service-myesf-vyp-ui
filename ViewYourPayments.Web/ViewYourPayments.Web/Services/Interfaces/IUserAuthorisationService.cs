using System.Security.Claims;
using System.Threading.Tasks;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Services.Interfaces
{
    public interface IUserAuthorisationService
    {
        PaymentsApplicationSession GetValueFromCookies(string keyName);
        void UpsertApplicationCookie(string keyName, PaymentsApplicationSession applicationSession);
        Task<bool> UpdateClaimWithUkprnAndProviderName(ClaimsPrincipal claimsPrincipal);
    }
}
