using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using ViewYourPayments.Core.Attributes;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Core.Enums.User;
using ViewYourPayments.Core.Interfaces.User;

namespace ViewYourPayments.Core.Models.User
{
    public class ClaimsUser : IUser
    {
        public static readonly string FirstNameClaimType = "http://sfs-sfa.gov.uk/claims/firstName";
        public static readonly string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public static readonly string LastNameClaimType = "http://sfs-sfa.gov.uk/claims/lastName";
        public static readonly string PrincipalClaimType = "http://sfs-sfa.gov.uk/claims/principal";
        public static readonly string UkprnClaimType = "http://sfs-sfa.gov.uk/claims/organisationId";
        public static readonly string RoleClaimType = "http://sfs-sfa.gov.uk/claims/role";
        public static readonly string SupportUserEmailAddressClaimType = "http://sfs-sfa.gov.uk/claims/supportUserEmailAddress";
        public static readonly string EmailClaimType = "http://sfs-sfa.gov.uk/claims/email";
        public static readonly string UserTypeClaimType = "http://sfs-sfa.gov.uk/claims/userType";
        public static readonly string OrganisationNameClaimType = "http://sfs-sfa.gov.uk/claims/organisationName";
        public static readonly string OrganisationTradingNameClaimType = "http://sfs-sfa.gov.uk/claims/organisationTradingName";
        public static readonly string IdTokenClaimType = "id_token";
        private const string InternalUserClaimValue = "0";

        private readonly ClaimsPrincipal _claimPrincipal;
        private readonly IEnumerable<UserRole> RequiredUserRoles = new[] { UserRole.ViewAsProvider, UserRole.ViewPaymentHistory };

        public ClaimsUser(ClaimsPrincipal claimPrincipal)
        {
            _claimPrincipal = claimPrincipal;
        }
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name => GetClaimValueForType(PrincipalClaimType);
        /// <summary>
        /// Returns a flag indicating if the User has been authenticated.
        /// </summary>
        public bool IsAuthenticated => _claimPrincipal.Identity.IsAuthenticated;
        /// <summary>
        /// Get the User's UKPRN.
        /// </summary>
        public int? Ukprn
        {
            get
            {
                var claimValueForType = GetClaimValueForType(UkprnClaimType);
                if (string.IsNullOrWhiteSpace(claimValueForType))
                {
                    return null;
                }
                if (!int.TryParse(claimValueForType, out int ukprn))
                {
                    throw new Exception(Name);
                }
                return ukprn;
            }
        }
        /// <summary>
        /// Get the User's first name.
        /// </summary>
        public string FirstName => GetDefaultStringClaimValueForType(FirstNameClaimType);
        /// <summary>
        /// Get the User's last name.
        /// </summary>
        public string LastName => GetDefaultStringClaimValueForType(LastNameClaimType);
        /// <summary>
        /// Gets the User's Organisation type.
        /// </summary>
        public string ProviderName => GetDefaultStringClaimValueForType(OrganisationNameClaimType);

        /// <summary>
        /// Returns the role this user has as a string.
        /// </summary>
        public string RolesDisplay
        {
            get
            {
                var roles = GetClaimValuesForType(RoleClaimType);
                return string.Join(", ", roles);
            }
        }
        /// <summary>
        /// Gets an list of the support users email addresses.
        /// </summary>
        public IList<string> SupportUserEmailAddresses => GetClaimValuesForType(SupportUserEmailAddressClaimType).ToList();
        /// <summary>
        /// Gets the User's email address.
        /// </summary>
        public string Email => GetStringClaimValueForType(EmailClaimType);
        /// <summary>
        /// Gets a list of the current user roles
        /// </summary>
        public IEnumerable<UserRole> UserRoles => GetClaimValuesForType(RoleClaimType)
                .Select(currentRole => currentRole.GetEnumFromPropertyValue<UserRole, ContactServiceDescriptorAttribute, string>(e => e.GroupName));
        /// <summary>
        /// Returns a flag indicating if the user has at least one of the required permissions.
        /// </summary>
        /// <param name="requiredUserPermissions">The permissions that the user must have at least one of.</param>
        /// <returns><c>true</c> if the user has at least one permission; otherwise, <c>false</c>.</returns>
        public bool HasAnyRole(IEnumerable<UserRole> requiredUserPermissions)
        {
            return UserRoles.Any(requiredUserPermissions.Contains);
        }
        /// <summary>
        /// Determines whether the user is an internal user.
        /// </summary>
        /// <returns>Returns true if the user is an internal user.</returns>
        public bool IsInternalUser => GetClaimValueForType(UserTypeClaimType) == InternalUserClaimValue;

        public bool IsAuthorised
        {
            get
            {
                return Ukprn.HasValue && Ukprn.Value > 0
                && !string.IsNullOrWhiteSpace(ProviderName)
                && RequiredUserRoles.Intersect(UserRoles).Any();
            }
        }

        public IEnumerable<string> GetRequiredInternalRoles()
        {
            return RequiredUserRoles?
                .Where(x => x.GetEnumCustomAttribute<UserTypeDescriptorAttribute>()?.UserType == UserType.Internal)?
                .Select(x => x.GetEnumCustomAttribute<DisplayAttribute>()?.Name);
        }

        public IEnumerable<string> GetRequiredExternalRoles()
        {
            return RequiredUserRoles?
                .Where(x => x.GetEnumCustomAttribute<UserTypeDescriptorAttribute>()?.UserType == UserType.External)?
                .Select(x => x.GetEnumCustomAttribute<DisplayAttribute>()?.Name);
        }

        private string GetClaimValueForType(string type)
        {
            return _claimPrincipal?.Claims.SingleOrDefault(c => c.Type == type)?.Value;
        }
        private IEnumerable<string> GetClaimValuesForType(string type)
        {
            return _claimPrincipal?.Claims.Where(c => c.Type == type).Select(c => c.Value);
        }
        private string GetStringClaimValueForType(string claimType, [CallerMemberName] string memberName = "")
        {
            var claimValueForType = GetClaimValueForType(claimType);
            if (string.IsNullOrEmpty(claimValueForType))
            {
                throw new Exception();
            }
            return claimValueForType;
        }
        private string GetDefaultStringClaimValueForType(string claimType)
        {
            var claimValueForType = GetClaimValueForType(claimType);
            if (string.IsNullOrEmpty(claimValueForType))
            {
                return string.Empty;
            }
            return claimValueForType;
        }
    }
}