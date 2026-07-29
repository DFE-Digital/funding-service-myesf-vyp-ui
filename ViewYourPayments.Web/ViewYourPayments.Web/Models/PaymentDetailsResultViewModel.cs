using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ViewYourPayments.Web.Models
{
    public class PaymentDetailsResultViewModel
    {
        public IEnumerable<PaymentDetailViewModel> PaymentLines { get; set; } = Enumerable.Empty<PaymentDetailViewModel>();

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaymentAmount { get; set; }

        public DateTime PaymentDate { get; set; }
        public string ProviderName { get; set; }
        public string Ukprn { get; set; }
        public string VendorNumber { get; set; }
    }
}
