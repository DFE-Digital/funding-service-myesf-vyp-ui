using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ViewYourPayments.Web.Models
{
    public class DatePickerViewModel
    {
        public DatePickerViewModel()
        {

        }
        public DatePickerViewModel(DateTime startDate, DateTime endDate)
        {
            StartDateYear = startDate.Year;
            StartDateMonth = startDate.Month;
            StartDateDay = startDate.Day;
            EndDateYear = endDate.Year;
            EndDateMonth = endDate.Month;
            EndDateDay = endDate.Day;
        }
        [BindProperty(Name = "start-date-year", SupportsGet = true)]
        public int StartDateYear { get; set; }
        [BindProperty(Name = "start-date-month", SupportsGet = true)]
        public int StartDateMonth { get; set; }
        [BindProperty(Name = "start-date-day", SupportsGet = true)]
        public int StartDateDay { get; set; }
        [BindProperty(Name = "end-date-year", SupportsGet = true)]
        public int EndDateYear { get; set; }
        [BindProperty(Name = "end-date-month", SupportsGet = true)]
        public int EndDateMonth { get; set; }
        [BindProperty(Name = "end-date-day", SupportsGet = true)]
        public int EndDateDay { get; set; }
        public int InitialDurationInDays { get; set; }
        public string ControllerName { get; set; }
        public string FilterAction { get; set; }
        public string ResetAction => "Index";
        public string ControlHeader { get; set; } = "Filter by date";
        public string ControlPrompt { get; set; } = "You can view payments for 3 previous calendar years";

        public IEnumerable<SelectListItem> YearRange
        {
            get
            {
                var currentYear = DateTime.UtcNow.Year;
                return new[] {
                    new SelectListItem(currentYear.ToString(), currentYear.ToString()),
                    new SelectListItem((currentYear-1).ToString(), (currentYear-1).ToString()),
                    new SelectListItem((currentYear-2).ToString(), (currentYear-2).ToString()),
                    new SelectListItem((currentYear-3).ToString(), (currentYear-3).ToString())
                    };
            }
        }
    }
}
