namespace ViewYourPayments.Core.Models.DFESignIn
{
    /// <summary>
    /// Structure to serialise organisation item data returned by DFE SignIn during authentication.
    /// </summary>
    public class OrgItem
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
    }
}
