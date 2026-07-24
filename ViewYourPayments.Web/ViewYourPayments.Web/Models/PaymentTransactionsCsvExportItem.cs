using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Models
{
    public class PaymentTransactionsCsvExportItem
    {
            public string VendorNumber { get; set; }
            public string Ukprn { get; set; }
            public string Date { get; set; }
            public string ContractNumber { get; set; }
            public string BudgetGroup { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
    }
}
