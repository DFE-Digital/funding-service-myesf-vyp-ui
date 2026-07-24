using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentSummaryItem
    {
        public int PaymentIdentifier { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
