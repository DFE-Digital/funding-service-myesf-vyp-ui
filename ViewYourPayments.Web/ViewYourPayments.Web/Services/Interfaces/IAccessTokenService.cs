using System.Threading.Tasks;

namespace ViewYourPayments.Web.Services.Interfaces
{
    public interface IAccessTokenService
    {
        Task<string> GetPaymentApiAccessToken();
    }
}
