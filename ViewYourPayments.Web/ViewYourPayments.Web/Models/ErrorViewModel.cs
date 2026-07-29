namespace ViewYourPayments.Web.Models
{
    public class ErrorViewModel : BasePageViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string ErrorMessage { get; set; }
    }
}