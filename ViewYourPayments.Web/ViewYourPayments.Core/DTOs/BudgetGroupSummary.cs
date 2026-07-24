using System;

namespace ViewYourPayments.Core.DTOs
{
    public class BudgetGroupSummary
    {
        public decimal TotalAmount { get; set; }
        public BudgetGroupItem[] BudgetGroups { get; set; } = Array.Empty<BudgetGroupItem>();
    }
}
