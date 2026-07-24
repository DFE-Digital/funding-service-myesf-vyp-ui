
using System.Collections.Generic;
using ViewYourPayments.Core.Models.User;

namespace ViewYourPayments.Core.Interfaces.User
{
    public interface IUser
    {
        string Name { get; }

        /// <summary>
        /// Returns a flag indicating if the User has been authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Get the User's UKPRN.
        /// </summary>
        int? Ukprn { get; }

        /// <summary>
        /// Gets the User's Provider Name.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Returns the role this user has as a string.
        /// </summary>
        string RolesDisplay { get; }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        string FirstName { get; }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        string LastName { get; }

        /// <summary>
        /// Gets a list of the support users email addresses.
        /// </summary>
        IList<string> SupportUserEmailAddresses { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Gets a list of user roles
        /// </summary>
        IEnumerable<UserRole> UserRoles { get; }
        /// <summary>
        /// Returns a flag indicating if the user has at least one of the required permissions.
        /// </summary>
        /// <param name="requiredUserPermissions">The permissions that the user must have at least one of.</param>
        /// <returns><c>true</c> if the user has at least one permission; otherwise, <c>false</c>.</returns>
        bool HasAnyRole(IEnumerable<UserRole> requiredUserPermissions);

        /// <summary>
        /// Determines whether the user is an internal user.
        /// </summary>
        /// <returns><c>true</c> if the user is an internal user.</returns>
        bool IsInternalUser { get; }

        /// <summary>
        /// Determine whether user is authorised.
        /// </summary>
        bool IsAuthorised { get; }

    }
}
