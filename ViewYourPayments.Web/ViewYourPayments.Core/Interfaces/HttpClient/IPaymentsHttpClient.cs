using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;

namespace ViewYourPayments.Core.Interfaces.HttpClient
{
    public interface IPaymentsHttpClient
    {
        Task<PaymentSummaryResponse> GetPaymentSummaries(string token, string requestUri);
        Task<PaymentDetailsResponse> GetPaymentDetails(string token, string requestUri);
        Task<PaymentTransactionsResponse> GetPaymentTransactions(string token, string requestUri, PaymentTransactionSearchCriteria searchCriteria);
        Task<IEnumerable<string>> GetUniqueTransactionsDescriptions(string token, string requestUri);
    }
}
