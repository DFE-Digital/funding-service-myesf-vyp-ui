namespace ViewYourPayments.Core.Models.User
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContactServiceDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Contact Service Group Name
        /// </summary>
        public string GroupName { get; set; }
    }
}
