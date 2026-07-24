using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Shared
{
    public static class ErrorMessages
    {
        public const string MaxDateDuration = "Please enter dates within the past 3 years";
        public const string FutureDateNotAllowed = "Future dates are not allowed";
        public const string FromDateShouldBeBeforeTodate = "From date must be before to date";
        public const string InvalidDates = "Invalid dates - please review";
    }

}
