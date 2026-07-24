using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Shared
{
    public class PaymentsApplicationSession
    {
        public PaymentsSummaryFilter PaymentsSummaryFilter { get; set; }

        public PaymentsTransactionsFilter PaymentTransactionsFilter { get; set; }
    }
}
