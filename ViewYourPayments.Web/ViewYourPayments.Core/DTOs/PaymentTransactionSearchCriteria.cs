using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentTransactionSearchCriteria
    {
        /// <summary>
        /// Provider Identifier-UKPRN.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// Date from which transactions are required, must be used together with end date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date until which transactions are required,, must be used together with start date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Page number (optional). If not specified.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Page size (optional). If not specified.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Primary sort field(optional). Default is PaymentDate.
        /// </summary>
        public PaymentTransactionSortFields PrimarySortField { get; set; } = PaymentTransactionSortFields.PaymentDate;

        /// <summary>
        /// Sort direction field(optional). Default is Descending.
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        /// <summary>
        /// Selected search terms by user.
        /// </summary>
        public string[] SearchTerms { get; set; } = Array.Empty<string>();
    }
}
