using System;
using System.Collections.Generic;
using System.Linq;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Web.Models
{
    public class PaymentTransactionResultViewModel
    {
        public IEnumerable<PaymentTransactionViewModel> PaymentTransactions { get; set; } = Enumerable.Empty<PaymentTransactionViewModel>();
        public string Ukprn { get; set; }
        public string VendorNumber { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool ShouldDisplayNextPage => CurrentPage > 0 && CurrentPage < TotalPages;
        public bool ShouldDisplayPreviousPage => CurrentPage > 1;
        public string NextPageLink { get; set; }
        public string PreviousPageLink { get; set; }
        public BudgetGroupSummary BudgetGroupSummary { get; set; }
        public PaymentTransactionSortFields PrimarySortField { get; set; }
        public SortDirection PrimarySortDirection { get; set; }
    }
}
