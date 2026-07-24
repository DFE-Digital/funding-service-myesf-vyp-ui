using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Web.Services.Interfaces;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Web.Controllers;
using FluentAssertions;
using Xunit;
using System.Security.Claims;
using ViewYourPayments.Core.Models.User;
using ViewYourPayments.Core.Interfaces.User;
using System.Collections.Generic;
using ViewYourPayments.Web.Models;
using System;
using ViewYourPayments.Web.Shared;
using System.Linq;
using ViewYourPayments.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Core.DTOs;
using Newtonsoft.Json.Linq;

namespace ViewYourPayments.Web.Tests.Controllers
{
    public class PaymentTransactionsControllerTests
    {
        private readonly Mock<IOptions<AppSettings>> _appSettingsOptions;
        private readonly Mock<IUserAuthorisationService> _userAuthorisationService;
        private readonly Mock<IApplicationLogger> _applicationLogger;
        private readonly Mock<IPaymentsService> _mockPaymentService;
        private readonly PaymentTransactionsController _paymentTransactionController;
        private readonly Mock<IDocumentManagementService> _mockDocumentManagementService;
        readonly Mock<ISession> _sessionMock;

        private readonly AppSettings _appSetting;
        public PaymentTransactionsControllerTests()
        {
            _sessionMock = new Mock<ISession>();
            _appSettingsOptions = new Mock<IOptions<AppSettings>>();
            _userAuthorisationService = new Mock<IUserAuthorisationService>();
            Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
            _applicationLogger = new Mock<IApplicationLogger>();
            _appSetting = new AppSettings { MyEsfUrl = "https://test.myesf.com", InitialTransactionViewRecordsDuration = 100, PaymentTransactionPageSize =20 };
            _mockPaymentService = new Mock<IPaymentsService>();
            _mockDocumentManagementService = new Mock<IDocumentManagementService>();
            _paymentTransactionController = new PaymentTransactionsController(_appSettingsOptions.Object, _applicationLogger.Object, _mockPaymentService.Object, _userAuthorisationService.Object, _mockDocumentManagementService.Object);
            _paymentTransactionController.Url = urlHelperMock.Object;
        }

        [Theory]
        [InlineData(0, false, 1, PaymentTransactionSortFields.PaymentDate, SortDirection.Descending, null)]                      // User has requested browser page refresh
        [InlineData(0, false, 1, PaymentTransactionSortFields.PaymentDate, SortDirection.Descending, "")]                      // User has requested browser page refresh
        [InlineData(1, false, 5, PaymentTransactionSortFields.LineAmount, SortDirection.Ascending, "Test,Test1")] //Changed page number
        [InlineData(1, false, 5, PaymentTransactionSortFields.LineAmount, SortDirection.Ascending, "  Test, Test1, Test2  ,  Test3 ,Test4  ")] // where there are whitespaces between search terms
        public async Task Index_WhenRequestIsSubmittedToFilterRecords_ShouldUseFiltersFromTheForm(int pageNumber,
                                                                                           bool isPageReset,
                                                                                           int currentPageNumberInSession,
                                                                                           PaymentTransactionSortFields primarySortField,
                                                                                           SortDirection sortDirection,
                                                                                           string searchTerms)
        {
            //Arrange
            var dateRange = new DatePickerViewModel(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-5));
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails("6666");
            var appSession = GetApplicationSession(currentPageNumberInSession, primarySortField, sortDirection, searchTerms);
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;

            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());

            //Act
            dynamic result = await _paymentTransactionController.Index(dateRange, string.Empty, pageNumber, isPageReset);
            var model = (PaymentTransactionsPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.Should().NotBeNull();
            model.Result.PaymentTransactions.Count().Should().Be(4);
            model.TransactionFilter.DateRange.StartDateDay.Should().Be(appSession.PaymentTransactionsFilter.FromDate.Day);
            model.TransactionFilter.DateRange.EndDateDay.Should().Be(appSession.PaymentTransactionsFilter.ToDate.Day);

            _mockPaymentService.Verify();
            var searchTermsString = JArray.Parse(string.IsNullOrWhiteSpace(model.TransactionFilter.CurrentSearchTermsKeyValuePairs) ? "[]" : model.TransactionFilter.CurrentSearchTermsKeyValuePairs);
            Assert.DoesNotContain(searchTermsString.SelectMany(item => item.Children<JProperty>()), prop => prop.Value.ToString().StartsWith(" ") || prop.Value.ToString().EndsWith(" "));
        }

