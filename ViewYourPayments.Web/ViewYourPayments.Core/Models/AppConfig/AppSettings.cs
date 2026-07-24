using System;
using System.Collections.Generic;
using System.Text;
using ViewYourPayments.Core.Enums.User;

namespace ViewYourPayments.Core.Models.AppConfig
{
    public class AppSettings
    {
        public string Environment { get; set; }
        public Logging Logging { get; set; }
        public DfeSettings DfeSettings { get; set; }
        public DfeRoleProviderSettings DfeRoleProviderSettings { get; set; }

        public string MyEsfUrl { get; set; }
        public string AllowedHosts { get; set; }

        public SingleSignOnProviderType SingleSignOnProvider { get; set; }
        public int InitialPaymentsRecordsDuration { get; set; }
        public int InitialTransactionViewRecordsDuration { get; set; } = 30;
        public IdamsSettings IdamsSettings { get; set; }
        /// <summary>
        /// Wcf service settings for provider search service
        /// </summary>
        public ProviderSearchSettings ProviderSearchSettings { get; set; }
        public string AppInsightInstrumentationKey { get; set; }
        public string ConnectionString { get; set; }
        /// <summary>
        /// Http client setting for view your payment api
        /// </summary>
        public PaymentsApiHttpClientSettings PaymentsApiHttpClientSettings { get; set; }
        /// <summary>
        /// Http client settings to retrieve access token for payment api
        /// </summary>
        public OAuthHttpClientSettings OAuthHttpClientSettings { get; set; }
        public string PaymentDetailsPdfFileNamePrefix { get; set; }

        public string APPLOGGINGAPPINSIGHTS_INSTRUMKEY { get; set; }
        public int PaymentTransactionPageSize { get; set; }
        public string ContactUsUrl { get; set; }
        public string PrivacyUrl { get; set; }
        public string MSClarityId { get; set; }
    }
}
