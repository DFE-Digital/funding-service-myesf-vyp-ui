namespace ViewYourPayments.Core.DTOs
{
    public class PaymentDetailsResponse
    {
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ProviderUkprn { get; set; }
        public string ProviderFinanceVendorIdentifier { get; set; }
        public PaymentLineItem[] PaymentLines { get; set; } = Array.Empty<PaymentLineItem>();
    }
}
