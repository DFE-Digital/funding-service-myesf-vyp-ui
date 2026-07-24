namespace ViewYourPayments.Web.Models
{
    public class BasePageViewModel: IBasePageViewModel
    {
        public string ProviderName { get; set; }
        public string UkprnNumber { get; set; }
        public bool DisplayChangeProvider { get; set; }
        public string PageName { get; set; }
        public string ChangeProviderUrl { get; set; }
        public string SignoutUrl { get; set; }
        public string ViewYourSubservicesUrl { get; set; }
        public string MyEsfUrl { get; set; }
        public bool DisplayHomePageLink { get; set; }
        public bool IsAuthorisedUser { get; set; }
        public string ContactUsUrl { get; set; }
        public string PrivacyLink { get; set; }
        public string CookiesLink { get; set; }
        public string AccessibilityLink { get; set; }
        public string TermsAndConditionsLink { get; set; }
        public string ViewCookiesUsedByEsfaLink { get; set; }
        public string SelectCookiesLink { get; set; }
        public bool CookiesPreferencesSet { get; set; }
        public string MSClarityId { get; set; }
    }
}