using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Services.Interfaces
{
    public interface IAccessTokenService
    {
        Task<string> GetPaymentApiAccessToken();
    }
}
