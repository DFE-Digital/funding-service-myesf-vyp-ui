using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViewYourPayments.Web.Models
{
    public class PaymentTransactionViewModel
    {       
        public string PaymentLineIdentifier { get; set; }

        public DateTime PaymentDate { get; set; }

        public string ContractNumber { get; set; }

        public string BudgetDescription { get; set; }

        public string PaymentLineDescription { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaymentLineAmount { get; set; }
    }
}