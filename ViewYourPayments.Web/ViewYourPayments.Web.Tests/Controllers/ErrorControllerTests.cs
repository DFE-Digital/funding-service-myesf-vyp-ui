using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using ViewYourPayments.Core.Attributes;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Core.Enums.User;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Controllers;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;
using Xunit;

namespace ViewYourPayments.Web.Tests.Controllers
{
    public class ErrorControllerTests
    {
        private readonly Mock<IOptions<AppSettings>> _mockAppSettingsOptions;
        private readonly Mock<IUserAuthorisationService> _mockUserAuthorisationService;
        private readonly Mock<IApplicationLogger> _mockApplicationLogger;
        private readonly ErrorController _errorController;
        private readonly Mock<IDocumentManagementService> _mockDocumentManagementService;
        readonly Mock<ISession> _sessionMock;
        private readonly AppSettings _appSetting;

        public ErrorControllerTests()
        {
            _sessionMock = new Mock<ISession>();
            _mockAppSettingsOptions = new Mock<IOptions<AppSettings>>();
            _mockUserAuthorisationService = new Mock<IUserAuthorisationService>();
            _mockApplicationLogger = new Mock<IApplicationLogger>();
            _appSetting = new AppSettings { MyEsfUrl = "https://test.myesf.com", InitialPaymentsRecordsDuration = 100 };
            _mockDocumentManagementService = new Mock<IDocumentManagementService>();
            Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
            _errorController = new ErrorController(_mockAppSettingsOptions.Object, _mockApplicationLogger.Object);
        }
        [Theory]
        [InlineData(UserType.Internal, new[] { UserRole.ViewAsProvider }, UserRole.SfsAdmin)]
        [InlineData(UserType.External, new[] { UserRole.ViewPaymentHistory }, UserRole.ViewContractsAndAgreements)]
        public void AccessDenied_WhenUserWithoutProperRoleLogin_ShouldDisplayAccessDeniedWithRequiredRoles(UserType userType, UserRole[] requiredUserRoles, UserRole loggedInUserRole)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetUserDetails(userType, loggedInUserRole);
            _errorController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession(0));

            //Act
            dynamic result = _errorController.AccessDenied();
            var model = (UnauthorisedErrorViewModel)result.Model;

            //Assert
            model.requiredRoles.Should().Equal(GetUserRolesDisplayNames(requiredUserRoles));
        }

        [Theory]
        [InlineData(UserType.Internal, UserRole.ViewAsProvider)]
        [InlineData(UserType.External, UserRole.ViewPaymentHistory)]
        public void AccessDenied_WhenUserWithProperRoleNavigateToAccessDeniedPage_ShouldRedirectToPaymentSummaryPage(UserType userType, UserRole loggedInUserRole)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetUserDetails(userType, loggedInUserRole);
            _errorController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession(0));

            //Act
            dynamic result = _errorController.AccessDenied();

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PaymentSummary", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        private ClaimsPrincipal GetUserDetails(UserType userType, UserRole loggedInUserRole)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, "TestO"),
                new Claim(ClaimsUser.RoleClaimType, loggedInUserRole.ToString()),
                new Claim(ClaimsUser.UkprnClaimType, "555"),
                new Claim(ClaimsUser.UserTypeClaimType, userType.GetEnumCustomAttribute<UserTypeClaimAttribute>().ClaimValue),
                new Claim(ClaimsUser.EmailClaimType, "abc@test.com"),
                }));
        }

        private string[] GetUserRolesDisplayNames(UserRole[] userRoles)
        {
            return userRoles?.Select(role => role.GetEnumCustomAttribute<DisplayAttribute>().Name)?.ToArray();
        }

        private PaymentsApplicationSession GetApplicationSession(int pageNumber = 1)
        {
            var appSession = new PaymentsApplicationSession()
            {
                PaymentsSummaryFilter = new PaymentsSummaryFilter(DateTime.UtcNow.AddDays(-(_appSetting.InitialPaymentsRecordsDuration + 5)), DateTime.UtcNow, pageNumber)
            };
            return appSession;
        }
    }
}
