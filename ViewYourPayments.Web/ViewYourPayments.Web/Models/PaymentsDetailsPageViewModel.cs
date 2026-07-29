namespace ViewYourPayments.Web.Models
{
    public class PaymentsDetailsPageViewModel : BasePageViewModel
    {
        public PaymentsDetailsPageViewModel()
        {
            DisplayHomePageLink = true;
        }
        public string ProviderUKPRN { get; set; }
        public int PaymentIdentifier { get; set; }
        public PaymentDetailsResultViewModel Result { get; set; }
        public string BackToPaymentSummaryPageUrl { get; set; }
    }
}