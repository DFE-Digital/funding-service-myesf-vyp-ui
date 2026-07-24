using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IApplicationLogger _applicationLogger;

        public ErrorController(IOptions<AppSettings> appSettings,
                               IApplicationLogger applicationLogger
                               )
        {
            _appSettings = appSettings;
            _applicationLogger = applicationLogger;
        }

        public IActionResult Index()
        {
            ErrorPageViewModel model = new ErrorPageViewModel();
            try
            {
                var user = new ClaimsUser(HttpContext.User);
                BasePageViewModelHelper.PopulateBasePageViewModel(model, _appSettings.Value, "Error", user);
                model.ContactUsUrl = _appSettings.Value.ContactUsUrl;
                model.DisplayHomePageLink = false;
            }
            catch (Exception ex)
            {
                _applicationLogger.LogException(ex, "Error occured in error page");
                model = new ErrorPageViewModel() { PageName = "Error" };
            }
            return View(model);
        }

        /// <summary>
        /// Returns an ActionResult of being routed to the Access Denied page.
        /// </summary>
        public IActionResult AccessDenied()
        {
            UnauthorisedErrorViewModel model = new UnauthorisedErrorViewModel();
            try
            {
                var user = new ClaimsUser(HttpContext.User);

                if (user.IsAuthorised)
                    return RedirectToAction("Index", "PaymentSummary");

                BasePageViewModelHelper.PopulateBasePageViewModel(model, _appSettings.Value, "Access Denied", user);
                model.ContactUsUrl = _appSettings?.Value?.ContactUsUrl;
                model.DisplayHomePageLink = true;
                model.DisplayChangeProvider = false;
                model.DfeSignInUrl = _appSettings?.Value?.DfeSettings?.DfeSignInUrl;
                model.requiredRoles = user.IsInternalUser ? user.GetRequiredInternalRoles() : user.GetRequiredExternalRoles();
            }
            catch (Exception ex)
            {
                _applicationLogger.LogException(ex, "Unauthorized user");
                model = new UnauthorisedErrorViewModel() { PageName = "Access Denied" };
            }
            return View(model);
        }
    }
}