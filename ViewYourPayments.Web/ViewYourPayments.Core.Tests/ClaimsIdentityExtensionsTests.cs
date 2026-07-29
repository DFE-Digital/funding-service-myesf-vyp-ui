using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ViewYourPayments.Core.Extensions;
using ViewYourPayments.Core.Models.DFESignIn;
using ViewYourPayments.Core.Models.User;
using Xunit;

namespace DFESignIn.Tests
{
    /// <summary>
    /// Tests for <see cref="DFESignIn.Domain.Extensions.ClaimsIdentityExtensions"/>.
    /// </summary>
    public class ClaimsIdentityExtensionsTests
    {


        /// <summary>
        /// Adds the organisation ukprn claim with valid ukprn value and returns claim identity.
        /// </summary>
        [Fact]
        public void AddOrganisationUKPRNClaim_WithValidUkprnValue_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakeUkprn = 11111;
            string claimPath = "http://sfs-sfa.gov.uk/claims/organisationId";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddOrganisationUkPrnClaim(fakeUkprn.ToString());

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeUkprn.ToString());
        }

        /// <summary>
        /// Adds the organisation name claim with valid org name value and returns claim identity.
        /// </summary>
        [Fact]
        public void AddOrganisationNameClaim_WithValidOrgNameValue_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakeOrgName = "fake limited";
            string claimPath = "http://sfs-sfa.gov.uk/claims/organisationName";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddOrganisationNameClaim(fakeOrgName);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeOrgName);
        }

        /// <summary>
        /// Adds the name claim with valid user name and returns claim identity.
        /// </summary>
        [Fact]
        public void AddNameClaim_WithValidUserName_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakeUserName = "fake user";
            string claimPath = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddNameClaim(fakeUserName);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeUserName);
        }

        /// <summary>
        /// Adds the first name claim with valid first name and returns claim identity.
        /// </summary>
        [Fact]
        public void AddFirstNameClaim_WithValidFirstName_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakeFirstName = "fake";
            string claimPath = "http://sfs-sfa.gov.uk/claims/firstName";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddFirstNameClaim(fakeFirstName);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeFirstName);
        }

        /// <summary>
        /// Adds the last name claim with valid last name and returns claim identity.
        /// </summary>
        [Fact]
        public void AddLastNameClaim_WithValidLastName_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakeLastName = "user";
            string claimPath = "http://sfs-sfa.gov.uk/claims/lastName";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddLastNameClaim(fakeLastName);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeLastName);
        }

        /// <summary>
        /// Adds the principal claim with valid principal identifier and returns claim identity.
        /// </summary>
        [Fact]
        public void AddPrincipalClaim_WithValidPrincipalId_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            var fakePrincipal = "fake principal";
            string claimPath = "http://sfs-sfa.gov.uk/claims/principal";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddPrincipalClaim(fakePrincipal);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakePrincipal);
        }

        /// <summary>
        /// Adds the roles claim with one role and returns claim identity.
        /// </summary>
        [Fact]
        public void AddRoles_WithOneRole_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            List<Role> fakesRoles = new List<Role>();
            fakesRoles.Add(new Role() { Code = "sfsadmin", Id = Guid.NewGuid(), Name = "sfsadmin" });

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddRoles(fakesRoles);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(ClaimsUser.RoleClaimType).Value.Should().Be("SfsAdmin");
        }

        /// <summary>
        /// Adds the roles claims with multiple roles and returns claim identity.
        /// </summary>
        [Fact]
        public void AddRoles_WithMultipleRoles_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            List<Role> fakesRoles = new List<Role>();
            fakesRoles.Add(new Role() { Code = "sfsadmin", Id = Guid.NewGuid(), Name = "sfsadmin" });
            fakesRoles.Add(new Role() { Code = "ViewContractsAndAgreements", Id = Guid.NewGuid(), Name = "MYESF - View contracts and agreements" });
            string claimPath = ClaimsUser.RoleClaimType;

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddRoles(fakesRoles);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.Claims.Where(claim => claim.Type == claimPath).Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.Claims.Where(claim => claim.Type == claimPath).Should().HaveCount(2);
        }

        /// <summary>
        /// Adds the user type claim with valid dfe user type and returns claim identity.
        /// </summary>
        [Fact]
        public void AddUserTypeClaim_WithInternalUserType_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            string claimPath = "http://sfs-sfa.gov.uk/claims/userType";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddUserTypeClaim(true);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be("0");
        }

        /// <summary>
        /// Adds the user type claim with invalid dfe user type and returns claim identity.
        /// </summary>
        [Fact]
        public void AddUserTypeClaim_WithExternalUserType_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            string claimPath = "http://sfs-sfa.gov.uk/claims/userType";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddUserTypeClaim(false);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be("1");
        }

        /// <summary>
        /// Adds the email claim type with valid email and returns claim identity.
        /// </summary>
        [Fact]
        public void AddEmailClaimType_WithValidEmail_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            string fakeEmail = "fake@fake.com";
            string claimPath = "http://sfs-sfa.gov.uk/claims/email";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddEmailClaim(fakeEmail);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeEmail);
        }

        /// <summary>
        /// Adds the identifier token claim with valid identifier token and returns claim identity.
        /// </summary>
        [Fact]
        public void AddIdTokenClaim_WithValidIdToken_ReturnsClaimIdentity()
        {
            //Arrange
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Enumerable.Empty<Claim>());
            string fakeIdtoken = "faketoken";
            string claimPath = "id_token";

            //Act
            var returnedClaimsIdentity = claimsIdentity.AddIdTokenClaim(fakeIdtoken);

            //Assert
            returnedClaimsIdentity.Should().NotBeNull();
            returnedClaimsIdentity.Claims.Should().NotBeNullOrEmpty();
            returnedClaimsIdentity.FindFirst(claimPath).Value.Should().Be(fakeIdtoken);
        }
    }
}
