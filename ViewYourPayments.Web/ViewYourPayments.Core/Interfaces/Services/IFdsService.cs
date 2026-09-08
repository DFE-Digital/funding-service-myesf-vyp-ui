using ViewYourPayments.Core.Interfaces.User;

namespace ViewYourPayments.Core.Interfaces.Services
{
    public interface IFdsService
    {
        /// <summary>
        /// Gets the provider details from FDS for the given UKPRN.
        /// </summary>
        /// <param name="ukprn">The ukprn to search for.</param>
        /// <returns>The provider info.</returns>
        Task<IProvider> GetProvider(string ukprn);
    }
}
