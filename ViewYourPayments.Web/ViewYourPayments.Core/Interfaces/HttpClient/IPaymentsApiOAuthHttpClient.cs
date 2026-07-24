using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewYourPayments.Core.Models;

namespace ViewYourPayments.Core.Interfaces.HttpClient
{
    public interface IPaymentsApiOAuthHttpClient
    {
        Task<OAuthToken> GetToken();
    }
}
