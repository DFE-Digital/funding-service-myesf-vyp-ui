using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentSummaryResponse
    {
        public string ProviderUKPRN { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public PaymentSummaryItem[] PaymentSummaries { get; set; } = Array.Empty<PaymentSummaryItem>();
    }
}
