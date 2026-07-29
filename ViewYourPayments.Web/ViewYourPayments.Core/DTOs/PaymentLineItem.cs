namespace ViewYourPayments.Core.DTOs
{
    public class PaymentLineItem
    {
        public string PaymentLineIdentifier { get; set; }

        public DateTime PaymentDate { get; set; }

        public string ContractNumber { get; set; }

        public string BudgetDescription { get; set; }

        public string PaymentLineDescription { get; set; }

        public decimal PaymentLineAmount { get; set; }
    }
}
