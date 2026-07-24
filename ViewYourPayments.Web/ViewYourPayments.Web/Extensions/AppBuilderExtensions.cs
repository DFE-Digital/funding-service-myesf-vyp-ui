using DFESignIn.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ViewYourPayments.Core.Enums.User;
using ViewYourPayments.Core.Extensions;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.DFESignIn;

namespace ViewYourPayments.Web.Extensions
{
    public static class AppBuilderExtensions
    {
        private const string COOKIENAME = ".Auth.Vyp.Shared";
        private const int COOKIEEXPINMINS = 20;
        private const string LOGINPATH = "/login";
        private const string LOGOUTPATH = "/logout";
        private const string MATOrganisationCategoryId = "010";

        public static void ConfigureSignInService(this IServiceCollection services, AppSettings appSettings)
        {
            if (SingleSignOnProviderType.Idams == appSettings.SingleSignOnProvider)
            {
                ConfigureServiceUsingIdams(services, appSettings.IdamsSettings);
                return;
            }

            var serviceProvider = services.BuildServiceProvider();
            var signInProviderservice = serviceProvider.GetService<IDfeSignInProviderService>();
            ConfigureServiceUsingDfeSignIn(services, appSettings.DfeSettings, signInProviderservice);
        }

        private static void ConfigureServiceUsingDfeSignIn(this IServiceCollection services, DfeSettings dfeSettings, IDfeSignInProviderService dfeSignInProviderService)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; ;
            })
             .AddCookie(options => SetSignInCookies(options))
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // Set the authority to your Auth0 domain
                options.Authority = dfeSettings.OidcAuthority;
                options.ClientId = dfeSettings.OidcClientId;
                options.ClientSecret = dfeSettings.OidcClientSecret;

                // Set response type to code
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                // Configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.Scope.Add("organisationid");

                options.CallbackPath = dfeSettings.OidcRedirectUrl;
                options.SignedOutCallbackPath = dfeSettings.OidcPostLogOutUrl;
                options.SignedOutRedirectUri = $"{dfeSettings.OidcPostLogOutUrl}Redirect";

                options.SaveTokens = true;
                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = dfeSettings.OidcAudience;

                options.UseSecurityTokenValidator = true;

                options.ProtocolValidator = new OpenIdConnectProtocolValidator
                {
                    RequireSub = true,
                    RequireStateValidation = false,
                    NonceLifetime = TimeSpan.FromMinutes(60),
                    RequireNonce = true
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async (context) =>
                    {
                        await ValidateToken(dfeSettings, dfeSignInProviderService, context);
                        var httpContext = context.HttpContext;
                        httpContext.Items["Properties"] = context.Properties;
                        httpContext.Features.Set(context.Properties);
                        return;
                    },
                    OnRemoteFailure = async (context) =>
                    {
                        await RemoteAuthFail(context);
                    },
                };
            });

            services.AddMvc();
        }

        private static void ConfigureServiceUsingIdams(IServiceCollection services, IdamsSettings idamsSettings)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.SameAsRequest;
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            })
                .AddWsFederation(GetWsFedOptionDev(idamsSettings))
               .AddCookie(options => SetSignInCookies(options));
            services.AddMvc();
        }

        private static Action<WsFederationOptions> GetWsFedOptionDev(IdamsSettings idamsSettings)
        {
            return options =>
            {
                options.ClaimsIssuer = idamsSettings.ClaimsIssuer;
                options.MetadataAddress = idamsSettings.MetadataAddress;
                options.CallbackPath = idamsSettings.CallbackPath;
                options.RequireHttpsMetadata = idamsSettings.RequireHttpsMetadata;
                options.Wtrealm = idamsSettings.Wtrealm;
                options.Events.OnSecurityTokenValidated = (ctx) =>
                {
                    var identity = ctx.HttpContext.User.Identity;
                    return Task.FromResult(true);
                };
            };
        }

        private static void SetSignInCookies(CookieAuthenticationOptions cookieOptions)
        {
            cookieOptions.Cookie.Name = COOKIENAME;
            cookieOptions.SlidingExpiration = true;
            cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(COOKIEEXPINMINS);
            cookieOptions.LoginPath = new PathString(LOGINPATH);
            cookieOptions.LogoutPath = new PathString(LOGOUTPATH);
            cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }

        private static Task RemoteAuthFail(RemoteFailureContext context)
        {
            context.Response.Redirect("/Home/Error"); context.HandleResponse(); return Task.CompletedTask;
        }

        private static async Task ValidateToken(DfeSettings dfeSettings, IDfeSignInProviderService dfeSignInProviderService, TokenValidatedContext context)
        {
            var identity = context.Principal.Identities.FirstOrDefault();
            var userId = new Guid(identity.GetStringValue("nameidentifier").ToString());

            var organisationId = identity.GetOrganisationId("organisation");

            var organisation = await dfeSignInProviderService.GetOrganisationAsync(userId, organisationId);

            identity.AddOrganisationNameClaim(organisation.Name);
            identity.AddOrganisationUkPrnClaim(organisation.Ukprn);

            var firstName = identity.GetStringValue("givenname");
            var lastName = identity.GetStringValue("surname");
            var email = identity.GetStringValue("emailaddress");
            var fullName = $"{firstName} {lastName}";
            var principal = GetPrincipal(organisation, firstName, lastName, email, dfeSettings.DfeLegacyCodeId);
            bool isInternalUser = organisation.IsInternalDFE(dfeSettings.DfeLegacyCodeId);
            var roles = await dfeSignInProviderService.GetRolesAsync(userId, organisationId);

            identity.AddNameClaim(fullName);
            identity.AddFirstNameClaim(firstName);
            identity.AddLastNameClaim(lastName);
            identity.AddEmailClaim(email);
            identity.AddPrincipalClaim(principal);
            identity.AddRoles(roles);
            identity.AddIdTokenClaim(context.ProtocolMessage.IdToken);
            identity.AddUserTypeClaim(isInternalUser);

            Thread.CurrentPrincipal = new ClaimsPrincipal(identity);

            return;
        }

        private static string GetPrincipal(Organisation organisation, string firstName, string lastName, string email, string dfeLegacyCodeId)
        {
            string organisationComponent;
            if (organisation.IsInternalDFE(dfeLegacyCodeId))
            {
                organisationComponent = organisation.LegacyId;
            }
            else
            {
                organisationComponent = MATOrganisationCategoryId.Equals(organisation.Category?.Id)
                ? organisation.CompanyRegistrationNumber
                : organisation.Ukprn;
            }

            return $"{firstName}{lastName}{organisationComponent}{email}";
        }
    }
}
