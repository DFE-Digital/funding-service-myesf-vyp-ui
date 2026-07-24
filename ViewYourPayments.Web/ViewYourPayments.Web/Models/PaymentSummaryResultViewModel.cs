using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ViewYourPayments.Web.Models
{
    public class PaymentSummaryResultViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<PaymentSummaryItemViewModel> PaymentSummaries { get; set; } = new List<PaymentSummaryItemViewModel>();
        public string UkprnNumber { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool ShouldDisplayNextPage => this.CurrentPage > 0 && this.CurrentPage < this.TotalPages;         
        public bool ShouldDisplayPreviousPage => this.CurrentPage > 1;        
        public string NextPageLink => $"?page={this.CurrentPage+1}";
        public string PreviousPageLink=> $"?page={this.CurrentPage - 1}";
    }
}
