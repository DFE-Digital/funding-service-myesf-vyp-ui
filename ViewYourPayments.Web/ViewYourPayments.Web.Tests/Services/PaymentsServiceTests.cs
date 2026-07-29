using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Web.Services;
using ViewYourPayments.Web.Services.Interfaces;
using Xunit;

namespace ViewYourPayments.Web.Tests.Services
{
    public class PaymentsServiceTests
    {
        private readonly Mock<IPaymentsHttpClient> _mockPaymentsHttpClient;
        private readonly Mock<IAccessTokenService> _mockAccessTokenService;
        private readonly Mock<IApplicationLogger> _mockApplicationLogger;

        private IPaymentsService _paymentService;
        public PaymentsServiceTests()
        {
            _mockApplicationLogger = new Mock<IApplicationLogger>();
            _mockAccessTokenService = new Mock<IAccessTokenService>();
            _mockPaymentsHttpClient = new Mock<IPaymentsHttpClient>();
        }



        [Fact]
        public async Task GetPaymentSummaries_WhenPaymentSummaryIsEmpty_ShouldReturnZeroSummary()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentSummaries("123xyc", It.IsAny<string>())).Returns(Task.FromResult(GetPaymentSummaryWithNoSummaryRecord()));
            var request = "Provider/1234/Payments?dateFrom=2019/01/01&dateTo=2019/02/02&page=10";

