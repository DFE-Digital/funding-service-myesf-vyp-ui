using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ViewYourPayments.Core.Interfaces.User;
using ViewYourPayments.Core.Models.User;

namespace ViewYourPayments.Web.Shared
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
