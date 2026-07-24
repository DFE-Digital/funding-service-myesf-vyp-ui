using System;

namespace ViewYourPayments.Web.Models
{
    public class PaymentSummaryItemViewModel
    {
        public int PaymentIdentifier { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

    }
}