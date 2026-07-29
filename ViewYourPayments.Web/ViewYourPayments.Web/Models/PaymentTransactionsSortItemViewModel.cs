using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Web.Models
{
    public class PaymentTransactionsSortItemViewModel
    {
        public PaymentTransactionSortFields PrimarySortField { get; set; }
        public SortDirection SortDirection { get; set; }
        public PaymentTransactionSortFields CurrentField { get; set; }

        public bool DisplayAscending => PrimarySortField != CurrentField || SortDirection == SortDirection.Descending;
        public bool DisplayDescending => PrimarySortField != CurrentField || SortDirection == SortDirection.Ascending;
        public string SortByAscendingURL => $"?fieldName={CurrentField}&sortDirection={SortDirection.Ascending}";
        public string SortByDescendingURL => $"?fieldName={CurrentField}&sortDirection={SortDirection.Descending}";
    }
}
