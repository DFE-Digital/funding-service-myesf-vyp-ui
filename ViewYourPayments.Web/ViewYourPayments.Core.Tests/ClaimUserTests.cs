using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using ViewYourPayments.Core.Models.User;
using Xunit;


namespace ViewYourPayments.Core.Tests
{
    public class ClaimUserTests
    {

        [Theory]
        [InlineData(1333)]
        [InlineData(0)]
        public void Ukprn_WhenUkPrnIsNumber_ShouldReturnCorrectNumber(int ukprn)
        {
            // Arrange
            var claimPrincipal = GetuserClaimByUkprn(ukprn.ToString());
            var user = new ClaimsUser(claimPrincipal);
            // Act
            var ukprnNumber = user.Ukprn;

            //Assert
            ukprnNumber.Should().Be(ukprn);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ukprn_WhenUkPrnIsNullOrEmpty_ShouldReturnNullValue(string ukprn)
        {
            // Arrange
            var claimPrincipal = GetuserClaimByUkprn(ukprn);
            var user = new ClaimsUser(claimPrincipal);

            // Act
            var ukprnNumber = user.Ukprn;

            //Assert
            ukprnNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("22d3")]
        [InlineData("11.33")]
        public void Ukprn_WhenUkPrnHasNotAValidInteger_ShouldThrowException(string ukprn)
        {
            // Arrange
            var claimPrincipal = GetuserClaimByUkprn(ukprn.ToString());

            // Act

            var user = new ClaimsUser(claimPrincipal);
            //Assert
            var ex = Assert.Throws<Exception>(() => user.Ukprn);
        }

        [Fact]
        public void IsInternalUser_true()
        {
            // Arrange
            var claimPrincipal = GetuserClaimByType(false);
            var claimsUser = new ClaimsUser(claimPrincipal);
            // Act
            var result = claimsUser.IsInternalUser;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInternalUser_false()
        {
            // Arrange
            var claimPrincipal = GetuserClaimByType(true);
            var claimsUser = new ClaimsUser(claimPrincipal);
            // Act
            var result = claimsUser.IsInternalUser;

            //Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("", "Test", UserRole.None, false)]
        [InlineData("1", "", UserRole.ViewAsProvider, false)]
        [InlineData(null, "", UserRole.ViewAsProvider, false)]
        [InlineData("333", null, UserRole.ViewAsProvider, false)]
        [InlineData("212", "Test", UserRole.None, false)]
        [InlineData("212", "Test", UserRole.ViewAsProvider, true)]
        [InlineData("212", "Test", UserRole.ViewPaymentHistory, true)]
        public void IsAuthorised_WhenUkprnIsEmpty_ShouldRetrun_False(string ukprn, string providerName, UserRole role, bool expectedResult)
        {
            // Arrange
            var claimPrincipal = GetUserClaim(ukprn, providerName, role.ToString(), true);
            var claimsUser = new ClaimsUser(claimPrincipal);
            // Act
            var result = claimsUser.IsAuthorised;

            //Assert
            result.Should().Be(expectedResult);
        }

        private ClaimsPrincipal GetuserClaimByType(bool isExternal)
        {
            return GetUserClaim("1234", "TestProvider", "Payments", isExternal);
        }
        private ClaimsPrincipal GetuserClaimByUkprn(string ukprn)
        {
            return GetUserClaim(ukprn, "TestProvider", "Payments", true);
        }

        private ClaimsPrincipal GetUserClaim(string ukprn, string providerName, string roleName, bool isExternal = false)
        {
            var claimList = new List<Claim>();
            AddClaim(ClaimTypes.Name, "example name", claimList);
            AddClaim(ClaimTypes.NameIdentifier, "1", claimList);
            AddClaim(ClaimsUser.FirstNameClaimType, "Test", claimList);
            AddClaim(ClaimsUser.LastNameClaimType, "TestL", claimList);
            AddClaim(ClaimsUser.OrganisationNameClaimType, providerName, claimList);
            AddClaim(ClaimsUser.RoleClaimType, roleName, claimList);
            AddClaim(ClaimsUser.UkprnClaimType, ukprn, claimList);
            AddClaim(ClaimsUser.UserTypeClaimType, isExternal ? "1" : "0", claimList);
            AddClaim(ClaimsUser.EmailClaimType, "abc@test.com", claimList);
            return new ClaimsPrincipal(new ClaimsIdentity(claimList));
        }

        private void AddClaim(string claimType, string value, List<Claim> claimList)
        {
            if (string.IsNullOrEmpty(value)) return;
            claimList.Add(new Claim(claimType, value));
        }
    }
}
