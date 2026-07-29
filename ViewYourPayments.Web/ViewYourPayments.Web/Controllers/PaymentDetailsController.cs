using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
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
    public class PaymentDetailsController : BaseController
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IApplicationLogger _applicationLogger;
        private readonly IPaymentsService _paymentsService;
        private readonly IDocumentManagementService _documentManagementService;

        public PaymentDetailsController(IOptions<AppSettings> appSettings,
                                IUserAuthorisationService userAuthorisationService,
                                IApplicationLogger applicationLogger,
                                IPaymentsService paymentsService,
                                IDocumentManagementService documentManagementService
                                ) : base(applicationLogger, userAuthorisationService)
        {
            _appSettings = appSettings;
            _applicationLogger = applicationLogger;
            _paymentsService = paymentsService;
            _documentManagementService = documentManagementService;
        }


        [Route("PaymentDetails/{paymentIdentifier}")]
        public async Task<IActionResult> Index([FromRoute] int paymentIdentifier)
        {
            var user = new ClaimsUser(HttpContext.User);
            var userSession = GetUserSessionDetails();
            var paymentData = await _paymentsService.GetPaymentDetails(user.Ukprn.ToString(), paymentIdentifier);

            var viewModel = new PaymentsDetailsPageViewModel
            {
                Result = paymentData,
                PaymentIdentifier = paymentIdentifier,
                BackToPaymentSummaryPageUrl = GetBackToPaymentSummaryUrl(userSession)
            };
            BasePageViewModelHelper.PopulateBasePageViewModel(viewModel, _appSettings.Value, "Payment Details", user);

            return View(viewModel);
        }

        [Route("PaymentDetails/{paymentIdentifier}/downloadCsv")]
        public async Task<IActionResult> DownloadCsv([FromRoute] int paymentIdentifier)
        {
            var user = new ClaimsUser(HttpContext.User);
            var paymentData = await _paymentsService.GetPaymentDetails(user.Ukprn.ToString(), paymentIdentifier);
            if (!paymentData.PaymentLines.Any())
            {
                _applicationLogger.LogWarn($"No details found for paymentIdentifier- {paymentIdentifier} and ukprn: {user.Ukprn}");
                return Content("No details found for selected paymentIdentifier");
            }
            var csvdata = paymentData.PaymentLines.Select(p => new PaymentDetailsCsvExportItem
            {
                Ukprn = paymentData.Ukprn,
                VendorNumber = paymentData.VendorNumber,
                Date = p.PaymentDate.ToShortDateString(),
                ContractNumber = p.ContractNumber,
                BudgetGroup = p.BudgetDescription,
                Description = p.PaymentLineDescription,
                Amount = p.PaymentLineAmount,
            }).Append(new PaymentDetailsCsvExportItem
            {
                Ukprn = paymentData.Ukprn,
                VendorNumber = paymentData.VendorNumber,
                Date = paymentData.PaymentDate.ToShortDateString(),
                Description = "Total Amount",
                Amount = paymentData.PaymentAmount
            });

            var csvContent = _documentManagementService.TransformRecordsToCsvBytes(csvdata);

            var fileName = $"{paymentData.PaymentDate:dd MMMM yyyy} Payment - {paymentData.VendorNumber}.csv";
            return SaveFile(csvContent, "text/csv", fileName);
        }

        private string GetBackToPaymentSummaryUrl(PaymentsApplicationSession userSession)
        {
            var currentPageNumber = userSession?.PaymentsSummaryFilter?.PageNumber;
            var pageNumber = 1;
            if (currentPageNumber.HasValue && currentPageNumber.Value > 0) pageNumber = currentPageNumber.Value;
            return $"{Url.Action("Index", "PaymentSummary")}?page={pageNumber}";
        }
    }
}