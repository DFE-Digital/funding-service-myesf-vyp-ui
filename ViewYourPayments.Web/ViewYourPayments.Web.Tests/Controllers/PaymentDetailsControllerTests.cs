using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
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
    public class PaymentDetailsControllerTests
    {
        private readonly Mock<IOptions<AppSettings>> _mockAppSettingsOptions;
        private readonly Mock<IUserAuthorisationService> _mockUserAuthorisationService;
        private readonly Mock<IApplicationLogger> _mockApplicationLogger;
        private readonly Mock<IPaymentsService> _mockPaymentService;
        private readonly PaymentDetailsController _paymentDetailsController;
        private readonly Mock<IDocumentManagementService> _mockDocumentManagementService;
        readonly Mock<ISession> _sessionMock;

        private readonly AppSettings _appSetting;
        public PaymentDetailsControllerTests()
        {
            _sessionMock = new Mock<ISession>();
            _mockAppSettingsOptions = new Mock<IOptions<AppSettings>>();
            _mockUserAuthorisationService = new Mock<IUserAuthorisationService>();
            _mockApplicationLogger = new Mock<IApplicationLogger>();
            _appSetting = new AppSettings { MyEsfUrl = "https://test.myesf.com", InitialPaymentsRecordsDuration = 100 };
            _mockPaymentService = new Mock<IPaymentsService>();
            _mockDocumentManagementService = new Mock<IDocumentManagementService>();
            Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
            _paymentDetailsController = new PaymentDetailsController(_mockAppSettingsOptions.Object,
                                                                        _mockUserAuthorisationService.Object,
                                                                        _mockApplicationLogger.Object,
                                                                        _mockPaymentService.Object,
                                                                        _mockDocumentManagementService.Object);
            _paymentDetailsController.Url = urlHelperMock.Object;

        }

        [Theory]
        [InlineData("https://xyz.com")]       // Retruning from some other link i.e. Dfe Signin
        [InlineData("")] //Refresh action
        public async Task Index_WhenRequestIsNotMadeFromPaymentSummaryPage_ShouldDisplayResult(string referer)
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Referer"] = referer;
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetExternalUserDetails();
            _paymentDetailsController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession(0));
            _mockPaymentService.Setup(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new PaymentDetailsResultViewModel());

            //Act
            dynamic result = await _paymentDetailsController.Index(234242);
            var model = (PaymentsDetailsPageViewModel)result.Model;
            //Assert
            _mockPaymentService.Verify(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            model.Should().NotBeNull();
            model.BackToPaymentSummaryPageUrl.Should().Be("?page=1");
        }

        [Fact]
        public async Task Index_WhenRequestIsMadeFromPaymentSummaryPageWithInternalUser_ShouldDisplayResult()
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Referer"] = "https://test-vyp.com/paymentsummary/index";
            defaultHttpContext.Request.Host = HostString.FromUriComponent(new Uri("https://test-vyp.com"));
            defaultHttpContext.User = GetInternalUserDetails();
            _paymentDetailsController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession(2));
            _mockPaymentService.Setup(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(GetPaymentDetails());

            //Act
            dynamic result = await _paymentDetailsController.Index(234242);
            var model = (PaymentsDetailsPageViewModel)result.Model;
            //Assert
            _mockPaymentService.Verify(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            model.Should().NotBeNull();
            model.Result.PaymentLines.Count().Should().Be(2);
            model.BackToPaymentSummaryPageUrl.Should().Be("?page=2");

        }

        [Fact]
        public async Task CreateCsvFile_WhenNoPaymentExists_ShouldReturnNullResponse()
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext();
            byte[] contentResponse = Array.Empty<byte>();
            defaultHttpContext.User = GetInternalUserDetails();
            _paymentDetailsController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());
            _mockPaymentService.Setup(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new PaymentDetailsResultViewModel());

            //Act
            var result = await _paymentDetailsController.DownloadCsv(234242);
            //Assert
            _mockApplicationLogger.Verify(x => x.LogWarn(It.IsAny<string>()), Times.Once);
            _mockPaymentService.Verify(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _mockDocumentManagementService.Verify(x => x.TransformRecordsToCsvBytes(It.IsAny<IEnumerable<object>>()), Times.Never);
        }

        [Fact]
        public async Task CreateCsvFile_WhenPaymentExists_ShouldReturnFileResult()
        {
            //Arrange
            var defaultHttpContext = new DefaultHttpContext
            {
                User = GetInternalUserDetails()
            };
            var content = "VendorNumber,Ukprn,Date,ContractNumber,BudgetGroup,Description,Amount\r\nP0002646,10007405,08/07/2019,APPS-1666,Apprenticeships Carry-in,16 to 18 non-levy additional payments for employers Reconciliation Jul 2019,500.00\r\n";
            var contentResponse = System.Text.Encoding.UTF8.GetBytes(content);
            _paymentDetailsController.ControllerContext.HttpContext = defaultHttpContext;
            _mockAppSettingsOptions.Setup(x => x.Value).Returns(_appSetting);
            _mockUserAuthorisationService.Setup(x => x.GetValueFromCookies(It.IsAny<string>())).Returns(GetApplicationSession());
            _mockPaymentService.Setup(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(GetPaymentDetails());
            _mockDocumentManagementService.Setup(x => x.TransformRecordsToCsvBytes(Array.Empty<object>()))
                                                    .Returns(Array.Empty<byte>());

            //Act
            dynamic result = await _paymentDetailsController.DownloadCsv(234242);
            //Assert
            var model = (FileResult)result;
            model.ContentType.Should().Be("text/csv");
            model.Should().NotBeNull();
            _mockPaymentService.Verify(x => x.GetPaymentDetails(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _mockDocumentManagementService.Verify(x => x.TransformRecordsToCsvBytes(It.IsAny<IEnumerable<object>>()), Times.Once);
        }

        private PaymentsApplicationSession GetApplicationSession(int pageNumber = 1)
        {
            var appSession = new PaymentsApplicationSession()
            {
                PaymentsSummaryFilter = new PaymentsSummaryFilter(DateTime.UtcNow.AddDays(-(_appSetting.InitialPaymentsRecordsDuration + 5)), DateTime.UtcNow, pageNumber)
            };
            return appSession;
        }

        private IEnumerable<PaymentDetailsCsvExportItem> GetCsvData()
        {
            return new[]
            {
             new PaymentDetailsCsvExportItem { Amount = 110, VendorNumber = "P2233", Date = DateTime.Now.ToShortDateString(), BudgetGroup = "sdfsf", Ukprn = "sfsf" }
            };

        }


        private PaymentDetailsResultViewModel GetPaymentDetails()
        {
            return new PaymentDetailsResultViewModel
            {
                PaymentAmount = 1102.22M,
                PaymentDate = DateTime.Now,
                PaymentLines = new List<PaymentDetailViewModel>
                {
                  new PaymentDetailViewModel{ContractNumber="2222",
                                            BudgetDescription = "TestBudgetDesc",
                                            PaymentLineAmount=102.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData"},
                  new PaymentDetailViewModel{ContractNumber="3232",
                                            BudgetDescription = "TestBudgetDesc1",
                                            PaymentLineAmount=1002.11M,
                                            PaymentDate = DateTime.Now,
                                            PaymentLineDescription="TestuserData1"}
                }
            };
        }

        private ClaimsPrincipal GetInternalUserDetails()
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
                new Claim(ClaimsUser.UserTypeClaimType, "0"),
                }));
        }

        private ClaimsPrincipal GetInvlidUserDetails()
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
        private ClaimsPrincipal GetExternalUserDetails()
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

        private byte[] GetPaymentData()
        {
            return Array.Empty<byte>();
        }

    }
}
