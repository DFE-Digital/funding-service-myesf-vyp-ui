namespace ViewYourPayments.Web.Models
{
    /// <summary>
    /// Model to hold filter options for transaction filter.
    /// </summary>
    public class PaymentTransactionFilterViewModel
    {
        /// <summary>
        /// Date range model.
        /// </summary>
        public DatePickerViewModel DateRange { get; set; }

        /// <summary>
        /// Search terms string delimited by coma(,).
        /// </summary>
        public string SearchTerms { get; set; }

        /// <summary>
        /// Selectd search terms.
        /// </summary>
        public string CurrentSearchTermsKeyValuePairs { get; set; }  ="[]";

        /// <summary>
        /// Absolue URL to get unique transaction descriptions used in FastSelect.
        /// </summary>
        public string GetUniqueTransctionDescriptionsUrl { get; set; }
    }
}
