namespace ViewYourPayments.Web.Models
{
    public class PaymentSummaryPageViewModel : BasePageViewModel
    {
        public PaymentSummaryResultViewModel Result { get; set; }
        public DatePickerViewModel DateRange { get; set; }
        public bool HasValidationError { get; set; } = false;
        public string ValidationMessage { get; set; }
    }
}
