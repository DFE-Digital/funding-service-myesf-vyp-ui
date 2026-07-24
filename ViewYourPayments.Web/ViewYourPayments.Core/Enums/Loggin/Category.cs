using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.Enums.Loggin
{
    /// <summary>
    /// Enumeration representing possible logging categories.
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// Log information releted to provider search.
        /// </summary>
        ProviderSearch = 0,
        /// <summary>
        /// Log information releted to data search.
        /// </summary>
        DataSearchService = 1,
        /// <summary>
        /// Log information releted to user authentication.
        /// </summary>
        AuthenticationService = 2,
        VypWebApplication = 3,
        DataImport = 4
    }
}
