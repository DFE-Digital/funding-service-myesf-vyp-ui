namespace ViewYourPayments.Core.DTOs
{
    public class PaymentTransactionsResponse
    {
        public string ProviderUkprn { get; set; }
        public string ProviderFinanceVendorCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public BudgetGroupSummary BudgetGroupSummary { get; set; }
        public PaymentLineItem[] PaymentLines { get; set; } = Array.Empty<PaymentLineItem>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}
