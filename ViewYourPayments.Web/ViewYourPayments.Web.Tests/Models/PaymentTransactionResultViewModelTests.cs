using ViewYourPayments.Web.Models;
using Xunit;
using FluentAssertions;


namespace ViewYourPayments.Web.Tests.Models
{
    public class PaymentTransactionResultViewModelTests
    {
        [Theory]
        [InlineData(5,0,false)]
        [InlineData(5,1, true)]
        [InlineData(5, 5, false)]
        [InlineData(5, 6, false)]
        public void ShouldDisplayNextPage_ReturnsCorrectVauleForSelectedTotalPagesAndCurrentPage(int totalPages,int currentPage,bool expectedResult)
        {
            var model = new PaymentTransactionResultViewModel { TotalPages = totalPages, CurrentPage = currentPage };
            model.ShouldDisplayNextPage.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, true)]
        public void ShouldDisplayPreviousPage_ReturnCorrectVauleForSelectedCurrentPage(int currentPage, bool expectedResult)
        {
            var model = new PaymentTransactionResultViewModel {CurrentPage = currentPage };
            model.ShouldDisplayPreviousPage.Should().Be(expectedResult);
        }
    }
}
