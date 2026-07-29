using ViewYourPayments.Core.Models;

namespace ViewYourPayments.Core.Interfaces.HttpClient
{
    public interface IPaymentsApiOAuthHttpClient
    {
        Task<OAuthToken> GetToken();
    }
}
