using System;
using System.Collections.Generic;
using System.Text;
using ViewYourPayments.Core.Interfaces.User;

namespace ViewYourPayments.Core.Models.User
{
    public class Provider : IProvider
    {
        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Provider()
        {
        }

        /// <summary>
        /// Create Provider.
        /// </summary>
        /// <param name="ukprn">Ukprn of the provider.</param>
        /// <param name="name">Name of the provider.</param>
        /// <param name="companyNumber">Comapany number of the provider.</param>
        /// <param name="charityRegistrationNumber">Charity registration number of the provider.</param>
        public Provider(int ukprn, string name, string companyNumber, string charityRegistrationNumber)
        {
            Ukprn = ukprn;
            Name = name;
            CompanyNumber = companyNumber;
            CharityRegistrationNumber = charityRegistrationNumber;
        }
        #endregion


        #region Base properties

        /// <summary>
        /// UkPrn of the provider
        /// </summary>
        public int Ukprn { get;  set; }

        /// <summary>
        /// The name of the provider.
        /// </summary>
        public string Name { get;  set; }

        /// <summary>
        /// Comapany number of the provider.
        /// </summary>
        public string CompanyNumber { get;  set; }

        /// <summary>
        /// Charity registration number of the provider.
        /// </summary>
        public string CharityRegistrationNumber { get;  set; }

        #endregion
    }
}
