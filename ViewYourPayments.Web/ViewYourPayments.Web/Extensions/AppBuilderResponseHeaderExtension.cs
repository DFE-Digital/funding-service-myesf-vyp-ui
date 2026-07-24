using Microsoft.AspNetCore.Builder;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Extensions
{
    public  static class AppBuilderResponseHeaderExtension
    {
        public static void PreventDefaultNoCachingCacheHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, nextMiddleware) =>
            {
                context.Response.OnStarting(() =>
                {
                    var headers = context.Response.Headers;
                    var cacheControlSetToDefaultNoCaching =
                        headers.ContainsKey(HeaderNames.CacheControl)
                        && headers[HeaderNames.CacheControl] == "no-cache, no-store"
                        && headers.ContainsKey(HeaderNames.Pragma)
                        && headers[HeaderNames.Pragma] == "no-cache";
                    if (cacheControlSetToDefaultNoCaching)
                    {
                        headers.Remove(HeaderNames.Pragma);
                        headers[HeaderNames.CacheControl] = "private";
                    }
                    return Task.FromResult(0);
                });
                await nextMiddleware();
            });
        }
    }
}
