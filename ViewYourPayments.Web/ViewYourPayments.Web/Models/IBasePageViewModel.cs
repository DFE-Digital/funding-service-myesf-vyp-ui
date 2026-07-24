namespace ViewYourPayments.Web.Models
{
    public interface IBasePageViewModel
    {
        string ProviderName { get; set; }
        string UkprnNumber { get; set; }
        bool DisplayChangeProvider { get; set; }
        string PageName { get; set; }
        string ChangeProviderUrl { get; set; }
        string SignoutUrl { get; set; }
        string ViewYourSubservicesUrl { get; set; }
        string MyEsfUrl { get; set; }
        bool IsAuthorisedUser { get; set; }
        public bool DisplayHomePageLink { get; set; }
        public string ContactUsUrl { get; set; }
        public string PrivacyLink { get; set; }
        public string CookiesLink { get; set; }
        public string AccessibilityLink { get; set; }
        public string TermsAndConditionsLink { get; set; }
        string ViewCookiesUsedByEsfaLink { get; set; }
        string SelectCookiesLink { get; set; }
        bool CookiesPreferencesSet { get; set; }
        string MSClarityId { get; set; }
    }
}