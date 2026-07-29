using System;
using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Web.Shared
{
    public class PaymentsTransactionsFilter
    {
        public PaymentsTransactionsFilter(DateTime fromDate, DateTime toDate, int pageNumber)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageNumber = pageNumber;
        }
        /// <summary>
        /// Start date to filter records.
        /// </summary>
        public DateTime FromDate { get; }

        /// <summary>
        /// End date to filter records.
        /// </summary>
        public DateTime ToDate { get; }

        /// <summary>
        /// Page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Selected primary sort field. By default it is PaymentDate.
        /// </summary>
        public PaymentTransactionSortFields PrimarySortField { get; set; } = PaymentTransactionSortFields.PaymentDate;

        /// <summary>
        /// Selected sort direction. By default it is descending.
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        /// <summary>
        /// String containing multiple search terms with coma(,) as delimiter.
        /// </summary>
        public string SearchTerms { get; set; } = string.Empty;
    }
}
