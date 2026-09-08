using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Interfaces.User;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.Fds;
using ViewYourPayments.Core.Models.User;

namespace ViewYourPayments.Core.Services
{
    public class FdsService : IFdsService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IApplicationLogger _applicationLogger;

        public FdsService(HttpClient httpClient, IOptions<AppSettings> appSettings, IApplicationLogger applicationLogger)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _applicationLogger = applicationLogger;

            _httpClient.BaseAddress = new Uri(_appSettings.Value.FdsApiHttpClientSettings.BaseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _appSettings.Value.FdsApiHttpClientSettings.ApimSubscriptionKey);
        }

        public async Task<IProvider> GetProvider(string ukprn)
        {
            var paymentOrganisation = await SendServiceRequest(ServiceConstants.LearningProviderQueryPath, ServiceConstants.PaymentOrgUkprn, ServiceConstants.FieldOperatorEquals, ukprn);

            if (paymentOrganisation.Count() > 0)
            {
                return paymentOrganisation.SingleOrDefault();
            }

            var providers = await SendServiceRequest(ServiceConstants.LearningProviderQueryPath, ServiceConstants.Ukprn, ServiceConstants.FieldOperatorEquals, ukprn);

            if (providers.Count() > 0)
            {
                return providers.SingleOrDefault();
            }

            return null;
        }

        private static List<SearchCriteria> BuildSearchCriteria(string searchFieldName, string searchValue, string searchOperator = ServiceConstants.FieldOperatorEquals)
        {
            var searchCriteria = new SearchCriteria()
            {
                FieldName = searchFieldName,
                Value = searchValue,
                Operator = searchOperator
            };

            return new List<SearchCriteria> { searchCriteria };
        }

        private static ServiceRequest BuildServiceRequest(List<List<SearchCriteria>> searchCriteria, int pageNumber, int pageSize, bool skipPaging, string sortColumnField)
        {
            var request = new ServiceRequest();
            if (searchCriteria.Any())
            {
                request.SearchCriteria = searchCriteria;
            }

            if (!skipPaging)
            {
                request.PageNumber = pageNumber;
                request.PageSize = pageSize;
            }

            request.SortColumn = sortColumnField;

            request.SkipPaging = skipPaging;

            return request;
        }

        private async Task<IEnumerable<IProvider>> SendServiceRequest(string url, string fieldName, string fieldOperatorComparer = null, string value = null, bool isStatusFieldRequired = true, bool skipPaging = false, int pageNumber = 0, int pageSize = 100)
        {
            var searchCriteriaList = new List<List<SearchCriteria>>();

            searchCriteriaList.Add(BuildSearchCriteria(fieldName, value, fieldOperatorComparer));

            if (fieldName == ServiceConstants.PaymentOrgName)
            {
                searchCriteriaList.Add(BuildSearchCriteria(ServiceConstants.PaymentOrgUkprn, null, ServiceConstants.FieldOperatorIsNotNull));
            }
            else if (fieldName == ServiceConstants.ProviderName)
            {
                searchCriteriaList.Add(BuildSearchCriteria(ServiceConstants.Ukprn, null, ServiceConstants.FieldOperatorIsNotNull));
            }

            if (isStatusFieldRequired)
            {
                searchCriteriaList.Add(BuildSearchCriteria(ServiceConstants.Status, ServiceConstants.OpenOrgStatus, ServiceConstants.FieldOperatorContains));
            }

            var request = BuildServiceRequest(searchCriteriaList, pageNumber, pageSize, skipPaging, fieldName);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            _applicationLogger.LogInfo($"Calling FDS api with the request {_httpClient.BaseAddress.AbsoluteUri}{url} with the request {JsonConvert.SerializeObject(request)}");

            var result = await _httpClient.PostAsync(url, content);

            if (result.IsSuccessStatusCode)
            {
                JObject obj = JObject.Parse(await result.Content.ReadAsStringAsync());

                var response = obj["data"]?.ToObject<IEnumerable<Provider>>();

                _applicationLogger.LogInfo($"Total records {response.Count()} received");

                return response;
            }

            _applicationLogger.LogException(new HttpRequestException($"FDS service returns {result.StatusCode} for {_httpClient.BaseAddress.AbsoluteUri}{url} with request {JsonConvert.SerializeObject(request)}"));

            throw new HttpRequestException($"FDS service returns {result.StatusCode}");
        }
    }
}
