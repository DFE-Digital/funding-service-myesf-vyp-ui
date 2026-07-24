using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.Models.AppConfig
{
    public class PaymentsApiHttpClientSettings
    {
        public string ApimSubscriptionKey { get; set; }
        public string BaseAddress { get; set; }
    }
}
