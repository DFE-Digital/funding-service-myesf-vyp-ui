using FluentAssertions;
using ViewYourPayments.Core.Enums;
using ViewYourPayments.Web.Models;
using Xunit;

namespace ViewYourPayments.Web.Tests.Models
{
    public class PaymentTransactionsSortItemViewModelTests
    {
        [Theory]
        [InlineData(PaymentTransactionSortFields.Contract, PaymentTransactionSortFields.PaymentDate, SortDirection.Descending, true, true)] //Display both option
        [InlineData(PaymentTransactionSortFields.Contract, PaymentTransactionSortFields.Contract, SortDirection.Ascending, false, true)] //Display only Decending sort option
        [InlineData(PaymentTransactionSortFields.Contract, PaymentTransactionSortFields.Contract, SortDirection.Descending, true, false)] //Display only Ascending sort option
        public void ShouldDisplayNextPage_ReturnsCorrectVauleForSelectedTotalPagesAndCurrentPage(PaymentTransactionSortFields currentField,
                                                                    PaymentTransactionSortFields currentPrimarySortField,
                                                                    SortDirection currentSortDirection,
                                                                    bool expectedDisplayAscending,
                                                                    bool expectedDisplayDescending)
        {
            var model = new PaymentTransactionsSortItemViewModel
            {
                CurrentField = currentField,
                PrimarySortField = currentPrimarySortField,
                SortDirection = currentSortDirection
            };
            model.DisplayAscending.Should().Be(expectedDisplayAscending);
            model.DisplayDescending.Should().Be(expectedDisplayDescending);
        }
    }
}
