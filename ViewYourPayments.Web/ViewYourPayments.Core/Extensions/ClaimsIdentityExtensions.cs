using ViewYourPayments.Core.Models.DFESignIn;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using ViewYourPayments.Core.Models.User;
using System;

namespace ViewYourPayments.Core.Extensions
{
    /// <summary>
    /// Extension class that exposes extensions methods for <see cref="ClaimsIdentity"/>. 
    /// </summary>
    public static class ClaimsIdentityExtensions
    {      

        /// <summary>
        /// Adds ukprn claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Organisations ukprn value.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddOrganisationUkPrnClaim(this ClaimsIdentity identity, string value)
        {
            if (!string.IsNullOrEmpty(value)) identity.AddClaim(new Claim(ClaimsUser.UkprnClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds Org Name claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Organisations name</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddOrganisationNameClaim(this ClaimsIdentity identity, string value)
        {
            if (!string.IsNullOrEmpty(value)) identity.AddClaim(new Claim(ClaimsUser.OrganisationNameClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds User Name claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Logged-in users name.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddNameClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.NameClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds FirstName claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Logged-in users first name.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddFirstNameClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.FirstNameClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds LastName claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Logged-in users last name.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddLastNameClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.LastNameClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds Principal claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Logged-in users principal id.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddPrincipalClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.PrincipalClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds each role returned by DFE as a role claim in identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="roles">List of roles of type <see cref="Role"/> the logged-in user is subscribed to.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddRoles(this ClaimsIdentity identity, IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                var myesfRole = AllDfeRoles.Get(role.Code.ToLower());

                if (!string.IsNullOrEmpty(myesfRole))
                {
                    identity.AddClaim(new Claim(ClaimsUser.RoleClaimType, myesfRole));
                }
            }
           

            return identity;
        }

        /// <summary>
        /// Adds UerType claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="organisation">Contains organisation data related to the logged-in user.</param>
        /// <param name="dfeLegacyCodeId">The legacy code of "DFE" organisation defined in DFE Sign-In.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddUserTypeClaim(this ClaimsIdentity identity, bool isInternalUser)
        {
            identity.AddClaim(new Claim(ClaimsUser.UserTypeClaimType, isInternalUser ? "0" : "1"));           

            return identity;
        }

        /// <summary>
        /// Adds email claim to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Email address of the logged-in user.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddEmailClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.EmailClaimType, value));
            return identity;
        }

        /// <summary>
        /// Adds the id_token returned by Dfe to identity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="value">Id token returned by DFE Signin.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        public static ClaimsIdentity AddIdTokenClaim(this ClaimsIdentity identity, string value)
        {
            identity.AddClaim(new Claim(ClaimsUser.IdTokenClaimType, value));
            return identity;
        }

        /// <summary>
        /// Gets Org data returned by Dfe wrapped in a strongly typed Org object.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="ClaimType">The name of the organisation claim entity returned by DFE SignIn.</param>
        /// <returns><see cref="Organisation"/>.</returns>
        public static Organisation GetOrganisation(this ClaimsIdentity identity, string claimType)
        {
            try
            {
                return JsonConvert.DeserializeObject<Organisation>(GetValueFromClaimSingle(identity, claimType));
            }

            catch
            {
                throw new JsonSerializationException("Unable to get organisation details from DFE.");
            }
        }

        /// <summary>
        /// Returns the first available value associated to a claimType in the indentity claims.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="claimType">The name of the claim for which the value needs extracting from.</param>
        /// <returns>Value of the first matching claim.</returns>
        public static string GetStringValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.Claims.First(x => x.Type.EndsWith( claimType)).Value;
        }


        /// <summary>
        /// Gets Organisation ID returned by Dfe.
        /// </summary>
        /// <param name="identity">Represents a claims-based identity.</param>
        /// <param name="ClaimType">The name of the organisation claim entity returned by DFE SignIn.</param>
        /// <returns>Organsation Id.</returns>
        public static Guid GetOrganisationId(this ClaimsIdentity identity, string claimType)
        {
            try
            {
                var organisationAsJson = GetValueFromClaimSingle(identity, claimType);
                var organisation = JsonConvert.DeserializeObject<Organisation>(organisationAsJson);
                return new Guid(organisation.Id);
            }
            catch
            {
                throw new JsonSerializationException("Unable to get organisation details from DFE.");
            }
        }


        private static string GetValueFromClaimSingle(ClaimsIdentity identity, string claimType)
        {
            return identity.Claims.Where(c => c.Type == claimType).Select(c => c.Value).FirstOrDefault();
        }

        /// <summary>
        /// static collection of all dfe role codes mapped to myEsf roles.
        /// </summary>
        private static readonly NameValueCollection AllDfeRoles = new NameValueCollection()
        {
            { "apprenticeshipseditor", UserRole.ApprenticeshipsEditor.ToString() },
            { "documentexchangeadministratorfundingcentre", UserRole.DocumentExchangeAdministratorFundingCentre.ToString()},
            { "documentexchangeadministratorriskassurance", UserRole.DocumentExchangeAdministratorRiskAssurance.ToString() },
            { "documentexchangeuser", UserRole.DocumentExchangeUser.ToString() },
            { "sfsadmin", UserRole.SfsAdmin.ToString()  },
            { "viewasprovider", UserRole.ViewAsProvider.ToString() },
            { "viewpaymenthistory", UserRole.ViewPaymentHistory.ToString() },
            { "ViewContractsAndAgreements", UserRole.ViewContractsAndAgreements.ToString() },
            { "SignContractsAndAgreements", UserRole.SignContractsAndAgreements.ToString() },
            { "ViewPreviousSubcontractorDeclarations", UserRole.ViewPreviousSubcontractorDeclarations.ToString() },
            { "SubmitSubcontractorDeclarations", UserRole.SubmitSubcontractorDeclarations.ToString() },
            { "ViewAllocationStatements", UserRole.ViewAllocationStatements.ToString() },
            { "ViewFundingClaimsAndReconciliationStatements", UserRole.ViewFundingClaimsAndReconciliationStatements.ToString() },
            { "SignFundingClaims", UserRole.SignFundingClaims.ToString() },
            { "AllocationsAdministrator_1416", UserRole.AllocationsAdministrator_1416.ToString() },
            { "AllocationsAdministrator_1619", UserRole.AllocationsAdministrator_1619.ToString() },
            { "AllocationsAdministrator_DSG", UserRole.AllocationsAdministrator_DSG.ToString() },
            { "AllocationsAdministrator_GAG", UserRole.AllocationsAdministrator_GAG.ToString() },
            { "AllocationsAdministrator_NMSS", UserRole.AllocationsAdministrator_NMSS.ToString() },
            { "AllocationsAdministrator_PPG", UserRole.AllocationsAdministrator_PPG.ToString() },
            { "AllocationsAdministrator_PSG", UserRole.AllocationsAdministrator_PSG.ToString() },
            { "DownloadSCDReports", UserRole.DownloadSCDReports.ToString() },
            { "DownloadReports", UserRole.DownloadReports.ToString() },
            { "NffAdmin", UserRole.NffAdmin.ToString() },
            { "ViewApprenticeshipGrantForEmployersReports", UserRole.ViewApprenticeshipGrantForEmployersReports.ToString() },
            { "ViewRecoupmentReports", UserRole.ViewRecoupmentReports.ToString() }
        };
    }
}
