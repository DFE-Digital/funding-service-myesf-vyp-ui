using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Models.AppConfig;

namespace ViewYourPayments.Infrastructure
{
    /// <summary>
    /// Payment Http client to call Payment api with single instance.
    /// </summary>
    public class PaymentsHttpClient : IPaymentsHttpClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<AppSettings> _appSettings;

        public PaymentsHttpClient(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            httpClient.BaseAddress = new Uri(_appSettings.Value.PaymentsApiHttpClientSettings.BaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSettings.Value.PaymentsApiHttpClientSettings.ApimSubscriptionKey);
            _client = httpClient;
        }

        /// <summary>
        /// Get payment detail for selected request uri.
        /// </summary>
        /// <param name="token">Auth2/Bearer token for api authentication.</param>
        /// <param name="requestUri">Uri content endpoint and params.</param>
        /// <returns>Return PaymentDetailsResponse.</returns>
        public async Task<PaymentDetailsResponse> GetPaymentDetails(string token, string requestUri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var uriCallWithParameters = Uri.UnescapeDataString(requestUri);
            var result = await _client.GetAsync(uriCallWithParameters);

            if (result.IsSuccessStatusCode)
            {
                var responseString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<PaymentDetailsResponse>(responseString);
                return response ?? new PaymentDetailsResponse();
            }
            throw new HttpRequestException($"Payment api returns {result.StatusCode} for {_client.BaseAddress.AbsoluteUri} path");
        }

        /// <summary>
        /// Gets the payment summary result from selected uri and params.
        /// </summary>
        /// <param name="token">Auth2/Bearer token for api authentication.</param>
        /// <param name="requestUri">Uri content endpoint and params.</param>
        /// <returns>Return PaymentSummaryResponse.</returns>
        public async Task<PaymentSummaryResponse> GetPaymentSummaries(string token, string requestUri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var uriCallWithParameters = Uri.UnescapeDataString(requestUri);
            var result = await _client.GetAsync(uriCallWithParameters);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentSummaryResponse>(response);
            }
            throw new HttpRequestException($"Payment api returns {result.StatusCode} for {_client.BaseAddress.AbsoluteUri} path");
        }

        /// <summary>
        /// A POST method to get payment transactions for select filter.
        /// </summary>
        /// <param name="token">Auth2/Bearer token for api authentication.</param>
        /// <param name="requestUri">Uri content endpoint and params.</param>
        /// <param name="searchCriteria">Search criteria for selected request.</param>
        /// <returns>Returns PaymentTransactionsResponse.</returns>
        public async Task<PaymentTransactionsResponse> GetPaymentTransactions(string token, string requestUri, PaymentTransactionSearchCriteria searchCriteria)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var uriCallWithParameters = Uri.UnescapeDataString(requestUri);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(searchCriteria), Encoding.UTF8, "application/json");
            var result = await _client.PostAsync(uriCallWithParameters, content);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentTransactionsResponse>(response);
            }
            throw new HttpRequestException($"Payment api returns {result.StatusCode} for {_client.BaseAddress.AbsoluteUri} path");
        }

        /// <summary>
        /// A Get method to retrieve unique transaction for selected params.
        /// </summary>
        /// <param name="token">Auth2/Bearer token for api authentication.</param>
        /// <param name="requestUri">Uri content endpoint and params.</param>
        /// <returns>Returns transaction description list.</returns>
        public async Task<IEnumerable<string>> GetUniqueTransactionsDescriptions(string token, string requestUri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var uriCallWithParameters = Uri.UnescapeDataString(requestUri);
            var result = await _client.GetAsync(uriCallWithParameters);

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<string>>(response);
            }
            throw new HttpRequestException($"Payment api returns {result.StatusCode} for {_client.BaseAddress.AbsoluteUri} path");
        }

    }
}
