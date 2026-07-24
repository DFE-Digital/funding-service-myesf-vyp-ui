using System;
using System.Linq;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Enums;
using System.Collections.Generic;

namespace ViewYourPayments.Web.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IPaymentsHttpClient _paymentsHttpClient;
        private readonly IApplicationLogger _applicationLogger;
        private readonly IAccessTokenService _accessTokenService;
        private const string dateFormat = "yyyy/MM/dd";

        public PaymentsService(IPaymentsHttpClient paymentsHttpClient,
                                IApplicationLogger applicationLogger,
                                IAccessTokenService accessTokenService)
        {
            _paymentsHttpClient = paymentsHttpClient;
            _applicationLogger = applicationLogger;
            _accessTokenService = accessTokenService;
        }

        /// <summary>
        /// Retruns payment summaries for seleted search criteria.
        /// </summary>
        /// <param name="ukPrn">Provider identifer ukprn.</param>
        /// <param name="startDate">Date from which payments are required.</param>
        /// <param name="endDate">Date until which payments are required.</param>
        /// <param name="page">Page number.</param>
        /// <returns>Returns payment summary result view model.</returns>
        public async Task<PaymentSummaryResultViewModel> GetPaymentSummaries(string ukPrn, DateTime startDate, DateTime endDate, int page)
        {
            var token = await _accessTokenService.GetPaymentApiAccessToken();
            _applicationLogger.LogInfo("successfully received token");
            var request = $"Provider/{ukPrn}/Payments?dateFrom={startDate.ToString(dateFormat)}&dateTo={endDate.ToString(dateFormat)}&page={page}";
            var response = await _paymentsHttpClient.GetPaymentSummaries(token, request);
            _applicationLogger.LogInfo("successfully received payment response");

            var summaryItems = new PaymentSummaryResultViewModel
            {
                DateFrom = response.DateFrom,
                DateTo = response.DateTo,
                UkprnNumber = ukPrn,
                CurrentPage = response.CurrentPage,
                TotalPages = response.TotalPages,
                TotalRecords = response.TotalRecords,
                PaymentSummaries = response.PaymentSummaries.Select(p => new PaymentSummaryItemViewModel
                {
                    PaymentIdentifier = p.PaymentIdentifier,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate
                }).ToList()
            };
            return summaryItems;
        }

        /// <summary>
        /// Get payment detail for selected payment identifier made to a provider (UKPRN)
        /// </summary>
        /// <param name="ukPrn">Provider identifier-UKPRN.</param>
        /// <param name="paymentIdentifier">Payment identifier.</param>
        /// <returns>Returns PaymentDetailsResultViewModel.</returns>
        public async Task<PaymentDetailsResultViewModel> GetPaymentDetails(string ukPrn, int paymentIdentifier)
        {
            var response = await GetPaymentDetailResults(ukPrn, paymentIdentifier);

            if (!response.PaymentLines.Any())
            {
                return new PaymentDetailsResultViewModel();
            }

            return new PaymentDetailsResultViewModel
            {
                PaymentLines = response.PaymentLines.Select(p => new PaymentDetailViewModel
                {
                    BudgetDescription = p.BudgetDescription,
                    ContractNumber = p.ContractNumber,
                    PaymentLineAmount = p.PaymentLineAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentLineDescription = p.PaymentLineDescription,
                    PaymentLineIdentifier = p.PaymentLineIdentifier
                }),
                PaymentAmount = response.PaymentAmount.Value,
                PaymentDate = response.PaymentDate.Value,
                VendorNumber = response.ProviderFinanceVendorIdentifier,
                Ukprn = response.ProviderUkprn
            };
        }

        /// <summary>
        /// Gets the transactions for a provider (UKPRN) for a selected criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria for selected request.</param>
        /// <returns>Returns PaymentTransactionResultViewModel.</returns>
        public async Task<PaymentTransactionResultViewModel> GetPaymentTransactions(PaymentTransactionSearchCriteria searchCriteria)
        {
            var token = await _accessTokenService.GetPaymentApiAccessToken();
            _applicationLogger.LogInfo("successfully received token");

            var request = $"Provider/Transactions";
            var response = await _paymentsHttpClient.GetPaymentTransactions(token, request, searchCriteria);
            _applicationLogger.LogInfo("successfully received payment response");

            return   new PaymentTransactionResultViewModel
            {
                DateFrom = response.DateFrom,
                DateTo = response.DateTo,
                CurrentPage = response.CurrentPage,
                TotalRecords = response.TotalRecords,
                TotalPages = (int)Math.Ceiling((double)response.TotalRecords / response.PageSize),
                Ukprn = response.ProviderUkprn, 
                BudgetGroupSummary = response.BudgetGroupSummary,
                VendorNumber = response.ProviderFinanceVendorCode,
                PrimarySortField = searchCriteria.PrimarySortField,
                PrimarySortDirection =searchCriteria.SortDirection,
                PaymentTransactions = response.PaymentLines.Select(p => new PaymentTransactionViewModel
                {
                    BudgetDescription = p.BudgetDescription,
                    ContractNumber = p.ContractNumber,
                    PaymentLineAmount = p.PaymentLineAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentLineDescription = p.PaymentLineDescription,
                    PaymentLineIdentifier = p.PaymentLineIdentifier
                }).ToList()
            };
        }

        /// <summary>
        /// Get unique transaction description for selected search criteria.
        /// </summary>
        /// <param name="ukPrn">Provider identifer ukprn.</param>
        /// <param name="startDate">Date from which payments are required.</param>
        /// <param name="endDate">Date until which payments are required.</param>
        /// <returns>Returns list of unique transaction descripitons.</returns>
        public async Task<IEnumerable<string>> GetUniqueTransactionDescriptions(string ukPrn, DateTime startDate, DateTime endDate)
        {
            var token = await _accessTokenService.GetPaymentApiAccessToken();
            _applicationLogger.LogInfo("successfully received token");
            var request = $"Provider/{ukPrn}/UniqueTransactionDescriptions?dateFrom={startDate.ToString(dateFormat)}" +
                                $"&dateTo={endDate.ToString(dateFormat)}";
            var response = await _paymentsHttpClient.GetUniqueTransactionsDescriptions(token, request);
            _applicationLogger.LogInfo("successfully received payment response");
            return response;
        }


        private async Task<PaymentDetailsResponse> GetPaymentDetailResults(string ukPrn, int paymentIdentifier)
        {
            var token = await _accessTokenService.GetPaymentApiAccessToken();
            _applicationLogger.LogInfo("successfully received token");
            var request = $"Provider/{ukPrn}/Payment/{paymentIdentifier}";
            var response = await _paymentsHttpClient.GetPaymentDetails(token, request);
            _applicationLogger.LogInfo("successfully received payment details response");
            return response;
        }
    }
}
