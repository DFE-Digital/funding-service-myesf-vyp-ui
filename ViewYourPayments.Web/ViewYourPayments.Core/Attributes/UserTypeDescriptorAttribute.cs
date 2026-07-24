using System;
using ViewYourPayments.Core.Enums.User;

namespace ViewYourPayments.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class UserTypeDescriptorAttribute : Attribute
    {
        /// <summary>
        /// The type of user.
        /// </summary>
        public UserType UserType { get; set; }
    }
}
