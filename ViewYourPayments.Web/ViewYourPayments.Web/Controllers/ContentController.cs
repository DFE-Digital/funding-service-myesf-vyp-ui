using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    public class ContentController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        public ContentController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        public IActionResult PaymentHelpGuidelines()
        {
            ClaimsUser user = new ClaimsUser(HttpContext.User);
            var model = new BasePageViewModel()
            {
                ContactUsUrl = _appSettings.Value.ContactUsUrl,
                MyEsfUrl = _appSettings.Value.MyEsfUrl

            };
            BasePageViewModelHelper.PopulateBasePageViewModel(model, _appSettings.Value, "Payment Help Guidelines", user);
            return View(model);
        }
    }
}