namespace ViewYourPayments.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class UserTypeClaimAttribute : Attribute
    {
        /// <summary>
        /// The claim representation for the UserType that this attribute is applied to.
        /// </summary>
        public string ClaimValue { get; set; }
    }
}
