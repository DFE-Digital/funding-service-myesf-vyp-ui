using ViewYourPayments.Core.Models.DFESignIn;
using DFESignIn.Services.Interfaces;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewYourPayments.Core.Models.AppConfig;
using Microsoft.Extensions.Options;
using ViewYourPayments.Core.Interfaces.HttpClient;
using System.Linq;

namespace DFESignIn.Services.Implementations
{
    /// <summary>
    /// Main facade for interactions relating to DSI api's and SignIn/SignOut.
    /// </summary>
    public class DfeSignInProviderService : IDfeSignInProviderService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IDfeSignInProviderApiHttpClient _dfeSignInProviderHttpClient;


        public DfeSignInProviderService(){ }

        public DfeSignInProviderService(IOptions<AppSettings> appSettings,
            IDfeSignInProviderApiHttpClient dfeSignInProviderHttpClient)
        {
            this._dfeSignInProviderHttpClient = dfeSignInProviderHttpClient;
            this._appSettings = appSettings;
        }

        /// <summary>
        /// Method to return list of roles associated to a specific user, org combination.
        /// </summary>
        /// <param name="userId">UserId returned from DFE SignIn.</param>
        /// <param name="organisationId">Org Id returned from DFE SignIn.</param>
        /// <returns>List of <see cref="Role"/> objects.</returns>
        public async Task<IEnumerable<Role>> GetRolesAsync(Guid userId, Guid organisationId)
        {
            var apiSettings = _appSettings.Value.DfeRoleProviderSettings;
            var token = GetBearerToken(apiSettings);
            var request = $"/services/{apiSettings.OidcClientId}/organisations/{organisationId}/users/{userId}";
            var result = await _dfeSignInProviderHttpClient.Get<DfeClaims>(token, request);
            return result.Roles;
        }

        /// <summary>
        /// Get organisation details from Oidc api.
        /// </summary>
        /// <param name="userId">sddsdgfgf.</param>
        /// <param name="organisationId">sddsdsdffd.</param>
        /// <returns>ssdsd.</returns>
        public async Task<Organisation> GetOrganisationAsync(Guid userId, Guid organisationId)
        {
            var apiSettings = _appSettings.Value.DfeRoleProviderSettings;
            var token = GetBearerToken(apiSettings);
            var request = $"/users/{userId}/organisations";
            var result = await _dfeSignInProviderHttpClient.Get<IEnumerable<Organisation>>(token, request);
            return result?.FirstOrDefault(x=>x.Id.Equals(organisationId.ToString(),StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a JWT token using HMACSHA256 algorithm.
        /// </summary>
        /// <param name="apiSettings">The API settings containing the required credentials for generating the token.</param>
        /// <returns>A bearer token of type string.</returns>
        private static string GetBearerToken(DfeRoleProviderSettings apiSettings)
        {
            return new JwtBuilder()
                       .WithAlgorithm(new HMACSHA256Algorithm())
                       .Issuer(apiSettings.OidcClientId)
                       .Audience(apiSettings.OidcAudience)
                       .WithSecret(apiSettings.OidcClientSecret)
                       .Encode();
        }

    }
}