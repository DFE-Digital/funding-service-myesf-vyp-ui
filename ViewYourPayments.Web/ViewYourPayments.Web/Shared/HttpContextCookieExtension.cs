using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace ViewYourPayments.Web.Shared
{
    public static class HttpContextCookieExtension
    {
        private const int COOKIEEXPINMINS = 20;

        public static void UpsertCookie(this HttpContext httpContext, string keyName, object value)
        {
            httpContext.Response.Cookies.Delete(keyName);
            httpContext.AppendCookie(keyName, value);
        }
        private static void AppendCookie(this HttpContext httpContext, string keyName, object value)
        {
            var seralizedValue = JsonConvert.SerializeObject(value);
            httpContext.AppendCookie(keyName, seralizedValue);
        }
        private static void AppendCookie(this HttpContext httpContext, string keyName, string value)
        {
            CookieOptions option = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddMinutes(COOKIEEXPINMINS),
            };
            httpContext.Response.Cookies.Append(keyName, value, option);
        }

        public static T GetCookieValue<T>(this HttpContext httpContext, string keyName)
        {
            var cookieValueFromReq = httpContext.Request.Cookies[keyName];
            return cookieValueFromReq == null ? default : JsonConvert.DeserializeObject<T>(cookieValueFromReq);
        }
    }
}
