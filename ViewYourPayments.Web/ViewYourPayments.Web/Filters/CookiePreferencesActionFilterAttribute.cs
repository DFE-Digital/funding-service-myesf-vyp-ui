using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using ViewYourPayments.Web.Models;

namespace ViewYourPayments.Web.Filters
{
    public class CookiePreferencesActionFilterAttribute : ActionFilterAttribute
    {
        private const string CookiesPreferencesSetCookieName = "cookies_preferences_set";
        private const string CookiesPreferencesSetTrue = "true";

        private static readonly CookieOptions CookiePreferencesCookiesOptions = new CookieOptions
        {
            Secure = true,
            MaxAge = TimeSpan.FromDays(365),
            Path = "/"
        };

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CookiePreferencesActionFilterAttribute"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public CookiePreferencesActionFilterAttribute(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();

            var requestCookies = _httpContextAccessor.HttpContext.Request.Cookies;

            if (actionExecutedContext.Result is ViewResult viewResult &&
                viewResult.ViewData?.Model is BasePageViewModel model)
            {
                if (requestCookies.TryGetValue(CookiesPreferencesSetCookieName, out string cookiesPreferencesSetCookie)
                    && cookiesPreferencesSetCookie.Equals(CookiesPreferencesSetTrue, System.StringComparison.OrdinalIgnoreCase))
                {
                    model.CookiesPreferencesSet = true;
                }
                else
                {
                    model.CookiesPreferencesSet = false;
                    _httpContextAccessor.HttpContext.Response.Cookies.Append(
                        CookiesPreferencesSetCookieName,
                        CookiesPreferencesSetTrue,
                        CookiePreferencesCookiesOptions);
                }
            }
        }
    }
}