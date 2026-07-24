using ViewYourPayments.Core.Models.DFESignIn;
using FluentAssertions;
using Xunit;
using ViewYourPayments.Core.Extensions;

namespace DFESignIn.Tests
{
    /// <summary>
    /// Tests for <see cref="OrganisationExtensions"/>.
    /// </summary>
    public class OrganisationExtensionsTests
    {
        private readonly string orgLegacyCodeId = "999";

        /// <summary>
        /// Tests if the specified org is dfe.
        /// </summary>
        /// <param name="expectedValue">if set to <c>true</c> [expected value].</param>
        /// <param name="defaultDfeLegacyCodeId">The org legacy code identifier.</param>
        [Theory]
        [InlineData(true, "999")]
        [InlineData(false, "123")]
        [InlineData(false, "")]
        [InlineData(true, "999,123")]
        [InlineData(true, "999 , 123")]
        public void IsInternalDFE_WhenLegacyCodeMatchesWithConfiguredValue_ShouldReturnCorrectResult(bool expectedValue, string defaultDfeLegacyCodeId)
        {
            //Arrange
            var fakeOrganisation = new Organisation
            {
                LegacyId = orgLegacyCodeId
            };

            //Act
            var actual = fakeOrganisation.IsInternalDFE(defaultDfeLegacyCodeId);

            //Assert
            actual.Should().Be(expectedValue);
        }
    }
}
