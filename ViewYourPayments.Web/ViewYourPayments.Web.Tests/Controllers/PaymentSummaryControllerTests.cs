using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.User;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Web.Controllers;
using ViewYourPayments.Web.Models;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Web.Shared;
using Xunit;

namespace ViewYourPayments.Web.Tests.Controllers
{
    //Interface
    public interface IDateTimeProvider
    {
        DateTime GetNow();
    }

    //Implementation with real DateTime.Now
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetNow() => DateTime.Now;
    }

    public class PaymentSummaryControllerTests
    {
        private readonly Mock<IOptions<AppSettings>> _appSettingsOptions;
        private readonly Mock<IUserAuthorisationService> _userAuthorisationService;
        private readonly Mock<IApplicationLogger> _applicationLogger;
        private readonly Mock<IPaymentsService> _mockPaymentService;
        private readonly PaymentSummaryController _paymentSummarController;
        readonly Mock<ISession> _sessionMock;

        private readonly AppSettings _appSetting;
        public PaymentSummaryControllerTests()
        {
            _sessionMock = new Mock<ISession>();
            _appSettingsOptions = new Mock<IOptions<AppSettings>>();
            _userAuthorisationService = new Mock<IUserAuthorisationService>();
            _applicationLogger = new Mock<IApplicationLogger>();
            _appSetting = new AppSettings { MyEsfUrl = "https://test.myesf.com", InitialPaymentsRecordsDuration = 100 };
            _mockPaymentService = new Mock<IPaymentsService>();
            _paymentSummarController = new PaymentSummaryController(_appSettingsOptions.Object, _applicationLogger.Object, _mockPaymentService.Object, _userAuthorisationService.Object);
        }

        [Theory]
        [InlineData(0, false, 3, 3)]     //page refresh with no page number, page number should take from session
        [InlineData(2, false, 3, 2)] //Changed page number, should get data with page 2
        public async Task Index_WhenRequestIsNotSubmittedToFilterRecords_ShouldUseFilterParametersFromSession(int pageNumber, bool isPageReset, int pageNumberInSession, int expectedPageNumber)
        {
            //Arrange
            var datePicker = new DatePickerViewModel(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-5));
            var defaultHttpContext = new DefaultHttpContext();
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.User = GetValidUserDetails();
            _paymentSummarController.ControllerContext.HttpContext = defaultHttpContext;

            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession(pageNumberInSession));
            _mockPaymentService.Setup(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()
                                                            , It.IsAny<int>())).ReturnsAsync(GetPayments());

            //Act
            dynamic result = await _paymentSummarController.Index("", datePicker, pageNumber, isPageReset);
            var appSession = GetApplicationSession();
            var model = (PaymentSummaryPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.Should().NotBeNull();
            model.Result.PaymentSummaries.Count.Should().Be(1);
            model.DateRange.StartDateDay.Should().Be(appSession.PaymentsSummaryFilter.FromDate.Day);
            model.DateRange.EndDateDay.Should().Be(appSession.PaymentsSummaryFilter.ToDate.Day);
            _mockPaymentService.Verify(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(),
                                                                        It.IsAny<DateTime>(), expectedPageNumber), Times.Once);
        }

        [Theory]
        [InlineData(1, true, true)] //Reset action
        [InlineData(1, false, true)] //user typed url in active login. no fiter exists
        public async Task Index_WhenRequestIsNotValidForSesionValue_ShouldUseDefaultDateAsFilter(int pageNumber, bool hasActiveFilter, bool isPageReset)
        {
            //Arrange
            var applicationSession = GetApplicationSession();
            if (!hasActiveFilter) applicationSession.PaymentsSummaryFilter = null;
            var datePicker = new DatePickerViewModel(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-5));
            var defaultHttpContext = new DefaultHttpContext();
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetValidUserDetails();
            _paymentSummarController.ControllerContext.HttpContext = defaultHttpContext;
            var defaultDateFilter = new PaymentsSummaryFilter(DateTime.UtcNow.AddDays(-_appSetting.InitialPaymentsRecordsDuration), DateTime.UtcNow, 1);
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(applicationSession);
            _mockPaymentService.Setup(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()
                                                            , It.IsAny<int>())).ReturnsAsync(GetPayments());

            //Act
            dynamic result = await _paymentSummarController.Index("", datePicker, pageNumber, isPageReset);
            var model = (PaymentSummaryPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.Should().NotBeNull();
            model.Result.PaymentSummaries.Count.Should().Be(1);
            model.DateRange.StartDateDay.Should().Be(defaultDateFilter.FromDate.Day);
            model.DateRange.EndDateDay.Should().Be(defaultDateFilter.ToDate.Day);
            _mockPaymentService.Verify(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(),
                                                                         It.IsAny<DateTime>(), 1), Times.Once);
        }


        [Theory]
        [InlineData(29, 2, 2019, 2, 10, 2019, ErrorMessages.InvalidDates)]       // incorrect date
        [InlineData(3, 2, 2019, 2, 2, 2019, ErrorMessages.FromDateShouldBeBeforeTodate)] // Start date> end date
        [InlineData(22, 2, 2017, 2, 10, 2075, ErrorMessages.FutureDateNotAllowed)] // Future dates not allowed
        public async Task Index_WhenDateRangeNotInCorrectFormat_ShouldReturnResultWithValidationError(
                                                    int startDay,
                                                    int startMonth,
                                                    int startYear,
                                                    int endDay,
                                                    int endMonth,
                                                    int endYear,
                                                    string expectedMessage)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Referer"] = "https://test-vyp.com";
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails();
            _paymentSummarController.ControllerContext.HttpContext = defaultHttpContext;
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());


            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);

            var dateRange = new DatePickerViewModel
            {
                StartDateDay = startDay,
                StartDateMonth = startMonth,
                StartDateYear = startYear,
                EndDateDay = endDay,
                EndDateMonth = endMonth,
                EndDateYear = endYear,
            };
            //Act
            dynamic result = await _paymentSummarController.Index("", dateRange);

            //Assert
            var model = (PaymentSummaryPageViewModel)result.Model;
            model.HasValidationError.Should().BeTrue();
            model.ValidationMessage.Should().Be(expectedMessage);
            model.Result.Should().BeNull();
            _mockPaymentService.Verify(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Index_WhenDateRangeGreaterThan3Years_ShouldReturnResultWith3YearsValidationError()
        {
            //Arrange 12, 2, 2017, 2, 10, 2019
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Referer"] = "https://test-vyp.com";
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails();
            _paymentSummarController.ControllerContext.HttpContext = defaultHttpContext;
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());


            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);

            var dateRange = new DatePickerViewModel
            {
                StartDateDay = 12,
                StartDateMonth = 2,
                StartDateYear = 2017,
                EndDateDay = 2,
                EndDateMonth = 10,
                EndDateYear = 2019,
            };
            //Act
            dynamic result = await _paymentSummarController.Index("", dateRange);

            //Assert
            var model = (PaymentSummaryPageViewModel)result.Model;
            model.HasValidationError.Should().BeTrue();
            model.ValidationMessage.Should().Be(ErrorMessages.MaxDateDuration);
            model.Result.Should().BeNull();
            _mockPaymentService.Verify(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Index_WhenRequestPostAndWithCorrectDateInput_ShouldReturnResultWithoutValidationError()
        {
            //Arrange
            //Referer will not appear if request is coming from ADFS
            var startDate = new DateTime(DateTime.UtcNow.Year - 1, 01, 01);
            var endDate = new DateTime(DateTime.UtcNow.Year, 01, 01);
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.Request.Headers["Referer"] = "https://test-vyp.com";
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetValidUserDetails();
            _paymentSummarController.ControllerContext.HttpContext = defaultHttpContext;
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());
            _mockPaymentService.Setup(x => x.GetPaymentSummaries(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()
                                                , It.IsAny<int>())).Returns(Task.FromResult(GetPayments()));
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            //Set date range with input params
            var dateRange = new DatePickerViewModel(startDate, endDate);
            //Act
            dynamic result = await _paymentSummarController.Index("", dateRange);

            //Assert
            var model = (PaymentSummaryPageViewModel)result.Model;
            model.HasValidationError.Should().BeFalse();
            model.Result.PaymentSummaries.Count.Should().Be(1);
            //Get payment summary method should get called with input parameters
            _mockPaymentService.Verify(x => x.GetPaymentSummaries(It.IsAny<string>(), startDate, endDate, It.IsAny<int>()), Times.Once);
        }

        private PaymentsApplicationSession GetApplicationSession(int pageNumber = 1)
        {
            var appSession = new PaymentsApplicationSession()
            {
                PaymentsSummaryFilter = new PaymentsSummaryFilter(DateTime.UtcNow.AddDays(-(_appSetting.InitialPaymentsRecordsDuration + 5)), DateTime.UtcNow, pageNumber)
            };
            return appSession;
        }

        private PaymentSummaryResultViewModel GetPayments()
        {
            return new PaymentSummaryResultViewModel
            {
                PaymentSummaries = new List<PaymentSummaryItemViewModel>
                {
                   new PaymentSummaryItemViewModel{PaymentIdentifier =122, PaymentAmount =11221.99M, PaymentDate = DateTime.UtcNow.AddDays(-100)}
                }
            };
        }

        private ClaimsPrincipal GetInvalidUserDetails()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, "TestO"),
                new Claim(ClaimsUser.RoleClaimType, "Payments"),
                new Claim(ClaimsUser.UkprnClaimType, ""),
                new Claim(ClaimsUser.UserTypeClaimType, "0"),
                }));
        }
        private IEnumerable<IProvider> GetValidProvider()
        {
            var provider = new Provider[]
              {
                  new Provider (1234,"Test","1212","2323")
              };
            return provider;
        }
        private ClaimsPrincipal GetValidUserDetails()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, "TestO"),
                new Claim(ClaimsUser.RoleClaimType, "Payments"),
                new Claim(ClaimsUser.UkprnClaimType, "555"),
                new Claim(ClaimsUser.UserTypeClaimType, "1"),
                new Claim(ClaimsUser.EmailClaimType, "abc@test.com"),
                }));
        }

    }
}
