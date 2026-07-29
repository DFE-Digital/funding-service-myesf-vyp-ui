using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Filters;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    [Authorize]
    public class PaymentSummaryController : BaseController
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IPaymentsService _paymentsService;
        private readonly IUserAuthorisationService _userLoginService;
        private readonly IApplicationLogger _applicationLogger;


        public PaymentSummaryController(IOptions<AppSettings> appSettings,
                                IApplicationLogger applicationLogger,
                                 IPaymentsService paymentsService,
                                 IUserAuthorisationService userLoginService
                                ) : base(applicationLogger, userLoginService)
        {
            _appSettings = appSettings;
            _paymentsService = paymentsService;
            _userLoginService = userLoginService;
            _applicationLogger = applicationLogger;
        }

        [HttpPost, HttpGet]
        [AuthenticateUser]
        public async Task<IActionResult> Index([FromQuery] string ukPrn = "", [FromForm] DatePickerViewModel datePicker = null, [FromQuery] int page = 0, bool isPageReset = false)
        {
            var user = new ClaimsUser(HttpContext.User);
            var appSession = GetUserSessionDetails();
            var paymentsFilter = appSession?.PaymentsSummaryFilter;

            if (Request.Method.Equals(HttpMethods.Post))
            {
                paymentsFilter = SetFilterDateFromUserInput(datePicker, page);

            }
            else if (ShouldSetFilterDateFromSession(paymentsFilter, isPageReset))
            {
                datePicker = new DatePickerViewModel(paymentsFilter.FromDate, paymentsFilter.ToDate);
                paymentsFilter.PageNumber = page > 0 ? page : paymentsFilter.PageNumber;

            }
            else
            {
                paymentsFilter = new PaymentsSummaryFilter(
                            DateTime.UtcNow.AddDays(-1 * _appSettings.Value.InitialPaymentsRecordsDuration), DateTime.UtcNow, page);
                datePicker = new DatePickerViewModel(paymentsFilter.FromDate, paymentsFilter.ToDate);
            }

            appSession.PaymentsSummaryFilter = paymentsFilter;

            return await GetPaymentSummaryData(datePicker, appSession, user);
        }


        [HttpPost]
        public IActionResult Reset()
        {
            return RedirectToAction("Index", new { isPageReset = true });
        }

        private bool ShouldSetFilterDateFromSession(PaymentsSummaryFilter paymentFilter, bool isPageReset)
        {
            return !isPageReset && paymentFilter != null; //user has requested another page
        }

        private static PaymentsSummaryFilter SetFilterDateFromUserInput(DatePickerViewModel datePicker, int pageNumber)
        {
            DateTime.TryParse($"{datePicker.StartDateYear}/" +
                            $"{datePicker.StartDateMonth}/" +
                            $"{datePicker.StartDateDay}", out DateTime startDate);

            DateTime.TryParse($"{datePicker.EndDateYear}/" +
            $"{datePicker.EndDateMonth}/" +
            $"{datePicker.EndDateDay}", out DateTime endDate);

            return new PaymentsSummaryFilter(startDate, endDate, pageNumber);
        }

        private async Task<IActionResult> GetPaymentSummaryData(DatePickerViewModel datePicker, PaymentsApplicationSession applicationSession, ClaimsUser user)
        {
            var viewModel = new PaymentSummaryPageViewModel();
            var paymentsFilter = applicationSession.PaymentsSummaryFilter;
            datePicker.FilterAction = "Index";
            datePicker.ControllerName = "PaymentSummary";
            viewModel.DateRange = datePicker;
            viewModel.DateRange.InitialDurationInDays = _appSettings.Value.InitialPaymentsRecordsDuration;
            BasePageViewModelHelper.PopulateBasePageViewModel(viewModel, _appSettings.Value, "PaymentHistory", user);

            var dateRangeValidationMessage = ValidateDateRange(paymentsFilter.FromDate, paymentsFilter.ToDate);
            if (!string.IsNullOrWhiteSpace(dateRangeValidationMessage))
            {
                viewModel.HasValidationError = true;
                viewModel.ValidationMessage = dateRangeValidationMessage;
                _userLoginService.UpsertApplicationCookie(Constants.UserSessionCookieKey, applicationSession);
                return View("index", viewModel);
            }
            viewModel.Result = await _paymentsService.GetPaymentSummaries(user.Ukprn.ToString(), paymentsFilter.FromDate, paymentsFilter.ToDate, paymentsFilter.PageNumber);
            _userLoginService.UpsertApplicationCookie(Constants.UserSessionCookieKey, applicationSession);
            return View("index", viewModel);
        }
    }
}
