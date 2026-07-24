using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Web.Services.Interfaces;

namespace ViewYourPayments.Web.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private IMemoryCache _memoryCache;
        private IPaymentsApiOAuthHttpClient _oAuthHttpClient;
        private readonly IApplicationLogger _applicationLogger;

        public AccessTokenService(IPaymentsApiOAuthHttpClient oAuthHttpClient,
                                IApplicationLogger applicationLogger
                                , IMemoryCache memoryCache)
        {
            _oAuthHttpClient = oAuthHttpClient;
            _applicationLogger = applicationLogger;
            _memoryCache = memoryCache;
        }
        public async Task<string> GetPaymentApiAccessToken()
        {
            const string vypApiTokenKey = "VypApiToken";
            if (!_memoryCache.TryGetValue(vypApiTokenKey, out string vypApiToken))
            {
                var token = await _oAuthHttpClient.GetToken();
                _memoryCache.Set(vypApiTokenKey, token.AccessToken, new DateTimeOffset(DateTime.Now.AddSeconds(token.ExpiresIn - 60)));
                vypApiToken = token.AccessToken;
            }
            return vypApiToken;
        }
    }
}
