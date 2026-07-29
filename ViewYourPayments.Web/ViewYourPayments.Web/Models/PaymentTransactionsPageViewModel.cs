namespace ViewYourPayments.Web.Models
{
    /// <summary>
    /// View model for PaymentTransactionPage view.
    /// </summary>
    public class PaymentTransactionsPageViewModel : BasePageViewModel
    {
        public PaymentTransactionsPageViewModel()
        {
            PageName = "Payment Transactions";
            DisplayHomePageLink = true;
        }
        /// <summary>
        /// Provider Identifier-UKPRN.
        /// </summary>
        public string ProviderUKPRN { get; set; }

        /// <summary>
        /// Unique payment transaction identification.
        /// </summary>
        public int PaymentIdentifier { get; set; }

        /// <summary>
        /// PaymentTransaction result.
        /// </summary>
        public PaymentTransactionResultViewModel Result { get; set; }

        /// <summary>
        /// Selected filter options to filter transactions.
        /// </summary>
        public PaymentTransactionFilterViewModel TransactionFilter { get; set; }

        /// <summary>
        /// Boolean flag to check error occurance.
        /// </summary>
        public bool HasValidationError { get; set; } = false;

        /// <summary>
        /// Validation error message to display on view if there is any error.
        /// </summary>
        public string ValidationMessage { get; set; }
    }
}