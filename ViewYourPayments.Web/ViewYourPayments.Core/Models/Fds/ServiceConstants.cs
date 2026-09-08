namespace ViewYourPayments.Core.Models.Fds
{
    public class ServiceConstants
    {
        /// <summary>
        /// The provider query path.
        /// </summary>
        public const string LearningProviderQueryPath = "api/Provider/query";

        /// <summary>
        /// The ukprn.
        /// </summary>
        public const string Ukprn = "ukprn";

        /// <summary>
        /// The payment organisation ukprn.
        /// </summary>
        public const string PaymentOrgUkprn = "paymentorganisation.ukprn";

        /// <summary>
        /// The status.
        /// </summary>
        public const string Status = "providerStatus.providerStatusName";

        /// <summary>
        /// The provider name field.
        /// </summary>
        public const string ProviderName = "name";

        /// <summary>
        /// The payment organisation name.
        /// </summary>
        public const string PaymentOrgName = "paymentorganisation.name";

        /// <summary>
        /// The Organisation status.
        /// </summary>
        public const string OpenOrgStatus = "Open";

        /// <summary>
        /// The Field Operator Contains.
        /// </summary>
        public const string FieldOperatorContains = "contains";

        /// <summary>
        /// The Field Operator Equals.
        /// </summary>
        public const string FieldOperatorEquals = "equals";

        /// <summary>
        /// The Field Operator ISNotNull.
        /// </summary>
        public const string FieldOperatorIsNotNull = "isnotnull";
    }
}
