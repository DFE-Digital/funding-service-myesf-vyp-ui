namespace ViewYourPayments.Core.Models.DFESignIn
{
    /// <summary>
    /// Organisation associated to a DFE Sign-In user.
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public OrgItem Category { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public OrgItem Type { get; set; }

        /// <summary>
        /// Gets or sets the urn.
        /// </summary>
        /// <value>
        /// The urn.
        /// </value>
        public string Urn { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the ukprn.
        /// </summary>
        /// <value>
        /// The ukprn.
        /// </value>
        public string Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the establishment number.
        /// </summary>
        /// <value>
        /// The establishment number.
        /// </value>
        public string EstablishmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public OrgItem Status { get; set; }

        /// <summary>
        /// Gets or sets the closed on.
        /// </summary>
        /// <value>
        /// The closed on.
        /// </value>
        public string ClosedOn { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        /// <value>
        /// The telephone.
        /// </value>
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public OrgItem Region { get; set; }

        /// <summary>
        /// Gets or sets the local authority.
        /// </summary>
        /// <value>
        /// The local authority.
        /// </value>
        public OrgItem LocalAuthority { get; set; }

        /// <summary>
        /// Gets or sets the phase of education.
        /// </summary>
        /// <value>
        /// The phase of education.
        /// </value>
        public OrgItem PhaseOfEducation { get; set; }

        /// <summary>
        /// Gets or sets the statutory low age.
        /// </summary>
        /// <value>
        /// The statutory low age.
        /// </value>
        public int? StatutoryLowAge { get; set; }

        /// <summary>
        /// Gets or sets the statutory high age.
        /// </summary>
        /// <value>
        /// The statutory high age.
        /// </value>
        public int? StatutoryHighAge { get; set; }

        /// <summary>
        /// Gets or sets the legacy identifier.
        /// </summary>
        /// <value>
        /// The legacy identifier.
        /// </value>
        public string LegacyId { get; set; }

        /// <summary>
        /// Gets or sets the company registration number.
        /// </summary>
        /// <value>
        /// The company registration number.
        /// </value>
        public string CompanyRegistrationNumber { get; set; }
    }
}

