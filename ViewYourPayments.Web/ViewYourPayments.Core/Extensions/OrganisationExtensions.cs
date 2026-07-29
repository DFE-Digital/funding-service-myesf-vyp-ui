using ViewYourPayments.Core.Models.DFESignIn;

namespace ViewYourPayments.Core.Extensions
{
    /// <summary>
    /// Extension class that extends <see cref="Organisation"/>.
    /// </summary>
    public static class OrganisationExtensions
    {
        /// <summary>
        /// Returns boolean indicating whether the organisation associated with the user is a "Dfe" org. Used to derive internal users.
        /// </summary>
        /// <param name="organisation">Organisation that the logged-in user belongs to.</param>
        /// <param name="dfeLegacyCodeId">The default legacy code id for "DFE" organisation defined in DFE SignIn.</param>
        /// <returns>Boolean true or false.</returns>
        public static bool IsInternalDFE(this Organisation organisation, string dfeLegacyCodeId)
        {
            return !string.IsNullOrEmpty(organisation.LegacyId) && dfeLegacyCodeId.Split(',').Any(id => id.Trim() == organisation.LegacyId);
        }
    }
}
