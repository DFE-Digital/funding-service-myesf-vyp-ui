using System;

namespace ViewYourPayments.Web.Shared
{
    public class PaymentsSummaryFilter
    {
        public PaymentsSummaryFilter(DateTime fromDate, DateTime toDate, int pageNumber)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageNumber = pageNumber;
        }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public int PageNumber { get; set; }
    }
}