        [Theory]
        [InlineData(0, false ,1, 1,PaymentTransactionSortFields.PaymentDate,SortDirection.Descending,"")]                      // User has requested browser page refresh
        [InlineData( 1, false ,5, 1, PaymentTransactionSortFields.LineAmount, SortDirection.Ascending,"Test,Test1")] //Changed page number
        public async Task Index_WhenRequestIsNotSubmittedToFilterRecords_ShouldUseFilterParametersFromSession( int pageNumber, 
                                                                                                                bool isPageReset,
                                                                                                                int currentPageNumberInSession,
                                                                                                                int expectedPageNumber,
                                                                                                                PaymentTransactionSortFields primarySortField,
                                                                                                                SortDirection sortDirection,
                                                                                                                string searchTerms)
        {
            //Arrange
            var dateRange = new DatePickerViewModel(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-5));
            var defaultHttpContext = new DefaultHttpContext();
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.User = GetValidUserDetails("6666");
            var appSession = GetApplicationSession(currentPageNumberInSession, primarySortField, sortDirection, searchTerms);
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;

            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());

            //Act
            dynamic result = await _paymentTransactionController.Index(dateRange, string.Empty, pageNumber, isPageReset);
            var model = (PaymentTransactionsPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.Should().NotBeNull();
            model.Result.PaymentTransactions.Count().Should().Be(4);
            model.TransactionFilter.DateRange.StartDateDay.Should().Be(appSession.PaymentTransactionsFilter.FromDate.Day);
            model.TransactionFilter.DateRange.EndDateDay.Should().Be(appSession.PaymentTransactionsFilter.ToDate.Day);

            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
                (p => p.Ukprn == "6666" 
                && p.PageNumber == expectedPageNumber
                && p.StartDate.ToShortDateString() == appSession.PaymentTransactionsFilter.FromDate.ToShortDateString()
                && p.EndDate.ToShortDateString() == appSession.PaymentTransactionsFilter.ToDate.ToShortDateString()
                && p.PrimarySortField == primarySortField && p.SortDirection == sortDirection
                && string.Join(",", p.SearchTerms) == searchTerms
                )), Times.Once);
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
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails();
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
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
            dynamic result = await _paymentTransactionController.Index(dateRange,string.Empty);

            //Assert
            var model = (PaymentTransactionsPageViewModel)result.Model;
            model.HasValidationError.Should().BeTrue();
            model.ValidationMessage.Should().Be(expectedMessage);
            model.Result.Should().BeNull();
            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>()), Times.Never);
        }

        [Fact]
        public async Task Index_WhenDateRangeGreaterThan3Years_ShouldReturnResultWith3YearsValidationError()
        {
            //Arrange 12, 2, 2017, 2, 10, 2019
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails();
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
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
            dynamic result = await _paymentTransactionController.Index(dateRange, string.Empty);

            //Assert
            var model = (PaymentTransactionsPageViewModel)result.Model;
            model.HasValidationError.Should().BeTrue();
            model.ValidationMessage.Should().Be(ErrorMessages.MaxDateDuration);
            model.Result.Should().BeNull();
            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>()), Times.Never);
        }

        [Theory]
        [InlineData(true, true)] //Reset action
        [InlineData(false, true)] //user typed url in active login. no fiter exists
        public async Task Index_WhenRequestIsNotValidForSesionValue_ShouldUseDefaultValuesAsFilter( bool hasActiveFilter, bool isPageReset)
        {
            //Arrange
            
            var applicationSession = GetApplicationSession();
            if (!hasActiveFilter) applicationSession.PaymentsSummaryFilter = null;
            var dateRange = new DatePickerViewModel(DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(-5));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = HttpMethods.Post;
            defaultHttpContext.User = GetValidUserDetails("3434");
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            var defaultDateFilter = new PaymentsTransactionsFilter(DateTime.UtcNow.AddDays(-100), DateTime.UtcNow, 1);
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(applicationSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>()))
                                                            .ReturnsAsync(GetPaymentTransactions());

            //Act
            dynamic result = await _paymentTransactionController.Index(dateRange, string.Empty, 0, isPageReset);
            var model = (PaymentTransactionsPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.PaymentTransactions.Count().Should().Be(4);
            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
               (p => p.Ukprn == "3434"
               && p.PageNumber == 1
               && p.StartDate.ToShortDateString() == defaultDateFilter.FromDate.ToShortDateString()
               && p.EndDate.ToShortDateString() == defaultDateFilter.ToDate.ToShortDateString()
               && string.Join(",", p.SearchTerms) == ""
               && p.PrimarySortField == PaymentTransactionSortFields.PaymentDate
               && p.SortDirection == SortDirection.Descending
               )), Times.Once);
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
            defaultHttpContext.User = GetValidUserDetails("1234");
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());

            //Set date range with input params
            var dateRange = new DatePickerViewModel(startDate, endDate);
            //Act
            dynamic result = await _paymentTransactionController.Index(dateRange, string.Empty);

            //Assert
            var model = (PaymentTransactionsPageViewModel)result.Model;
            model.HasValidationError.Should().BeFalse();
            model.Result.PaymentTransactions.Count().Should().Be(4);
            //Get payment transaction method should get called with input parameters
            //TODO
            ////_mockPaymentService.Verify(x => x.GetPaymentTransactions("1234",startDate,endDate,1, It.IsAny<int>()
            ////                                                        , PaymentTransactionSortFields.PaymentDate,
            ////                                                        SortDirection.Descending), Times.Once);
        }

        [Fact]
        public async Task DownloadCsv_WhenNoPaymentExists_ShouldReturnNullResponse()
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            byte[] contentResponse = Array.Empty<byte>();
            defaultHttpContext.User = GetInternalUserDetails("555");
            var appSession = GetApplicationSession();
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(new PaymentTransactionResultViewModel());

            //Act
            var result = await _paymentTransactionController.DownloadCsv();

            //Assert

            _applicationLogger.Verify(x => x.LogWarn(It.IsAny<string>()), Times.Once);
            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
            (p => p.Ukprn == "555"
            && p.StartDate.ToShortDateString() == appSession.PaymentTransactionsFilter.FromDate.ToShortDateString()
            && p.EndDate.ToShortDateString() == appSession.PaymentTransactionsFilter.ToDate.ToShortDateString()
            && p.PrimarySortField == appSession.PaymentTransactionsFilter.PrimarySortField && p.SortDirection == appSession.PaymentTransactionsFilter.SortDirection
            && p.PageNumber == appSession.PaymentTransactionsFilter.PageNumber
            && string.Join(",", p.SearchTerms) == ""
            )), Times.Once);

            _mockDocumentManagementService.Verify(x => x.TransformRecordsToCsvBytes(It.IsAny<IEnumerable<object>>()), Times.Never);
        }

        [Theory]
        [InlineData(PaymentTransactionSortFields.PaymentLineDescription, SortDirection.Ascending)]
        [InlineData(PaymentTransactionSortFields.Contract, SortDirection.Descending)]
        public async Task DownloadCsv_WhenPaymentExists_ShouldReturnFileResult(PaymentTransactionSortFields currentPrimarySortField, SortDirection currentSortDirection)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext
            {
                User = GetInternalUserDetails("555")
            };
            var searchTermsInSession = "Test, TestValue1, Test value 3";
            var appSession = GetApplicationSession(1,currentPrimarySortField, currentSortDirection, searchTermsInSession);
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());
            _mockDocumentManagementService.Setup(x => x.TransformRecordsToCsvBytes(Array.Empty<object>()))
                                                    .Returns(Array.Empty<byte>());

            //Act
            dynamic result = await _paymentTransactionController.DownloadCsv();
            //Assert
            var model = (FileResult)result;
            model.ContentType.Should().Be("text/csv");
            model.Should().NotBeNull();

            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
            (p => p.Ukprn == "555"
            && p.PageNumber == appSession.PaymentTransactionsFilter.PageNumber
            && p.PageSize == 0
            && p.StartDate.ToShortDateString() == appSession.PaymentTransactionsFilter.FromDate.ToShortDateString()
            && p.EndDate.ToShortDateString() == appSession.PaymentTransactionsFilter.ToDate.ToShortDateString()
            && p.PrimarySortField == currentPrimarySortField && p.SortDirection == currentSortDirection
            && string.Join(",", p.SearchTerms) == appSession.PaymentTransactionsFilter.SearchTerms
            )), Times.Once);
            _mockDocumentManagementService.Verify(x => x.TransformRecordsToCsvBytes(It.IsAny<IEnumerable<object>>()), Times.Once);
        }

        [Theory]
        [InlineData(1, "", "")]
        [InlineData(1, null, null)]
        public async Task ChangeOrder_SortFieldsNotInCorrectFormat_ShouldUseDefaultValue(int pageNumber,
                                                                                                string primarySortField,
                                                                                                string sortDirection)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.User = GetValidUserDetails("6666");
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            var appSession = GetApplicationSession(pageNumber);

            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());

            //Act
            dynamic result = await _paymentTransactionController.ChangeOrder(primarySortField, sortDirection);
            var model = (PaymentTransactionsPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.Result.Should().NotBeNull();
            model.Result.PaymentTransactions.Count().Should().Be(4);
            model.TransactionFilter.DateRange.StartDateDay.Should().Be(appSession.PaymentTransactionsFilter.FromDate.Day);
            model.TransactionFilter.DateRange.EndDateDay.Should().Be(appSession.PaymentTransactionsFilter.ToDate.Day);

            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
            (p => p.Ukprn == "6666"
                    && p.StartDate.ToShortDateString() == appSession.PaymentTransactionsFilter.FromDate.ToShortDateString()
                    && p.EndDate.ToShortDateString() == appSession.PaymentTransactionsFilter.ToDate.ToShortDateString()
                    && p.PrimarySortField == PaymentTransactionSortFields.PaymentDate 
                    && p.SortDirection == SortDirection.Descending
                    )), Times.Once);
        }

        [Theory]
        [InlineData(1, "LineAmount", "Descending", PaymentTransactionSortFields.PaymentLineDescription, SortDirection.Ascending, PaymentTransactionSortFields.LineAmount, SortDirection.Descending)]
        [InlineData(5, "PaymentLineDescription", "Ascending", PaymentTransactionSortFields.Contract, SortDirection.Ascending, PaymentTransactionSortFields.PaymentLineDescription, SortDirection.Ascending)]
        public async Task ChangeOrder_WhenSortOrderChange_ShouldUpdateSortValueAndPageNumberInSession(int pageNumber,
                                                                                                               string primarySortField,
                                                                                                                string sortDirection,
                                                                                                                PaymentTransactionSortFields currentPrimarySortField,
                                                                                                                SortDirection currentSortDirection,
                                                                                                                PaymentTransactionSortFields expectedPrimarySortField,
                                                                                                                SortDirection expectedSortDirection)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            //Referer will not appear if request is coming from ADFS
            defaultHttpContext.Request.Method = HttpMethods.Get;
            defaultHttpContext.User = GetValidUserDetails("6666");
            var searchTermsInSession = "Test, TestValue1, Test value 3";
            _paymentTransactionController.ControllerContext.HttpContext = defaultHttpContext;
            var appSession = GetApplicationSession(pageNumber, currentPrimarySortField, currentSortDirection, searchTermsInSession);
            _appSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _userAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(appSession);
            _mockPaymentService.Setup(x => x.GetPaymentTransactions(It.IsAny<PaymentTransactionSearchCriteria>())).ReturnsAsync(GetPaymentTransactions());

            //Act
            dynamic result = await _paymentTransactionController.ChangeOrder(primarySortField, sortDirection);
            var model = (PaymentTransactionsPageViewModel)result.Model;
            //Assert
            model.HasValidationError.Should().BeFalse();
            model.TransactionFilter.DateRange.StartDateDay.Should().Be(appSession.PaymentTransactionsFilter.FromDate.Day);
            model.TransactionFilter.DateRange.EndDateDay.Should().Be(appSession.PaymentTransactionsFilter.ToDate.Day);
            _mockPaymentService.Verify(x => x.GetPaymentTransactions(It.Is<PaymentTransactionSearchCriteria>
                (p => p.Ukprn == "6666"
                && p.StartDate.ToShortDateString() == appSession.PaymentTransactionsFilter.FromDate.ToShortDateString()
                && p.EndDate.ToShortDateString() == appSession.PaymentTransactionsFilter.ToDate.ToShortDateString()
                && p.PrimarySortField == expectedPrimarySortField && p.SortDirection == expectedSortDirection
                && string.Join(",", p.SearchTerms) == searchTermsInSession
                )), Times.Once);
        }

        private IEnumerable<PaymentTransactionsCsvExportItem> GetCsvData()
        {
            return new[]
            {
             new PaymentTransactionsCsvExportItem { Amount = 110, VendorNumber = "P2233", Date = DateTime.Now.ToShortDateString(), BudgetGroup = "sdfsf", Ukprn = "sfsf" }
            };
        }
        private ClaimsPrincipal GetInternalUserDetails(string ukprn)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, "TestO"),
                new Claim(ClaimsUser.RoleClaimType, "Payments"),
                new Claim(ClaimsUser.UkprnClaimType, ukprn),
                new Claim(ClaimsUser.UserTypeClaimType, "0"),
                }));
        }

        private PaymentsApplicationSession GetApplicationSession(int pageNumber = 1,
                                                PaymentTransactionSortFields sortField = PaymentTransactionSortFields.PaymentDate,
                                                SortDirection sortDirection = SortDirection.Descending,
                                                string filterTerms = "")
        {
            var appSession = new PaymentsApplicationSession()
            {
                PaymentTransactionsFilter = new PaymentsTransactionsFilter(DateTime.UtcNow.AddDays(-(_appSetting.InitialPaymentsRecordsDuration + 5)), DateTime.UtcNow, pageNumber)
            };
            appSession.PaymentTransactionsFilter.PrimarySortField = sortField;
            appSession.PaymentTransactionsFilter.SortDirection = sortDirection;
            appSession.PaymentTransactionsFilter.SearchTerms = filterTerms;
            return appSession;
        }

        private PaymentTransactionResultViewModel GetPaymentTransactions()
        {
            return new PaymentTransactionResultViewModel
            {
                PaymentTransactions = new List<PaymentTransactionViewModel>
                {
                    new PaymentTransactionViewModel{ContractNumber="2222",
                                            BudgetDescription = "TestBudgetDesc",
                                            PaymentLineAmount=102.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData11"},
                  new PaymentTransactionViewModel{ContractNumber="3232",
                                            BudgetDescription = "TestBudgetDesc2",
                                            PaymentLineAmount=1002.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData222"},
                  new PaymentTransactionViewModel{ContractNumber="4343",
                                            BudgetDescription = "TestBudgetDesc3",
                                            PaymentLineAmount=4002.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData333"},
                  new PaymentTransactionViewModel{ContractNumber="5353",
                                            BudgetDescription = "TestBudgetDesc4",
                                            PaymentLineAmount=9002.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData444"}
                },
                CurrentPage =1,
                Ukprn = "1234",
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
        private ClaimsPrincipal GetValidUserDetails(string ukprn="555")
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimsUser.FirstNameClaimType, "Test"),
                new Claim(ClaimsUser.LastNameClaimType, "TestL"),
                new Claim(ClaimsUser.OrganisationNameClaimType, "TestO"),
                new Claim(ClaimsUser.RoleClaimType, "Payments"),
                new Claim(ClaimsUser.UkprnClaimType, ukprn),
                new Claim(ClaimsUser.UserTypeClaimType, "1"),
                new Claim(ClaimsUser.EmailClaimType, "abc@test.com"),
                }));
        }
    }
}
