using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Services.Interfaces;

namespace ViewYourPayments.Web.Extensions
{
    public class UserInfoClaims : IClaimsTransformation
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserAuthorisationService _userAuthorisationService;

        public UserInfoClaims(
           IHttpContextAccessor httpContext,
           IUserAuthorisationService userAuthorisationService)
        {
            _userAuthorisationService = userAuthorisationService;
            _httpContext = httpContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var cloneValue = principal.Clone();
            var newUpdateRequried = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(cloneValue);
            if (newUpdateRequried)
            {
                //store the tokens
                Thread.CurrentPrincipal = cloneValue;
                var auth = await _httpContext.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cloneValue, auth?.Properties);
            }
            return cloneValue;
        }
    }
}
