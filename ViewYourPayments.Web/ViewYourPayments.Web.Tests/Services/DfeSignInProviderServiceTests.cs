using DFESignIn.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewYourPayments.Core.Interfaces.HttpClient;
using ViewYourPayments.Core.Models.AppConfig;
using ViewYourPayments.Core.Models.DFESignIn;
using Xunit;

namespace DFESignIn.Tests
{
    public class DfeSignInProviderServiceTests
    {

        private Guid roleId = Guid.NewGuid();
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly Mock<IDfeSignInProviderApiHttpClient> _mockDfeSignInProviderHttpClient;

        public DfeSignInProviderServiceTests()
        {
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockDfeSignInProviderHttpClient = new Mock<IDfeSignInProviderApiHttpClient>();
        }

        [Fact]
        public async Task GetRolesAsync_ReturnsRoles_WhenGetRolesOperationIsCalled()
        {
            // Arrange

            var dfeSignInProviderService = new DfeSignInProviderService(_mockAppSettings.Object, _mockDfeSignInProviderHttpClient.Object);
            _mockAppSettings.Setup(x => x.Value).Returns(GetRoleProviderSettings());
            _mockDfeSignInProviderHttpClient.Setup(x => x.Get<DfeClaims>(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(GetClaim()));
            //Act
            var roles = await dfeSignInProviderService.GetRolesAsync(Guid.NewGuid(), Guid.NewGuid());

            //Assert            
            roles.Should().NotBeEmpty();
            roles.First(x => x.Id == roleId);
        }

        [Fact]
        public async Task GetOrganisationAsync_WhenResponseHasMatchingRecords_ShouldReturnCorrectOrganisation()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var dfeSignInProviderService = new DfeSignInProviderService(_mockAppSettings.Object, _mockDfeSignInProviderHttpClient.Object);
            _mockAppSettings.Setup(x => x.Value).Returns(GetRoleProviderSettings());
            _mockDfeSignInProviderHttpClient.Setup(x => x.Get<IEnumerable<Organisation>>(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(GetOrganisations(organisationId));
            //Act
            var organisations = await dfeSignInProviderService.GetOrganisationAsync(Guid.NewGuid(), organisationId);

            //Assert            
            organisations.Should().NotBeNull();
            organisations.Id.Should().Be(organisationId.ToString());
            _mockDfeSignInProviderHttpClient.Verify(x => x.Get<IEnumerable<Organisation>>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetOrganisationAsync_WhenResponseHasNoMatchingRecords_ShouldReturnNull()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var dfeSignInProviderService = new DfeSignInProviderService(_mockAppSettings.Object, _mockDfeSignInProviderHttpClient.Object);
            _mockAppSettings.Setup(x => x.Value).Returns(GetRoleProviderSettings());
            _mockDfeSignInProviderHttpClient.Setup(x => x.Get<IEnumerable<Organisation>>(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(GetOrganisations(organisationId));
            //Act
            var organisations = await dfeSignInProviderService.GetOrganisationAsync(Guid.NewGuid(), Guid.NewGuid());

            //Assert            
            organisations.Should().BeNull();
            _mockDfeSignInProviderHttpClient.Verify(x => x.Get<IEnumerable<Organisation>>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        private DfeClaims GetClaim()
        {
            return new DfeClaims { Roles = new[] { new Role { Id = roleId, Code = "Payment", Name = "PaymentUser" } } };
        }

        private AppSettings GetRoleProviderSettings()
        {
            return new AppSettings
            {
                DfeRoleProviderSettings = new DfeRoleProviderSettings
                {
                    OidcClientId = "Test1",
                    DfeSignInRolesApiUrl = "https://testapi",
                    OidcAudience = "signin",
                    OidcClientSecret = new Guid().ToString()
                }
            };
        }

        private IEnumerable<Organisation> GetOrganisations(Guid organisationId)
        {
            return new[]
            {
                new Organisation { Id=  organisationId.ToString(), Name = "Test1"},
                new Organisation {Id = "Test2", Name = "Test2"}
            };
        }

    }
}