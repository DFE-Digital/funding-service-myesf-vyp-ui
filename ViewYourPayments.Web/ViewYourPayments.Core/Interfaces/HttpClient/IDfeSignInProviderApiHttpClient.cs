namespace ViewYourPayments.Core.Interfaces.HttpClient
{
    public interface IDfeSignInProviderApiHttpClient
    {
        Task<T> Get<T>(string token, string requestUri);
    }
}
