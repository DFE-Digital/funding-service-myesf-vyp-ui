using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Web.Models;

namespace ViewYourPayments.Web.Services.Interfaces
{
    public interface IPaymentsService
    {
        /// <summary>
        /// Gets the payments for a selected provider (UKPRN) and date range.
        /// </summary>
        /// <param name="ukPrn">Provider identifier.</param>
        /// <param name="startDate">Date from which payments are required, must be used together with DateTo.</param>
        /// <param name="endDate">Date until which payments are required, must be used together with DateTo.</param>
        /// <param name="pageNumber">Requested page number.</param>
        /// <returns>Returns PaymentSummaryResultViewModel.</returns>

        Task<PaymentSummaryResultViewModel> GetPaymentSummaries(string ukPrn, DateTime startDate, DateTime endDate, int pageNumber);
        /// <summary>
        /// Get payment detail for selected payment identifier made to a provider (UKPRN).
        /// </summary>
        /// <param name="ukprn">Provider Identifier-UKPRN.</param>
        /// <param name="paymentIdentifier">Payment Identifier.</param>
        /// <returns>Returns PaymentDetailsResultViewModel.</returns>
        Task<PaymentDetailsResultViewModel> GetPaymentDetails(string ukPrn, int paymentIdentifier);

        /// <summary>
        /// Gets the transactions for a provider (UKPRN) for a selected criteria.
        /// </summary>
        /// <param name="searchCriteria"> Search criteria for selected request.</param>
        /// <returns>Returns PaymentTransactionResultViewModel.</returns>
        Task<PaymentTransactionResultViewModel> GetPaymentTransactions(PaymentTransactionSearchCriteria searchCriteria);

        /// <summary>
        /// Gets the transactions for a provider (UKPRN) and date range.
        /// </summary>
        /// <param name="ukPrn">Provider Identifier-UKPRN.</param>
        /// <param name="dateFrom">Date from which payments are required, must be used together with DateTo.</param>
        /// <param name="dateTo">Date until which payments are required,, must be used together with DateFrom.</param>
        /// <returns>List of unique transaction descripitons for selected criteria.</returns>
        Task<IEnumerable<string>> GetUniqueTransactionDescriptions(string ukPrn, DateTime startDate, DateTime endDate);
    }
}
