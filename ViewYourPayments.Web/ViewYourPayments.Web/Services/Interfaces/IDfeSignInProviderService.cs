using ViewYourPayments.Core.Models.DFESignIn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFESignIn.Services.Interfaces
{
    /// <summary>
    /// Interface that defines methods to perform interactions outside of openid connect.
    /// </summary>
    public interface IDfeSignInProviderService
    {
        /// <summary>
        /// Returns roles from DFE SignIn Api for a specified userId and organisationId.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="organisationId">Organisation Id.</param>
        /// <returns>List of <see cref="Role"/>.</returns>
        Task<IEnumerable<Role>> GetRolesAsync(Guid userId, Guid organisationId);

        /// <summary>
        /// Returns roles from DFE SignIn Api for a specified userId and organisationId.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="organisationId">Organisation Id.</param>
        /// <returns>List of <see cref="Role"/>.</returns>
        Task<Organisation> GetOrganisationAsync(Guid userId, Guid organisationId);

    }
}
