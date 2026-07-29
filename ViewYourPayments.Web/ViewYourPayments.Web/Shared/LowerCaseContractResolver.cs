using Newtonsoft.Json.Serialization;

namespace ViewYourPayments.Web.Shared
{
    /// <summary>
    /// Newtonsoft resolver to transform searalise property name with lower case.
    /// </summary>
    public class LowerCaseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Convert property name as lower.
        /// </summary>
        /// <param name="propertyName">Selected property name.</param>
        /// <returns>Retruns property name.</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
