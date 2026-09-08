using DFESignIn.Services.Implementations;
using DFESignIn.Services.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.Logging;
using ViewYourPayments.Core.Services;
using ViewYourPayments.Infrastructure;
using ViewYourPayments.Web.Extensions;
using ViewYourPayments.Web.Filters;
using ViewYourPayments.Web.Helpers;
using ViewYourPayments.Web.Services;
using ViewYourPayments.Web.Services.Interfaces;

namespace ViewYourPayments.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            var appInsightKey = Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
            var environment = Configuration.GetValue<string>("Environment");
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddApplicationInsightsTelemetry(appInsightKey);
            var appLogger = GetLogger(environment, appInsightKey);
            services.AddSingleton<IApplicationLogger>(al => (appLogger));

            services.AddSingleton<IProviderSearchService>(sp =>
           new UkRlpProviderSearchService(appSettings.ProviderSearchSettings.ServiceEndpoint,
           appSettings.ProviderSearchSettings.SchemaVersion,
           appSettings.ProviderSearchSettings.StakeholderId, appLogger));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedHost;

                options.ForwardedHostHeaderName = "X-Original-Host";

                options.KnownNetworks.Clear(); //its loopback by default
                options.KnownProxies.Clear();
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
            SetApplicationCultureToGB();

            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITagHelperComponent, VYPAnalyticsTagHelper>();
            services.AddHttpClient<IFdsService, FdsService>();
            services.AddHttpClient<IPaymentsHttpClient, PaymentsHttpClient>();
            services.AddHttpClient<IDfeSignInProviderApiHttpClient, DfeSignInProviderApiHttpClient>();
            services.AddHttpClient<IPaymentsApiOAuthHttpClient, PaymentsApiOAuthHttpClient>();
            services.AddTransient<IPaymentsService, PaymentsService>();
            services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddSingleton<IDfeSignInProviderService, DfeSignInProviderService>();
            services.AddTransient<IUserAuthorisationService, UserAuthorisationService>();
            services.AddTransient<IDocumentManagementService, DocumentManagementService>();
            services.AddScoped<IClaimsTransformation, UserInfoClaims>();
            services.ConfigureSignInService(appSettings);
            services.Configure<MvcOptions>(config =>
            {
                config.Filters.Add<CookiePreferencesActionFilterAttribute>();
            });
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default30",
                    new CacheProfile()
                    {
                        Duration = 30

                    });
            });
        }

        private static void SetApplicationCultureToGB()
        {
            var cultureInfo = new CultureInfo("en-GB", false);
            cultureInfo.NumberFormat.CurrencySymbol = "£";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.AddOrReplaceResponseHeader("strict-transport-security", "max-age=15768000; includeSubDomains");
                context.AddOrReplaceResponseHeader("content-security-policy", "default-src 'self'; script-src 'self' 'unsafe-inline' www.google-analytics.com 'unsafe-inline' www.googletagmanager.com 'unsafe-inline' az416426.vo.msecnd.net ajax.cloudflare.com https://*.clarity.ms https://c.bing.com; script-src-elem 'self' 'unsafe-inline' www.google-analytics.com 'unsafe-inline' www.googletagmanager.com 'unsafe-inline' az416426.vo.msecnd.net ajax.cloudflare.com https://*.clarity.ms https://c.bing.com; style-src 'self' 'unsafe-inline' *.cloudflare.com; font-src 'self' *.cloudflare.com data:; connect-src 'self' www.google-analytics.com dc.services.visualstudio.com/v2/track https://*.clarity.ms https://c.bing.com; img-src 'self' www.google-analytics.com https://*.clarity.ms https://c.bing.com");
                context.AddOrReplaceResponseHeader("x-frame-options", "DENY");
                context.AddOrReplaceResponseHeader("x-xss-protection", "0");
                context.AddOrReplaceResponseHeader("x-content-type-options", "nosniff");

                context.RemoveResponseHeader("x-powered-by");

                await next();

                var headers = context.Response.Headers;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Index");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors(policy => policy.SetIsOriginAllowed(origin => origin == "https://pplogon.fasst.org.uk"));
            app.UseCors(policy => policy.SetIsOriginAllowed(origin => origin == "https://adfs.preprod.skillsfunding.service.gov.uk"));

            // app.UseSession(new SessionOptions() { IdleTimeout = new TimeSpan(2, 0, 0) });
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.PreventDefaultNoCachingCacheHeaders();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=PaymentSummary}/{action=Index}/{id?}");
            });
        }


        /// <summary>
        /// Get instance of Application Insight logger.
        /// </summary>
        /// <returns>Instance of ILogger.</returns>
        /// <param name="environment">current environment.</param>
        /// <param name="appInsightsKey">appInsightsKey for logging.</param>
        private ApplicationInsightsLogger GetLogger(string environment, string appInsightsKey)
        {
            var telemetryClient = new TelemetryClient(new TelemetryConfiguration(appInsightsKey));
            return new ApplicationInsightsLogger(telemetryClient, environment);
        }
    }
}
