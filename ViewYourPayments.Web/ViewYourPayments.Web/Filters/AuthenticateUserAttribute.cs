using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ViewYourPayments.Core.Models.User;

namespace ViewYourPayments.Web.Filters
{
    public class AuthenticateUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentUser = new ClaimsUser(context.HttpContext.User);

            if (!currentUser.IsAuthorised)
            {
                context.Result = new RedirectToActionResult("UnAuthorisedUser", "Account", null);
            }
            base.OnActionExecuting(context);
        }
    }
}