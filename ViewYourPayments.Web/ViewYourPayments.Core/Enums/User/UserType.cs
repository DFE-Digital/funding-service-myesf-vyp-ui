using System;
using System.Collections.Generic;
using System.Text;
using ViewYourPayments.Core.Attributes;

namespace ViewYourPayments.Core.Enums.User
{
    public enum UserType
    {
        /// <summary>
        /// Internal user type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Internal user type.
        /// </summary>
        [UserTypeClaim(ClaimValue = "0")]
        Internal = 1,

        /// <summary>
        /// External User type
        /// </summary>
        [UserTypeClaim(ClaimValue = "1")]
        External = 2,

        /// <summary>
        /// The technical support type of user.
        /// </summary>
        [UserTypeClaim(ClaimValue = "0")]
        Technical = 3,
    }
}
