using System;
using System.Collections.Generic;

namespace ViewYourPayments.Core.Models.DFESignIn
{
    /// <summary>
    /// Structure to serialise claim data returned by DFE SignIn during authentication. The only exception is Roles which is returned via a seperate api call.
    /// </summary>
    public class DfeClaims
    {
        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the organisation identifier.
        /// </summary>
        /// <value>
        /// The organisation identifier.
        /// </value>
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public IEnumerable<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the ukprn.
        /// </summary>
        /// <value>
        /// The ukprn.
        /// </value>
        public string Ukprn { get; set; }
    }   
}
