using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Interfaces.User;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Services;
using Xunit;

namespace ViewYourPayments.Web.Tests.Services
{
    public class UserAuthorisationServiceTests
    {
        private readonly Mock<IFdsService> _mockFdsService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContext;
        readonly Mock<IApplicationLogger> _mockApplicationLogger;
        readonly UserAuthorisationService _userAuthorisationService;
        public UserAuthorisationServiceTests()
        {
            _mockFdsService = new Mock<IFdsService>();
            _mockApplicationLogger = new Mock<IApplicationLogger>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();
            _userAuthorisationService = new UserAuthorisationService(_mockHttpContext.Object,
                _mockFdsService.Object, _mockApplicationLogger.Object);
        }

        [Fact]
        public async Task UpdateClaimWithUkprnAndProviderName_WhenUserIsExternalAndProviderNameIsNotEmpty_ShouldNotUpdateUkprnAndProvider()
        {
            //Arrange
            var ukprn = 1234;
            var isExternalUser = true;
            var providerName = "Test User";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, providerName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext("?ukprn=1233"));

            //Action
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);

            //Assert
            var user = new ClaimsUser(claimPrincipal);
            result.Should().BeFalse();
            user.Ukprn.Value.Should().Be(ukprn);
            user.ProviderName.Should().Be(providerName);
            _mockFdsService.Verify(x => x.GetProvider(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("?ukprn=x123")]
        [InlineData("?text=22")]
        [InlineData("?ukprn=0")]
        [InlineData("?ukprn=")]
        public async Task UpdateClaimWithUkprnAndProviderName_NoUkprnInClaim_UserTypeIsInternalAndInvalidUkprnInQueryString_ShouldLogError(string queryString)
        {
            //Arrange
            var ukprn = 0;
            var isExternalUser = false;
            var providerName = "";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, providerName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext(queryString));

            //Act
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);

            //Assert
            result.Should().BeFalse();
            var user = new ClaimsUser(claimPrincipal);
            user.Ukprn.Value.Should().Be(ukprn);
            user.ProviderName.Should().Be(providerName);
            _mockFdsService.Verify(x => x.GetProvider(It.IsAny<string>()), Times.Never);
            _mockApplicationLogger.Verify(x => x.LogWarn(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public async Task UpdateClaimWithUkprnAndProviderName_NoUkprnInClaim_UserTypeIsInternalAndValidUkprnInQueryString_ShouldUpdateUkprnAndProvider()
        {
            //Arrange
            var ukprn = 0;
            var isExternalUser = false;
            var claimProviderName = "";
            var newProviderName = "--";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, claimProviderName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext("?ukprn=1233"));
            _mockFdsService.Setup(x => x.GetProvider(It.IsAny<string>())).ReturnsAsync(GetValidProvider(ukprn, newProviderName));

            //Action
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);

            //Assert
            var user = new ClaimsUser(claimPrincipal);
            result.Should().BeTrue();
            user.Ukprn.Value.Should().Be(1233);
            user.ProviderName.Should().Be(newProviderName);
        }

        [Fact]
        public async Task UpdateClaimWithUkprnAndProviderName_ValidUkprnInClaim_UserTypeIsInternalAndValidUkprnInQueryString_ShouldUpdateUkprnAndProvider()
        {
            //Arrange
            var ukprn = 123450;
            var isExternalUser = false;
            var claimProviderName = "Test";
            var newProviderName = "--";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, claimProviderName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext("?ukprn=1233"));
            _mockFdsService.Setup(x => x.GetProvider(It.IsAny<string>())).ReturnsAsync(GetValidProvider(ukprn, newProviderName));

            //Action
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);

            //Assert
            //Ukprn should be updated from query string
            var user = new ClaimsUser(claimPrincipal);
            result.Should().BeTrue();
            user.Ukprn.Value.Should().Be(1233);
            user.ProviderName.Should().Be(newProviderName);
        }

        [Fact]
        public async Task UpdateClaimWithUkprnAndProviderName_ValidUkprnAndProviderInClaim_UserTypeIsInternalAndUkprnInQueryString_NoValidProvider_ShouldThrowExecptionAndNoUkprnUpdateInClaim()
        {
            //Arrange
            var ukprn = 12345;
            var ukprnInQueryString = 1233;
            var isExternalUser = false;
            var claimProviderName = "Test";
            var newProviderName = "--";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, claimProviderName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext($"?ukprn={ukprnInQueryString}"));
            _mockFdsService.Setup(x => x.GetProvider(It.IsAny<string>())).ReturnsAsync(GetInvalidProvider());

            //Action
            //Act
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);
            var user = new ClaimsUser(claimPrincipal);
            //Assert
            result.Should().BeTrue();
            user.Ukprn.Value.Should().Be(ukprnInQueryString);
            user.ProviderName.Should().Be(newProviderName);
            _mockApplicationLogger.Verify(x => x.LogWarn(It.IsAny<string>()), Times.Exactly(1));
            _mockFdsService.Verify(x => x.GetProvider(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateClaimWithUkprnAndProviderName_ValidUkprnAndProviderInClaim_UserTypeIsInternalAndUkprnNotInQueryString_ShouldNotThrowException()
        {
            //Arrange
            var ukprn = 123450;
            var isExternalUser = false;
            var claimProviderName = "Test";
            var claimPrincipal = GetUserDetailsWithValidRole(ukprn, isExternalUser, claimProviderName);
            _mockHttpContext.Setup(x => x.HttpContext).Returns(GetHttpContext(""));
            _mockFdsService.Setup(x => x.GetProvider(It.IsAny<string>())).ReturnsAsync(GetInvalidProvider());

            //Action
            //Act
            var result = await _userAuthorisationService.UpdateClaimWithUkprnAndProviderName(claimPrincipal);
            var user = new ClaimsUser(claimPrincipal);
            //Assert
            result.Should().BeFalse();
            user.Ukprn.Value.Should().Be(ukprn);
            _mockApplicationLogger.Verify(x => x.LogWarn(It.IsAny<string>()), Times.Never);
            _mockFdsService.Verify(x => x.GetProvider(It.IsAny<string>()), Times.Never);
        }

        private HttpContext GetHttpContext(string queryString)
        {
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Referer"] = "https://test-vyp.com/paymentsummary/index";
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com?ukprn=1233"));
            defaultHttpContext.Request.QueryString = QueryString.FromUriComponent(new Uri($"https://test-vyp.com{queryString}")); ;
            return defaultHttpContext;
        }

        private ClaimsPrincipal GetUserDetailsWithValidRole(int ukprn, bool isExternalUser, string providerName)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, providerName),
                new Claim(ClaimsUser.RoleClaimType, "PaymentsViewer"),
                new Claim(ClaimsUser.UkprnClaimType, ukprn.ToString()),
                new Claim(ClaimsUser.UserTypeClaimType, isExternalUser?"1":"0"),
                new Claim(ClaimsUser.EmailClaimType, "abc@test.com"),
                }));
        }
        private IProvider GetInvalidProvider()
        {
            return null;
        }
        private IProvider GetValidProvider(int ukprn, string providerName)
        {
            return new Provider(ukprn, providerName, "1212", "2323");
        }
    }
}
