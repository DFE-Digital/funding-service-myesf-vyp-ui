# Manage Your Education and Skills Funding View Your Payments

The Manage Your Education and Skills Funding (MYESF) View Your Payments web application to allow the following:

- view your payment history and filter results by a date range (up to 3 years old).
- view individual payment details and export details as CSV or PDF downloads.
- view detailed remittance transactions at budget group summary and detailed levels.
- filter transactions by date range (up to 3 years old) and by transaction type.
- sort results to suit your needs and then export detailed results as CSV downloads.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 web application utilising Azure App Service for deployment.

The web application runs on an Azure App service on Azure.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `appsettings.json` file will need to be created in the `ViewYourPayments.Web` project. Below, and included in the repo, there is `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "Environment": "",
  "AppSettings": {
    "APPLOGGINGAPPINSIGHTS_INSTRUMKEY": "",
    "DfeSettings": {
      "OidcClientId": "",
      "OidcClientSecret": "",
      "OidcAuthority": "",
      "OidcAudience": "",
      "OidcRedirectUrl": "",
      "OidcPostLogOutUrl": "",
      "DfeLegacyCodeId": "",
      "DfeSignInUrl": ""
    },
    "DfeRoleProviderSettings": {
      "OidcClientId": "",
      "OidcClientSecret": "",
      "OidcAudience": "",
      "DfeSignInRolesApiUrl": ""
    },
    "IdamsSettings": {
      "ClaimsIssuer": "",
      "MetadataAddress": "",
      "CallbackPath": "",
      "RequireHttpsMetadata": false,
      "Wtrealm": "",
      "RedirectUrl": ""
    },
    "ProviderSearchSettings": {
      "ServiceEndpoint": "",
      "schemaVersion": "",
      "StakeholderId": ""
    },
    "PaymentsApiHttpClientSettings": {
      "BaseAddress": "",
      "ApimSubscriptionKey": ""
    },
    "OAuthHttpClientSettings": {
      "Resource": "",
      "ClientId": "",
      "GrantType": "",
      "ClientSecret": "",
      "BaseAddress": ""
    },
    "MyEsfUrl": "",
    "MSClarityId": ""
  }
}
```

### Setting Details

- **`Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.

- **`AppSettings:APPLOGGINGAPPINSIGHTS_INSTRUMKEY`** 
  The Microsoft Azure Monitor Application Insights JavaScript SDK collects usage data, which allows you to monitor and analyze the performance of JavaScript web applications. 

- **`AppSettings:DfeSettings:OidcClientId`** 
  The unique public identifier assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`AppSettings:DfeSettings:OidcClientSecret`** 
  The confidential credential assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`AppSettings:DfeSettings:OidcAuthority`** 
  The Authority is the base URL of the OpenID Connect that issues authentication tokens and verifies user identities.

- **`AppSettings:DfeSettings:OidcAudience`** 
  The key that defines the intended recipient of an identity token or access token.

- **`AppSettings:DfeSettings:OidcRedirectUrl`** 
  The Redirect Url is the specific, pre-registered web address where an Identity Provider (IdP) sends the user—and their authentication tokens—after they successfully log in.

- **`AppSettings:DfeSettings:OidcPostLogOutUrl`** 
  The Post-Logout Redirect URI is the specific web address where an Identity Provider (IdP) sends users after they successfully log out of their session.

- **`AppSettings:DfeSettings:DfeLegacyCodeId`** 
  The comma separated list of DfE legacy code ids used to map or track legacy system records.

- **`AppSettings:DfeSettings:DfeSignInUrl`** 
  The base endpoint URL used to communicate with the Department for Education (DfE) Sign-in Public API.

- **`AppSettings:DfeRoleProviderSettings:OidcClientId`** 
  The unique public identifier assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`AppSettings:DfeRoleProviderSettings:OidcClientSecret`** 
  The confidential credential assigned to the DFESignIn application by the OpenID Connect Identity Provider during registration.

- **`AppSettings:DfeRoleProviderSettings:OidcAudience`** 
  The key that defines the intended recipient of an identity token or access token.

- **`AppSettings:DfeRoleProviderSettings:DfeSignInRolesApiUrl`** 
  The configuration setting used by UK educational services integrated with the Department for Education (DfE) Sign-in service to specify the API endpoint for retrieving user roles and permissions.

- **`AppSettings:IdamsSettings:ClaimsIssuer`** 
  The key defines the expected domain or identifier of the security authority issuing identity claims.

- **`AppSettings:IdamsSettings:MetadataAddress`** 
  The key defines the specific URL where an application can download the public configuration file (the metadata document) from the Identity and Access Management System (IDAMS).

- **`AppSettings:IdamsSettings:CallbackPath`** 
  The key defines the relative URL path within your application where the Identity and Access Management System (IDAMS) will post back security tokens after a user completes authentication.

- **`AppSettings:IdamsSettings:RequireHttpsMetadata`** 
  A boolean configuration key that determines whether the application must enforce secure HTTPS connections when downloading the security metadata document from the Identity and Access Management System (IDAMS).

- **`AppSettings:IdamsSettings:Wtrealm`** 
  The key used in applications integrating with the UK Government's Identity and Access Management System (IDAMS) specifies the unique URI identifier (the Realm) of your specific application.

- **`AppSettings:IdamsSettings:RedirectUrl`** 
  The key defines the absolute web address where the Identity and Access Management System (IDAMS) must return users after they have successfully authenticated.

- **`AppSettings:ProviderSearchSettings:BaseAddress`**
  The root API URL for the provider data search service.

- **`AppSettings:ProviderSearchSettings:ApimSubscriptionKey`** 
  The key defines the secret API key required for authenticating outbound HTTP requests to an APIM-fronted external service or provider registry.

- **`AppSettings:OAuthHttpClientSettings:Resource`**
  The key used by an application's HTTP client to specify the target API or secured system it wants to access when requesting an OAuth 2.0 access token.

- **`AppSettings:OAuthHttpClientSettings:ClientId`** 
  The key used to store the public identifier of your application within an automated machine-to-machine communication loop.

- **`AppSettings:OAuthHttpClientSettings:GrantType`** 
  The key used to specify the exact OAuth 2.0 authentication flow (the "grant type") that an application's HTTP client must execute to obtain an access token.

- **`AppSettings:OAuthHttpClientSettings:ClientSecret`**
  A high-security configuration key that stores the secret password used by an application to authenticate its identity during a machine-to-machine OAuth 2.0 handshake.

- **`AppSettings:OAuthHttpClientSettings:BaseAddress`** 
  The key defines the root URL of the secure external API or web service that your OAuth-authenticated HTTP client will communicate with.

- **`AppSettings:MyEsfUrl`**
  The base URL for the Manage Your Education and Skills Funding (MYESF) service.

- **`AppSettings:MSClarityId`**
  The unique tracking identifier assigned by Microsoft Clarity to a specific website project. It tells the Microsoft Clarity tracking script where to send session recordings, heatmaps, and user behavior analytics for that particular website.

## Node packages

`ViewYourPayments.Web` project uses Node packages for front-end development. The required packages and versions are defined in the `package-lock.json` file.

Please ensure the package references found in the `ViewYourrPayments.Web` dependencies section match the package references found in the `package-lock.json` file. If they do not match, uninstall the packages from the npm depencencies within `ViewYourPayments.Web` and restore the packages by selecting `Restore packages` from the `package.json` file context menu.
  
## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
  