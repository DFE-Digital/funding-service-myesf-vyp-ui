using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Models;
using ViewYourPayments.Core.Models.AppConfig;

namespace ViewYourPayments.Infrastructure
{
    public class PaymentsApiOAuthHttpClient : IPaymentsApiOAuthHttpClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<AppSettings> _appSettings;

        public PaymentsApiOAuthHttpClient(HttpClient httpClient, IOptions<AppSettings> appSetting)
        {
            _appSettings = appSetting;
            httpClient.BaseAddress = new Uri(_appSettings.Value.OAuthHttpClientSettings.BaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = httpClient;
        }

        public async Task<OAuthToken> GetToken()
        {
            var value = new Dictionary<string, string>
                     {
                        { "resource", _appSettings.Value.OAuthHttpClientSettings.Resource},
                        {"client_id",_appSettings.Value.OAuthHttpClientSettings.ClientId },
                        { "grant_type",_appSettings.Value.OAuthHttpClientSettings.GrantType},
                        { "client_secret",_appSettings.Value.OAuthHttpClientSettings.ClientSecret}
                     };
            var content = new FormUrlEncodedContent(value);
            var tokenResponse = await _client.PostAsync("oauth2/token", content);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"OAuth Token api returns {tokenResponse.StatusCode} for {_client.BaseAddress.AbsoluteUri} path");
            }

            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<OAuthToken>(jsonContent);
            return token;
        }
    }
}
