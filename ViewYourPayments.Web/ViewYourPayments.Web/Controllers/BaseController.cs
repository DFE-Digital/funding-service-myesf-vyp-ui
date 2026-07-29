using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IApplicationLogger _applicationLogger;
        private readonly IUserAuthorisationService _userAuthorisationService;
        public BaseController(IApplicationLogger applicationLogger,
                                IUserAuthorisationService userAuthorisationService
                                )
        {
            _applicationLogger = applicationLogger;
            _userAuthorisationService = userAuthorisationService;
        }

        [Authorize]
        public PaymentsApplicationSession GetUserSessionDetails()
        {
            var appSession = _userAuthorisationService.GetValueFromCookies(Constants.UserSessionCookieKey);
            return appSession ?? new PaymentsApplicationSession();
        }

        protected FileResult SaveFile(byte[] fileContents, string contentType, string fileName)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(fileContents, contentType);
        }

        protected string ValidateDateRange(DateTime fromDate, DateTime toDate)
        {
            if (fromDate == DateTime.MinValue || toDate == DateTime.MinValue)
            {
                return ErrorMessages.InvalidDates;
            }
            if (fromDate > toDate)
            {
                return ErrorMessages.FromDateShouldBeBeforeTodate;
            }
            if (toDate > DateTime.UtcNow)
            {
                return ErrorMessages.FutureDateNotAllowed;
            }
            if (fromDate <= DateTime.UtcNow.AddYears(-3))
            {
                return ErrorMessages.MaxDateDuration;
            }
            return string.Empty;
        }
    }
}