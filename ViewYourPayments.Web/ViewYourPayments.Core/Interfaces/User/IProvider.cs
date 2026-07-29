namespace ViewYourPayments.Core.Interfaces.User
{
    public interface IProvider
    {
        /// <summary>
        /// The name of the provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// UKPRN of the provider.
        /// </summary>
        int Ukprn { get; }

        /// <summary>
        /// Comapany number of the provider.
        /// </summary>
        string CompanyNumber { get; }

        /// <summary>
        /// Charity registration number of the provider.
        /// </summary>
        string CharityRegistrationNumber { get; }
    }
}
