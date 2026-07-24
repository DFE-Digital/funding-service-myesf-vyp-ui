using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UkrlpProviderService;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Interfaces.Services;
using ViewYourPayments.Core.Interfaces.User;
using ViewYourPayments.Core.Models.User;

namespace ViewYourPayments.Core.Services
{
    public class UkRlpProviderSearchService : IProviderSearchService
    {
        #region Core Properties
        private readonly string _serviceUrl;
        private readonly string _schemaVersion;
        private readonly string _stakeHolderId;
        private readonly IApplicationLogger _logger;
        #endregion


        #region Constructors


        /// <summary>
        /// The full constructor with UK RLP endpoint.
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="schemaVersion"></param>
        /// <param name="stakeHolderId"></param>
        public UkRlpProviderSearchService(string serviceUrl, string schemaVersion, string stakeHolderId, IApplicationLogger logger)
        {
            _schemaVersion = schemaVersion;
            _serviceUrl = serviceUrl;
            _stakeHolderId = stakeHolderId;
            _logger = logger;
        }

        #endregion


        #region Core Methods

        /// <summary>
        /// Get the trading name for a given UKPRN.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the provider.</param>
        /// <returns>The list of matching organisations.</returns>
        public async Task<IEnumerable<IProvider>> GetProvidersByUkprn(int ukprn)
        {
            if (_logger != null)
            {
                _logger.LogWarn($"Calling UKRLP for GetTradingNameByUkprn for [{ukprn}]");
            }

            return await CallWithSelectionCriteria(criteria => criteria.UnitedKingdomProviderReferenceNumberList = new[] { ukprn.ToString() });
        }

        
        /// <summary>
        /// Get the trading name for a given active UKPRN.
        /// </summary>
        /// <param name="ukprn">The UKPRN of the active provider.</param>
        /// <returns>The list of matching active organisations.</returns>
        public async Task<IEnumerable<IProvider>> GetActiveProvidersByUkprn(int ukprn)
        {
            if (_logger != null)
            {
                _logger.LogWarn( $"Calling UKRLP for GetTradingNameByUkprn for [{ukprn}]");
            }

            return await CallWithSelectionCriteriaForActiveState(criteria => criteria.UnitedKingdomProviderReferenceNumberList = new[] { ukprn.ToString() });
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Call the UKRLP provider search service with the given selection criteria, for all states.
        /// </summary>
        /// <param name="selectionCriteria">The selection criteria.</param>
        /// <returns>The collection of matching providers.</returns>
        private async Task<IEnumerable<IProvider>> CallWithSelectionCriteria(Action<SelectionCriteriaStructure> selectionCriteria)
        {
            var states = new[] { "A", "PD1", "PD2" };
            var providers = new List<IProvider>();

            var tasks = new List<Task<IEnumerable<IProvider>>>();

            foreach (var state in states)
            {
                tasks.Add(CallWithSelectionCriteria(selectionCriteria, state));
            }

            foreach (var task in tasks)
            {
                providers.AddRange(await task);
            }

            return providers.OrderBy(o => o.Name);
        }

        /// <summary>
        /// Call the UKRLP provider search service with the given selection criteria, for the active state only.
        /// </summary>
        /// <param name="selectionCriteria">The selection criteria.</param>
        /// <returns>The collection of matching providers.</returns>
        private async Task<IEnumerable<IProvider>> CallWithSelectionCriteriaForActiveState(Action<SelectionCriteriaStructure> selectionCriteria)
        {
            var states = new[] { "A" };
            var providers = new List<IProvider>();

            var tasks = new List<Task<IEnumerable<IProvider>>>();

            foreach (var state in states)
            {
                tasks.Add(CallWithSelectionCriteria(selectionCriteria, state));
            }

            foreach (var task in tasks)
            {
                providers.AddRange(await task);
            }

            return providers.OrderBy(o => o.Name);
        }

        /// <summary>
        /// Call the UKRLP provider search service with the given selection criteria, for the given state.
        /// </summary>
        /// <param name="selectionCriteria">The selection criteria.</param>
        /// <param name="state">The provider state to add to the selection criteria.</param>
        /// <returns>The collection of matching providers.</returns>
        private async Task<IEnumerable<IProvider>> CallWithSelectionCriteria(Action<SelectionCriteriaStructure> selectionCriteria, string state)
        {
            var request = BuildOrganisationServiceRequest();
            selectionCriteria(request.SelectionCriteria);
            request.SelectionCriteria.ProviderStatus = state;

            return await CallUkRlpService(request);
        }

        /// <summary>
        /// Build the basic search request structure.
        /// </summary>
        /// <returns>The basic search request structure.</returns>   
        private ProviderQueryStructure BuildOrganisationServiceRequest()
        {
            return new ProviderQueryStructure
            {
                QueryId = "0",
                SchemaVersion = _schemaVersion,
                SelectionCriteria = new SelectionCriteriaStructure
                {
                    StakeholderId = _stakeHolderId,
                    ApprovedProvidersOnly = YesNoType.No,
                    ApprovedProvidersOnlySpecified = true,
                    CriteriaConditionSpecified = true,
                    CriteriaCondition = QueryCriteriaConditionType.AND
                }
            };
        }

        /// <summary>
        /// Call the UKRLP service.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <returns>The collection of providers matching the request.</returns>
        private async Task<IEnumerable<IProvider>> CallUkRlpService(ProviderQueryStructure request)
        {
            try
            {
                var client = new ProviderQueryPortTypeClient(ProviderQueryPortTypeClient.EndpointConfiguration.ProviderQueryPort, _serviceUrl);
                var retrieveAllProvidersResponse = await client.retrieveAllProvidersAsync(request);
                    if (retrieveAllProvidersResponse?.ProviderQueryResponse?.MatchingProviderRecords != null)
                    {
                        var listOfProviders = retrieveAllProvidersResponse.ProviderQueryResponse.MatchingProviderRecords;
                        if (listOfProviders.Any())
                        {
                            var providersList = new List<Provider>();

                            foreach (var provider in listOfProviders)
                            {
                                var companyNumber = provider?.VerificationDetails?.Where(o => o.VerificationAuthority == "Companies House")?
                                    .Select(o => o.VerificationID).FirstOrDefault();

                                var charityHouseNumber = provider?.VerificationDetails?.Where(o => o.VerificationAuthority == "Charity Commission")?
                                    .Select(o => o.VerificationID).FirstOrDefault();

                                providersList.Add(new Provider(
                                    Convert.ToInt32(provider.UnitedKingdomProviderReferenceNumber),
                                    provider.ProviderName,
                                    companyNumber,
                                    charityHouseNumber)
                                );
                            }

                            return providersList;
                        }
                    _logger.LogWarn($"No provider found for {request.SelectionCriteria.UnitedKingdomProviderReferenceNumberList} and client {client.Endpoint.Address.Uri.AbsoluteUri}");
                    }
                
            }
            catch (Exception exception)
            {
                if (_logger != null)
                {
                    _logger.LogException( exception);
                }
            }

            return Enumerable.Empty<IProvider>();
        }

        #endregion
    }
}
