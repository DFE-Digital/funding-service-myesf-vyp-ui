using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ViewYourPayments.Core.Enums.User;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Web.Shared;

namespace ViewYourPayments.Web.Controllers
{
    public class AccountController : Controller
    {
        readonly IOptions<AppSettings> _appSettings;
        readonly IApplicationLogger _applicationLogger;


        public AccountController(IOptions<AppSettings> appSettings, IApplicationLogger applicationLogger)
        {
            _appSettings = appSettings;
            _applicationLogger = applicationLogger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task Signout()
        {
            if (SingleSignOnProviderType.Idams == _appSettings.Value.SingleSignOnProvider)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Response.Redirect(Url.Action("PostLogOut", "Account"));
            }
            else
            {
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    RedirectUri = Url.Action("PostLogOut", "Account")
                });
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            
        }

        [AllowAnonymous]
        public async Task Login(string returnUrl = "/")
        {
            var redirectUri = Url.Action("index", "Home");
            await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties() { RedirectUri = redirectUri });

        }

        [AllowAnonymous]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("oidc", new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoggedOut", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");
        }

        [AllowAnonymous]
        public IActionResult PostLogOut()
        {
            HttpContext.Response.Cookies.Delete(Constants.UserSessionCookieKey);
            return new RedirectResult(string.Format("{0}{1}", _appSettings.Value.MyEsfUrl, "/account/logout"));
        }

        [AllowAnonymous]
        public IActionResult PostLoggedOutRedirect()
        {
            return new RedirectResult(string.Format("{0}{1}", _appSettings.Value.MyEsfUrl, "/account/logout"));
        }

        public IActionResult UnAuthorisedUser()
        {
            _applicationLogger.LogWarn("User is not authorised");
            return RedirectToAction("AccessDenied", "Error");
        }
    }
}