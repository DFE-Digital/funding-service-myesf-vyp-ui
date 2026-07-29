using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Models;
using ViewYourPayments.Web.Services;
using ViewYourPayments.Web.Services.Interfaces;
using Xunit;

namespace ViewYourPayments.Web.Tests.Services
{
    public class AccessTokenServiceTests
    {
        private readonly Mock<IApplicationLogger> _mockApplicationLogger;
        private readonly Mock<IPaymentsApiOAuthHttpClient> _mockPaymentApiOAuthHttpClient;
        private readonly IMemoryCache _memoryCache;

        private IAccessTokenService _accessTokenService;
        public AccessTokenServiceTests()
        {
            _mockApplicationLogger = new Mock<IApplicationLogger>();
            _mockPaymentApiOAuthHttpClient = new Mock<IPaymentsApiOAuthHttpClient>();
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            _memoryCache = serviceProvider.GetService<IMemoryCache>();
        }
        [Fact]
        public async Task GetPaymentApiAccessToken_WhenNoValidTokenInMemoryCache_ShouldCallGetToken()
        {
            //Arrange
            _accessTokenService = new AccessTokenService(_mockPaymentApiOAuthHttpClient.Object,
                                                         _mockApplicationLogger.Object,
                                                         _memoryCache);

            _mockPaymentApiOAuthHttpClient.Setup(x => x.GetToken()).Returns(Task.FromResult(GetToken(120)));

            //Action
            var result = await _accessTokenService.GetPaymentApiAccessToken();
            //Assert
            _mockPaymentApiOAuthHttpClient.Verify(x => x.GetToken(), Times.Once);
            result.Should().Be("1234");
        }

        [Fact]
        public async Task GetPaymentSummaries_WhenValidTokenInMemoryCache_ShouldNotCallGetToken()
        {
            //Arrange
            string token = "12333";
            _memoryCache.Set("VypApiToken", token, DateTimeOffset.Now.AddSeconds(100));
            _accessTokenService = new AccessTokenService(_mockPaymentApiOAuthHttpClient.Object,
                                                         _mockApplicationLogger.Object,
                                                         _memoryCache);

            _mockPaymentApiOAuthHttpClient.Setup(x => x.GetToken()).Returns(Task.FromResult(GetToken(120)));

            //Action
            var result = await _accessTokenService.GetPaymentApiAccessToken();
            //Assert
            _mockPaymentApiOAuthHttpClient.Verify(x => x.GetToken(), Times.Never);
            result.Should().Be(token);

        }

        [Fact]
        public async Task GetPaymentSummaries_WhenExpriedTokenInMemoryCache_ShouldCallGetToken()
        {
            //Arrange
            string token = "12333";
            _memoryCache.Set("VypApiToken", token, DateTimeOffset.Now.AddSeconds(-1));
            _accessTokenService = new AccessTokenService(_mockPaymentApiOAuthHttpClient.Object,
                                                         _mockApplicationLogger.Object,
                                                         _memoryCache);

            _mockPaymentApiOAuthHttpClient.Setup(x => x.GetToken()).Returns(Task.FromResult(GetToken(120)));

            //Action
            var result = await _accessTokenService.GetPaymentApiAccessToken();
            //Assert
            _mockPaymentApiOAuthHttpClient.Verify(x => x.GetToken(), Times.Once);
            result.Should().Be("1234");

        }

        private OAuthToken GetToken(int expiryDurationInMinute)
        {
            return new OAuthToken
            {
                AccessToken = "1234",
                ExpiresIn = expiryDurationInMinute,
                RefreshToken = "22323",
            };
        }
    }
}