            //Action
            var result = await _paymentService.GetPaymentSummaries("1234", new DateTime(2019, 01, 01), new System.DateTime(2019, 02, 02), 10);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentSummaries("123xyc", request), Times.Once);
            result.PaymentSummaries.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetPaymentSummaries_WhenPaymentSummaryIsNull_ShouldReturnZeroSummary()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentSummaries("123xyc", It.IsAny<string>())).
                                                Returns(Task.FromResult(GetPaymentSummaryWithPaymentSummaryAsNull()));
            var request = "Provider/1234/Payments?dateFrom=2019/01/01&dateTo=2019/02/02&page=1";

            //Action
            var result = await _paymentService.GetPaymentSummaries("1234", new DateTime(2019, 01, 01), new System.DateTime(2019, 02, 02), 1);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentSummaries("123xyc", request), Times.Once);
            result.PaymentSummaries.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetPaymentSummaries_WhenPaymentRecordReturns_ShouldReturnCorrectValue()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));

            _mockPaymentsHttpClient.Setup(x => x.GetPaymentSummaries("123xyc", It.IsAny<string>())).Returns(Task.FromResult(GetPaymentSummary()));
            var request = "Provider/1234/Payments?dateFrom=2019/01/01&dateTo=2019/02/02&page=2";

            //Action
            var result = await _paymentService.GetPaymentSummaries("1234", new DateTime(2019, 01, 01), new System.DateTime(2019, 02, 02), 2);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentSummaries("123xyc", request), Times.Once);
            result.PaymentSummaries.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetPaymentDetails_WhenPaymentDetailsResponseIsEmpty_ShouldReturnEmptyResult()
        {
            //Arrange
            var paymentIdentifier = 102020;
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentDetails("123xyc", It.IsAny<string>())).ReturnsAsync(new PaymentDetailsResponse());
            var request = $"Provider/1234/Payment/{paymentIdentifier}";

            //Action
            var result = await _paymentService.GetPaymentDetails("1234", paymentIdentifier);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentDetails("123xyc", request), Times.Once);
            result.PaymentLines.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetPaymentDetails_WhenPaymentDetailsResponseHasPaymentLines_ShouldReturnCorrectResult()
        {
            //Arrange
            var paymentIdentifier = 102020;
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentDetails("123xyc", It.IsAny<string>())).ReturnsAsync(GetPaymentDetailsResponse());
            var request = $"Provider/1234/Payment/{paymentIdentifier}";

            //Action
            var result = await _paymentService.GetPaymentDetails("1234", paymentIdentifier);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentDetails("123xyc", request), Times.Once);
            result.PaymentLines.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetPaymentTransactions_WhenNoTransactionsAreRetruned_ShouldReturnEmptyList()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentTransactions("123xyc", It.IsAny<string>(), It.IsAny<PaymentTransactionSearchCriteria>())).
                                                Returns(Task.FromResult(new PaymentTransactionsResponse()));
            var request = "Provider/Transactions";
            var searchCriteria = new PaymentTransactionSearchCriteria
            {
                Ukprn = "1234",
                StartDate = new DateTime(2019, 01, 01),
                EndDate = new DateTime(2019, 02, 02),
                PageNumber = 1,
                PageSize = 7,
                PrimarySortField = PaymentTransactionSortFields.Contract,
                SortDirection = SortDirection.Ascending
            };
            var result = await _paymentService.GetPaymentTransactions(searchCriteria);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentTransactions("123xyc", request,
                It.Is<PaymentTransactionSearchCriteria>(p => p.Ukprn == searchCriteria.Ukprn &&
                   p.PageNumber == searchCriteria.PageNumber && p.PageSize == searchCriteria.PageSize
                   && p.SearchTerms == searchCriteria.SearchTerms
                   && p.StartDate.ToShortDateString() == searchCriteria.StartDate.ToShortDateString()
                   && p.EndDate.ToShortDateString() == searchCriteria.EndDate.ToShortDateString())
                ), Times.Once);
            result.PaymentTransactions.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetPaymentTransactions_WhenTransactionsAreRetruned_ShouldReturnRecords()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetPaymentTransactions("123xyc", It.IsAny<string>(), It.IsAny<PaymentTransactionSearchCriteria>())).
                                                Returns(Task.FromResult(GetPaymentTransactions()));
            var request = "Provider/Transactions";

            //Action
            var searchCriteria = new PaymentTransactionSearchCriteria
            {
                Ukprn = "1234",
                StartDate = new DateTime(2019, 01, 01),
                EndDate = new DateTime(2019, 02, 02),
                PageNumber = 1,
                PageSize = 10,
                PrimarySortField = PaymentTransactionSortFields.Contract,
                SortDirection = SortDirection.Ascending
            };
            var result = await _paymentService.GetPaymentTransactions(searchCriteria);

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetPaymentTransactions("123xyc", request,
                It.Is<PaymentTransactionSearchCriteria>(p => p.Ukprn == searchCriteria.Ukprn &&
                   p.PageNumber == searchCriteria.PageNumber && p.PageSize == searchCriteria.PageSize
                   && p.SearchTerms == searchCriteria.SearchTerms
                   && p.StartDate.ToShortDateString() == searchCriteria.StartDate.ToShortDateString()
                   && p.EndDate.ToShortDateString() == searchCriteria.EndDate.ToShortDateString())
                ), Times.Once);
            result.PaymentTransactions.Count().Should().Be(2);
            result.TotalPages.Should().Be(3);
            result.Ukprn.Should().Be("1234");
        }

        [Fact]
        public async Task GetUniqueTransactionDescriptions_WhenDesciptionListIsNull_ShouldReturnZeroResults()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            _mockPaymentsHttpClient.Setup(x => x.GetUniqueTransactionsDescriptions("123xyc", It.IsAny<string>())).
                                                Returns(Task.FromResult(Enumerable.Empty<string>()));
            var request = "Provider/1234/UniqueTransactionDescriptions?dateFrom=2019/01/01&dateTo=2019/02/02";
            //Action
            var result = await _paymentService.GetUniqueTransactionDescriptions("1234", new DateTime(2019, 01, 01), new DateTime(2019, 02, 02));

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetUniqueTransactionsDescriptions("123xyc", request), Times.Once);
            result.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetUniqueTransactionDescriptions_WhenDesciptionListHasItems_ShouldReturnCorrectResults()
        {
            //Arrange
            _paymentService = new PaymentsService(_mockPaymentsHttpClient.Object,
                                    _mockApplicationLogger.Object, _mockAccessTokenService.Object);
            _mockAccessTokenService.Setup(x => x.GetPaymentApiAccessToken()).Returns(Task.FromResult("123xyc"));
            var expectedItmes = new string[] { "Test", "Test and Try", "Try Again" };
            _mockPaymentsHttpClient.Setup(x => x.GetUniqueTransactionsDescriptions("123xyc", It.IsAny<string>())).
                                                Returns(Task.FromResult(expectedItmes.AsEnumerable()));
            var request = "Provider/1234/UniqueTransactionDescriptions?dateFrom=2019/01/01&dateTo=2019/02/02";
            //Action
            var result = await _paymentService.GetUniqueTransactionDescriptions("1234", new DateTime(2019, 01, 01), new DateTime(2019, 02, 02));

            //Assert
            _mockPaymentsHttpClient.Verify(x => x.GetUniqueTransactionsDescriptions("123xyc", request), Times.Once);
            result.Count().Should().Be(3);
        }

        private PaymentSummaryResponse GetPaymentSummary()
        {
            return new PaymentSummaryResponse
            {
                PaymentSummaries = new[] {
                        new PaymentSummaryItem {
                            PaymentAmount = 100.02M,
                            PaymentDate = new DateTime(2019, 01, 01),
                            PaymentIdentifier = 12345
                        },
                        new PaymentSummaryItem {
                            PaymentAmount = 200.10M,
                            PaymentDate = new DateTime(2019, 01, 01),
                            PaymentIdentifier = 12345,
                        }
                    },
                DateFrom = new DateTime(2019, 01, 01),
                DateTo = new DateTime(2019, 06, 06),
                CurrentPage = 1,
                TotalRecords = 2,
                TotalPages = 1
            };

        }
        private PaymentDetailsResponse GetPaymentDetailsResponse()
        {
            return new PaymentDetailsResponse
            {
                PaymentLines = new[] {
                        new PaymentLineItem {
                           ContractNumber="2333",
                           BudgetDescription="Test Budget",
                           PaymentLineAmount =101.3m,
                           PaymentLineDescription = "Test this payment line"
                        },
                        new PaymentLineItem {
                             ContractNumber="4444",
                           BudgetDescription="Test Budget 11",
                           PaymentLineAmount =1991.3m,
                           PaymentLineDescription = "Test this payment line one"
                        }
                    },
                PaymentAmount = 2092.6m,
                PaymentDate = DateTime.Now,
                ProviderUkprn = "1234"
            };

        }
        private PaymentSummaryResponse GetPaymentSummaryWithNoSummaryRecord()
        {
            return new PaymentSummaryResponse
            {
                PaymentSummaries = Array.Empty<PaymentSummaryItem>(),
                DateFrom = new DateTime(2019, 01, 01),
                DateTo = new DateTime(2019, 06, 06)
            };

        }

        private PaymentSummaryResponse GetPaymentSummaryWithPaymentSummaryAsNull()
        {
            return new PaymentSummaryResponse
            {
                DateFrom = new DateTime(2019, 01, 01),
                DateTo = new DateTime(2019, 06, 06)
            };

        }


        private PaymentTransactionsResponse GetPaymentTransactions()
        {
            return new PaymentTransactionsResponse
            {
                PaymentLines = new[] {
                        new PaymentLineItem {
                           ContractNumber="2333",
                           BudgetDescription="Test Budget",
                           PaymentLineAmount =101.3m,
                           PaymentLineDescription = "Test this payment line"
                        },
                        new PaymentLineItem {
                             ContractNumber="4444",
                           BudgetDescription="Test Budget 11",
                           PaymentLineAmount =1991.3m,
                           PaymentLineDescription = "Test this payment line one"
                        }
                    },
                BudgetGroupSummary = new BudgetGroupSummary
                {
                    BudgetGroups = new[]
                    {
                        new BudgetGroupItem{BudgetDescription = "Test Budget", BudgetAmount=101.3M},
                        new BudgetGroupItem {BudgetDescription ="Test Budget 11", BudgetAmount =1991.2M},
                        new BudgetGroupItem {BudgetDescription ="Test Budget New", BudgetAmount =5091.2M},
                        new BudgetGroupItem {BudgetDescription ="Other", BudgetAmount =-11.4M}
                    }
                },
                DateFrom = new DateTime(2019, 01, 01),
                DateTo = new DateTime(2019, 06, 06),
                CurrentPage = 1,
                TotalRecords = 11,
                ProviderUkprn = "1234",
                PageSize = 5,
                ProviderFinanceVendorCode = "Test"
            };

        }
    }
}
