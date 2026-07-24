using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    public class HomeController :Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        public HomeController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        [Authorize]
        public IActionResult Index()
        {

            return View();

        }
        public IActionResult Privacy()
        {
            return View();
        }

    }
}
