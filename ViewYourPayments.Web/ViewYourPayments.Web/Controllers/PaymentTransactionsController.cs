using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Filters;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    [Authorize]
    [AuthenticateUser]

    public class PaymentTransactionsController : BaseController
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IPaymentsService _paymentsService;
        private readonly IUserAuthorisationService _userLoginService;
        private readonly IApplicationLogger _applicationLogger;
        private readonly IDocumentManagementService _documentManagementService;
        private const int DefaultPageSizeForDownload = 0;
        private const PaymentTransactionSortFields DefaultPrimarySortField = PaymentTransactionSortFields.PaymentDate;
        private const SortDirection DefaultSortDirection = SortDirection.Descending;


        public PaymentTransactionsController(IOptions<AppSettings> appSettings,
                                IApplicationLogger applicationLogger,
                                 IPaymentsService paymentsService,
                                 IUserAuthorisationService userLoginService,
                                 IDocumentManagementService documentManagementService
                                ) : base(applicationLogger, userLoginService)
        {
            _appSettings = appSettings;
            _paymentsService = paymentsService;
            _userLoginService = userLoginService;
            _applicationLogger = applicationLogger;
            _documentManagementService = documentManagementService;
        }

        /// <summary>
        /// Get payment transaction page for selected search options.
        /// </summary>
        /// <param name="dateRange">Date range (StartDate and EndDate).</param>
        /// <param name="searchTerms">Selected search terms by user.</param>
        /// <param name="page">Selected page number.</param>
        /// <param name="isPageReset">IsPageReset flag to identify that user has requested clear session value and reset search params.</param>
        /// <returns>Returns PaymentTransaction page.</returns>
        public async Task<IActionResult> Index([FromForm]DatePickerViewModel dateRange, [FromForm] string  searchTerms = null, [FromQuery] int page = 0, bool isPageReset = false)
        {
            var user = new ClaimsUser(HttpContext.User);
            var appSession = GetUserSessionDetails();
            var paymentsFilter = appSession?.PaymentTransactionsFilter;
            searchTerms = SanitizeCsvString(searchTerms);

            ViewBag.Section = string.Empty;

            if (Request.Method.Equals(HttpMethods.Post))
            {
                paymentsFilter = SetFilterDateFromUserInput(dateRange, searchTerms);
            }
            else if (ShouldSetFilterDateFromSession(paymentsFilter, isPageReset))
            {
                paymentsFilter.PageNumber = page > 0 ? page : paymentsFilter.PageNumber;
            }
            else
            {
                paymentsFilter = new PaymentsTransactionsFilter(
                            DateTime.UtcNow.AddDays(-1 * _appSettings.Value.InitialTransactionViewRecordsDuration), DateTime.UtcNow, page);
            }

           var  transactionFilterViewModel = new PaymentTransactionFilterViewModel
            {
                DateRange = new DatePickerViewModel(paymentsFilter.FromDate, paymentsFilter.ToDate),
                SearchTerms = paymentsFilter.SearchTerms
            };
            appSession.PaymentTransactionsFilter = paymentsFilter;
            var viewModel = await GetPaymentTransactionsData(transactionFilterViewModel, appSession, user, paymentsFilter.PrimarySortField, paymentsFilter.SortDirection);
            return  View("index", viewModel); 
        }

        /// <summary>
        /// Reset search params to default.
        /// </summary>
        /// <returns>Returns PaymentTransaction page.</returns>
        [HttpPost]
        public IActionResult Reset()
        {
            return RedirectToAction("Index", new { isPageReset = true });
        }

        /// <summary>
        /// Change result order for selected primary sort field and direction.
        /// </summary>
        /// <param name="fieldName">primary sort field name.</param>
        /// <param name="sortDirection">sort direction.</param>
        /// <returns>Return transaction page with updated result.</returns>
        [HttpGet]
        public async Task<IActionResult> ChangeOrder([FromQuery]string fieldName, [FromQuery]string sortDirection)
        {
            var user = new ClaimsUser(HttpContext.User);
            var appSession = GetUserSessionDetails();
            var paymentsFilter = appSession?.PaymentTransactionsFilter;

            if (!Enum.TryParse(fieldName, out PaymentTransactionSortFields sortField))
            {
                sortField = PaymentTransactionSortFields.PaymentDate;
                _applicationLogger.LogWarn($"Incorrect sort field selected, defaulted to PaymentDate, selected value was: {fieldName}");
            }
            if (!Enum.TryParse(sortDirection, true, out SortDirection sortTypeDirection))
            {
                sortTypeDirection = SortDirection.Descending;
                _applicationLogger.LogWarn($"Incorrect sort direction selected, defaulted to Descending, selected value was: {sortDirection}");
            }

            if (paymentsFilter == null)
            {           
                paymentsFilter = new PaymentsTransactionsFilter(
                            DateTime.UtcNow.AddDays(-1 * _appSettings.Value.InitialTransactionViewRecordsDuration), DateTime.UtcNow, 1);
            }
           var transactionFilter = new PaymentTransactionFilterViewModel
            {
                DateRange = new DatePickerViewModel(paymentsFilter.FromDate, paymentsFilter.ToDate),
                SearchTerms = paymentsFilter.SearchTerms,
            };           

            paymentsFilter.PageNumber = 1;
            paymentsFilter.PrimarySortField = sortField;
            paymentsFilter.SortDirection = sortTypeDirection;
            appSession.PaymentTransactionsFilter = paymentsFilter;

            var viewModel = await GetPaymentTransactionsData(transactionFilter, appSession, user,sortField, sortTypeDirection);
            ViewBag.Section = "budget-group-summary-section";
            return View("index", viewModel); 
        }

        public async Task<IActionResult> DownloadCsv()
        {
            var user = new ClaimsUser(HttpContext.User);
            var appSession = GetUserSessionDetails();
            var paymentsFilter = appSession?.PaymentTransactionsFilter;
            var searchCritria = new PaymentTransactionSearchCriteria
            {
                Ukprn = user.Ukprn.ToString(),
                StartDate = paymentsFilter.FromDate,
                EndDate = paymentsFilter.ToDate,
                PageNumber = 1,
                PageSize = DefaultPageSizeForDownload,
                PrimarySortField = paymentsFilter.PrimarySortField,
                SortDirection = paymentsFilter.SortDirection,
                SearchTerms = paymentsFilter.SearchTerms?.Split(",")
            };
            var result = await _paymentsService.GetPaymentTransactions(searchCritria);

            if (result == null || !result.PaymentTransactions.Any())
            {
                _applicationLogger.LogWarn($"No details found for ukprn: {user.Ukprn}");
                return Content("No details found for selected paymentIdentifier");
            }
            var csvdata = result.PaymentTransactions.Select(p => new PaymentTransactionsCsvExportItem
            {
                Ukprn = result.Ukprn,
                VendorNumber = result.VendorNumber,
                Date = p.PaymentDate.ToShortDateString(),
                ContractNumber = p.ContractNumber,
                BudgetGroup = p.BudgetDescription,
                Description = p.PaymentLineDescription,
                Amount = p.PaymentLineAmount,
            });

            var csvContent = _documentManagementService.TransformRecordsToCsvBytes(csvdata);

            var fileName = $"Transaction details-{result.VendorNumber}-{DateTime.Now:dd MMMM yyyyTHHmmss}.csv";
            return SaveFile(csvContent, "text/csv", fileName);
        }

        /// <summary>
        /// Get unique transaction descriptions for selected dates and ukprn in session.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DescriptionFilterItem>> GetUniqueTransanctionDescripitons()
        {
            var user = new ClaimsUser(HttpContext.User);
            var appSession = GetUserSessionDetails();
            var paymentsFilter = appSession?.PaymentTransactionsFilter;
            var result = await _paymentsService.GetUniqueTransactionDescriptions(user.Ukprn.ToString(),
                                                                        paymentsFilter.FromDate,
                                                                        paymentsFilter.ToDate);

            if (result == null || !result.Any())
            {
                _applicationLogger.LogInfo($"No unique transaction descriptions found for ukprn: {user.Ukprn} - fromdate:{paymentsFilter.FromDate} - endDate: {paymentsFilter.ToDate}");
            }
            return result.Select(x=>new DescriptionFilterItem { Text =x, Value =x});
        }

        private bool ShouldSetFilterDateFromSession(PaymentsTransactionsFilter paymentFilter, bool isPageReset)
        {
            return !isPageReset && paymentFilter != null;
        }

        private static PaymentsTransactionsFilter SetFilterDateFromUserInput(DatePickerViewModel datePicker, string searchTerms)
        {
            DateTime.TryParse($"{datePicker.StartDateYear}/" +
                            $"{datePicker.StartDateMonth}/" +
                            $"{datePicker.StartDateDay}", out DateTime startDate);

            DateTime.TryParse($"{datePicker.EndDateYear}/" +
            $"{datePicker.EndDateMonth}/" +
            $"{datePicker.EndDateDay}", out DateTime endDate);

            return new PaymentsTransactionsFilter(startDate, endDate, 1)
            {
                SearchTerms = searchTerms
            };
        }

        private async Task<PaymentTransactionsPageViewModel> GetPaymentTransactionsData(PaymentTransactionFilterViewModel filterViewModel, 
                                        PaymentsApplicationSession applicationSession,
                                        ClaimsUser user, 
                                        PaymentTransactionSortFields sortField ,
                                        SortDirection sortDirection )
        {
            var viewModel = new PaymentTransactionsPageViewModel();
            var paymentsFilter = applicationSession.PaymentTransactionsFilter;
            filterViewModel.DateRange.FilterAction = "Index";
            filterViewModel.DateRange.ControllerName = "PaymentTransactions";
            viewModel.TransactionFilter = filterViewModel;
            viewModel.TransactionFilter.DateRange.InitialDurationInDays = _appSettings.Value.InitialPaymentsRecordsDuration;
            BasePageViewModelHelper.PopulateBasePageViewModel(viewModel, _appSettings.Value, "Filter and export transactions", user);

            var dateRangeValidationMessage = ValidateDateRange(paymentsFilter.FromDate, paymentsFilter.ToDate);
            if (!string.IsNullOrWhiteSpace(dateRangeValidationMessage))
            {
                viewModel.HasValidationError = true;
                viewModel.ValidationMessage = dateRangeValidationMessage;
                _userLoginService.UpsertApplicationCookie(Constants.UserSessionCookieKey, applicationSession);
                return viewModel;
            }

            if (paymentsFilter.PageNumber <= 0) paymentsFilter.PageNumber = 1;
            var searchCritria = new PaymentTransactionSearchCriteria
            {
                Ukprn = user.Ukprn.ToString(),
                StartDate = paymentsFilter.FromDate,
                EndDate = paymentsFilter.ToDate,
                PageNumber = paymentsFilter.PageNumber,
                PageSize = _appSettings.Value.PaymentTransactionPageSize,
                PrimarySortField = sortField,
                SortDirection = sortDirection,
                SearchTerms = paymentsFilter.SearchTerms?.Split(",")
            };

            viewModel.Result = await _paymentsService.GetPaymentTransactions(searchCritria);
            viewModel.Result.NextPageLink = Url.Action("Index", "PaymentTransactions") + $"?page={viewModel.Result.CurrentPage + 1}";
            viewModel.Result.PreviousPageLink = Url.Action("Index", "PaymentTransactions") + $"?page={viewModel.Result.CurrentPage - 1}";
            viewModel.TransactionFilter.GetUniqueTransctionDescriptionsUrl = Url.Action("GetUniqueTransanctionDescripitons", "PaymentTransactions");
            viewModel.TransactionFilter.CurrentSearchTermsKeyValuePairs =  GetSearchTermsAsJson(paymentsFilter.SearchTerms);
            _userLoginService.UpsertApplicationCookie(Constants.UserSessionCookieKey, applicationSession);
            return viewModel;
        }

        private static string GetSearchTermsAsJson(string searchTerms)
        {
            var currentSearchTerms = searchTerms?.Split(",",StringSplitOptions.RemoveEmptyEntries);
            if (currentSearchTerms != null && currentSearchTerms.Any())
            {
                var descriptionFilterItems = currentSearchTerms.Select(x => new DescriptionFilterItem { Text = x, Value = x });
                return JsonConvert.SerializeObject(descriptionFilterItems, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new LowerCaseContractResolver()
                    });
            }
            return string.Empty;
        }

        static string SanitizeCsvString(string searchTerms)
        {
            return string.IsNullOrWhiteSpace(searchTerms) ? string.Empty :
                string.Join(",", searchTerms.Split(",").Select(item => item.Trim()).Where(item => !string.IsNullOrWhiteSpace(item)));
        }
    }
}