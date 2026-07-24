using Microsoft.AspNetCore.Http;
using System;

namespace ViewYourPayments.Web.Helpers
{
    /// <summary>
    /// Helper class containing common functions for an HTTP context.
    /// </summary>
    public static class HttpContextHelper
    {
        /// <summary>
        /// Adds or replaces a header with the provided key and value to the response headers list.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value.</param>
        public static void AddOrReplaceResponseHeader(this HttpContext httpContext, string key, string value)
        {
            GuardHttpContext(httpContext);
            GuardHttpHeaderKey(key);
            GuardHttpHeaderValue(value);
            var headers = httpContext.Response.Headers;
            RemoveHeaderResponseKeyIfExists(headers, key);
            headers.Add(key, value);
        }
        /// <summary>
        /// Removes a header from the response headers list (if it exists already).
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="key">The header key.</param>
        public static void RemoveResponseHeader(this HttpContext httpContext, string key)
        {
            GuardHttpContext(httpContext);
            GuardHttpHeaderKey(key);
            var headers = httpContext.Response.Headers;
            RemoveHeaderResponseKeyIfExists(headers, key);
        }
        private static void GuardHttpContext(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext), "The HTTP context cannot be null");
            }
        }
        private static void GuardHttpHeaderKey(string key)
            => GuardStringParameter(nameof(key), key, "header key");
        private static void GuardHttpHeaderValue(string value)
            => GuardStringParameter(nameof(value), value, "header value");
        private static void GuardStringParameter(string paramName, string paramValue, string errorMessageName)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName, $"The {errorMessageName} cannot be null");
            }
            else if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ArgumentException($"The {errorMessageName} cannot be empty", paramName);
            }
        }
        private static void RemoveHeaderResponseKeyIfExists(IHeaderDictionary headers, string key)
        {
            if (headers.ContainsKey(key))
            {
                headers.Remove(key);
            }
        }
    }
}
