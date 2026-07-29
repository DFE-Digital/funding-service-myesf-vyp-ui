using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Models;

namespace ViewYourPayments.Web.Shared
{
    public class BasePageViewModelHelper
    {
        public static void PopulateBasePageViewModel(IBasePageViewModel viewModel, AppSettings appSettings, string pageName, ClaimsUser user)
        {
            if (viewModel == null) return;
            viewModel.ProviderName = user.ProviderName;
            viewModel.UkprnNumber = user.Ukprn.ToString();
            viewModel.ChangeProviderUrl = $"{appSettings.MyEsfUrl}/provider-search/stop-impersonating-provider";
            viewModel.ViewYourSubservicesUrl = $"{appSettings.MyEsfUrl}/view-your-subservices";
            viewModel.PageName = pageName;
            viewModel.SignoutUrl = "account/logout";
            viewModel.MyEsfUrl = appSettings.MyEsfUrl;
            viewModel.DisplayHomePageLink = true;
            viewModel.ContactUsUrl = appSettings.ContactUsUrl;
            if (user.IsInternalUser)
            {
                viewModel.DisplayChangeProvider = true;
                viewModel.ProviderName = $"Viewing as {user.ProviderName}";
            }
            viewModel.IsAuthorisedUser = user.IsAuthorised;
            viewModel.ViewCookiesUsedByEsfaLink = $"{appSettings.MyEsfUrl}/cookie-details";
            viewModel.SelectCookiesLink = $"{appSettings.MyEsfUrl}/cookies";
            viewModel.PrivacyLink = $"{appSettings.PrivacyUrl}";
            viewModel.CookiesLink = $"{appSettings.MyEsfUrl}/cookies";
            viewModel.AccessibilityLink = $"{appSettings.MyEsfUrl}/accessibility";
            viewModel.TermsAndConditionsLink = $"{appSettings.MyEsfUrl}/terms-and-conditions";
            viewModel.MSClarityId = appSettings.MSClarityId ?? "MSClarityTestId";
        }
    }
}
