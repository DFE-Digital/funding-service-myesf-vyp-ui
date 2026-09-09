using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.Fds;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Core.Services;
using Xunit;

namespace ViewYourPayments.Core.Tests
{
    public class FdsServiceTests
    {
        private readonly HttpMessageHandler _httpMessageHandler = Mock.Of<HttpMessageHandler>();
        private readonly Mock<IApplicationLogger> _mockLogger = new Mock<IApplicationLogger>(MockBehavior.Strict);
        private const string learningProviderQueryPath = "https://test.com/api/Provider/query";

        [Fact]
        public async Task GetProvider_WhenCalledWith_SearchField_PaymentOrganisation_Name()
        {
            //Arrange
            var response = new
            {
                totalcount = 1,
                pagenumber = 1,
                data = new List<object>
                {
                    new
                    {
                        name = "test",
                        ukprn = 1,
                        status = "Open",
                        type = "test",
                        subtype = "test",
                        child = new List<object>()
                        {
                            new
                            {
                                name = "test",
                                ukprn = 2,
                                status = "Open",
                                type = "test",
                                subtype = "test"
                            }
                        }
                    }
                }
            };

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, response);

            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            var result = await fdsService.GetProvider("1");

            //Assert
            result.Should().BeEquivalentTo(new Provider(1, "test", null, null));
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 1);
            _mockLogger.Verify();
        }

        [Fact]
        public async Task GetProvider_Returns_Provider_Result()
        {
            //Arrange            
            var providerResponse = new
            {
                totalcount = 1,
                pagenumber = 1,
                data = new List<Object>
                {
                    new
                    {
                        name = "test",
                        ukprn = 2,
                        status = "Open",
                        type = "test",
                        subtype = "test",
                        managementgroup = new
                        {
                            name = "test",
                            ukprn = 1,
                            status = "Open",
                            type = "test",
                            subtype = "test"
                        }
                    }
                }
            };

            var paymentOrgResponse = new
            {
                totalcount = 0,
                pagenumber = 1,
                data = Enumerable.Empty<object>()
            };

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, paymentOrgResponse, providerResponse);

            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            var result = await fdsService.GetProvider("1");

            //Assert
            result.Should().BeEquivalentTo(new Provider(2, "test", null, null));
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 2);
            _mockLogger.Verify();
        }

        [Fact]
        public async Task GetProvider_Return_Empty_Data()
        {
            //Arrange
            var matResponse = new
            {
                totalcount = 1,
                pagenumber = 1,
                data = new List<object>
                {
                    new
                    {
                        name = "test",
                        ukprn = 2,
                        status = "Open",
                        type = "test",
                        subtype = "test"
                    }
                }
            };

            var providerResponse = new
            {
                totalcount = 0,
                pagenumber = 1,
                data = Enumerable.Empty<object>()
            };

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, matResponse, providerResponse);

            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            var result = await fdsService.GetProvider("2");

            //Assert
            result.Should().BeEquivalentTo(new Provider(2, "test", null, null));
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 1);
            _mockLogger.Verify();
        }

        [Fact]
        public async Task GetProvider_Returns_Null()
        {
            //Arrange
            var response = new
            {
                totalcount = 0,
                pagenumber = 1,
                data = Enumerable.Empty<object>()
            };

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, response, response);
            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            var result = await fdsService.GetProvider("1");

            //Assert
            result.Should().BeEquivalentTo((Provider)null);
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 2);
            _mockLogger.Verify();
        }

        [Fact]
        public async Task GetProvider_Thorws_Exception_WhenCalledWith_SearchField_PaymentOrganisation_Ukprn()
        {
            //Arrange

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            _mockLogger
            .Setup(s => s.LogException(It.IsAny<Exception>()))
            .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, shouldReturnErrorForPaymentOrg: true, paymentOrganisationresponse: "InternalServerError");

            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            Func<Task> func = async () => await fdsService.GetProvider("1");

            //Assert
            await func.Should().ThrowAsync<HttpRequestException>();
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 1);
            _mockLogger.Verify();
        }

        [Fact]
        public async Task GetProvinder_Thorws_Exception_WhenCalledWith_SearchField_Ukprn()
        {
            //Arrange
            var response = new
            {
                totalcount = 0,
                pagenumber = 1,
                data = Enumerable.Empty<object>()
            };

            _mockLogger
             .Setup(s => s.LogInfo(It.IsAny<string>()))
             .Verifiable();

            _mockLogger
            .Setup(s => s.LogException(It.IsAny<Exception>()))
            .Verifiable();

            SetupMessageHandler(learningProviderQueryPath, response, "InternalServerError", shouldReturnErrorForProvider: true);
            var fdsService = new FdsService(GetHttpClient(), GetAppSettings(), _mockLogger.Object);

            //Act
            Func<Task> func = async () => await fdsService.GetProvider("55");

            //Assert
            await func.Should().ThrowAsync<HttpRequestException>();
            VerifyMessageHandler(HttpMethod.Post, learningProviderQueryPath, 2);
            _mockLogger.Verify();
        }

        private IOptions<AppSettings> GetAppSettings()
        {
            return new OptionsWrapper<AppSettings>(new AppSettings
            {
                FdsApiHttpClientSettings = new FdsApiHttpClientSettings
                {
                    BaseAddress = "https://test.com",
                    ApimSubscriptionKey = "12345678"
                }
            });
        }

        private HttpClient GetHttpClient()
         => new HttpClient(_httpMessageHandler);

        private void SetupMessageHandler(string url, object paymentOrganisationresponse = null, object providerresponse = null, bool shouldReturnErrorForPaymentOrg = false, bool shouldReturnErrorForProvider = false)
        {
            Mock.Get(_httpMessageHandler)
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.Equals(url)),
                ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken token) =>
            {
                if (request.Content != null)
                {
                    object responseContent = null;

                    HttpResponseMessage expectedResponse = new HttpResponseMessage();

                    string jsonBody = request.Content.ReadAsStringAsync().Result;

                    JObject json = JObject.Parse(jsonBody);

                    string searchFieldName = json["SearchCriteria"][0][0].ToObject<SearchCriteria>().FieldName;

                    if (searchFieldName == ServiceConstants.PaymentOrgName || searchFieldName == ServiceConstants.PaymentOrgUkprn)
                    {
                        expectedResponse.StatusCode = shouldReturnErrorForPaymentOrg ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;
                        responseContent = paymentOrganisationresponse ?? SetFdsServiceResponse();
                    }
                    else if (searchFieldName == ServiceConstants.ProviderName || searchFieldName == ServiceConstants.Ukprn)
                    {
                        expectedResponse.StatusCode = shouldReturnErrorForProvider ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;
                        responseContent = providerresponse ?? SetFdsServiceResponse(false, false);
                    }

                    expectedResponse.Content = new StringContent(
                        JsonConvert.SerializeObject(responseContent),
                        Encoding.UTF8,
                        "application/json");

                    return Task.FromResult(expectedResponse);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            });
        }

        private void VerifyMessageHandler(HttpMethod httpMethod, string expectedUri, int noOfTimesHit)
        {
            Mock.Get(_httpMessageHandler)
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Exactly(noOfTimesHit),
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method.Equals(httpMethod)
                        && req.RequestUri.Equals(new Uri(expectedUri))),
                    ItExpr.IsAny<CancellationToken>());
        }

        private static List<Provider> GetProviders(bool isMatResponse = true, bool isListOfProvider = true)
        {
            int ukprn = isMatResponse ? 12345678 : 10000055;

            var providers = new List<Provider>
            {
                new Provider(ukprn, "test", null, null)
            };

            if (isListOfProvider)
            {
                providers.Add(new Provider(ukprn + 3, "test", null, null));
            }

            return providers;
        }

        private static object SetFdsServiceResponse(bool isPaymentOrgResponse = true, bool isListOfProviders = true)
        {
            int ukprn = isPaymentOrgResponse ? 12345678 : 10000055;
            var provider = new
            {
                name = "test",
                ukprn = ukprn,
                status = "Open",
                child = new List<object>
                {
                    new
                    {
                        name = "test",
                        ukprn = ukprn + 1,
                        status = "Open"
                    },
                    new
                    {
                        name = "test",
                        ukprn = ukprn + 2,
                        status = "Open"
                    }
                }
            };

            var providerTwo = new
            {
                name = "test",
                ukprn = ukprn + 3,
                status = "Open",
                child = new List<object>
                {
                    new
                    {
                        name = "test",
                        ukprn = ukprn + 4,
                        status = "Open"
                    },
                    new
                    {
                        name = "test",
                        ukprn = ukprn + 5,
                        status = "Open"
                    }
                }
            };

            var providers = new List<object>
            {
                provider
            };

            if (isListOfProviders)
            {
                providers.Add(providerTwo);
            }

            return new
            {
                totalcount = providers.Count(),
                pagenumber = 1,
                data = providers
            };
        }
    }
}
