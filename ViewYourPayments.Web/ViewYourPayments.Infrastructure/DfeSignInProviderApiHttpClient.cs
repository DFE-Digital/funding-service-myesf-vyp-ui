using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Models.AppConfig;

namespace ViewYourPayments.Infrastructure
{
    public class DfeSignInProviderApiHttpClient : IDfeSignInProviderApiHttpClient
    {
        private readonly IApplicationLogger _applicationLogger;
        private readonly HttpClient _client;
        private readonly IOptions<AppSettings> _appSettings;

        public DfeSignInProviderApiHttpClient(IApplicationLogger applicationLogger, HttpClient httpClient, IOptions<AppSettings> appSetting)
        {
            _applicationLogger = applicationLogger;
            _appSettings = appSetting;
            _client = httpClient;
            _client.BaseAddress = new Uri(_appSettings.Value.DfeRoleProviderSettings.DfeSignInRolesApiUrl);
        }
        public async Task<T> Get<T>(string token, string requestUri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Dfe signin api returns {response.StatusCode} for {_client.BaseAddress.AbsoluteUri}/{requestUri} path");
            }
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }
    }
}
