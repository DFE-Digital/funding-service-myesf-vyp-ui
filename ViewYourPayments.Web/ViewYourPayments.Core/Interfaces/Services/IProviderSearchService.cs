using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces.User;

namespace ViewYourPayments.Core.Interfaces.Services
{
    public interface IProviderSearchService
    {
        /// <summary>
        /// Get the providers with a matching UKPRN.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the provider to match on.</param>
        /// <returns>The list of matching providers.</returns>
        Task<IEnumerable<IProvider>> GetProvidersByUkprn(int ukprn);

        /// <summary>
        /// Get the active providers with a matching UKPRN.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the active provider to match on.</param>
        /// <returns>The list of matching providers.</returns>
        Task<IEnumerable<IProvider>> GetActiveProvidersByUkprn(int ukprn);
    }
}
