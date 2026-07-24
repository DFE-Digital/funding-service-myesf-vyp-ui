using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ViewYourPayments.Core.Attributes;
using ViewYourPayments.Core.Enums.User;

namespace ViewYourPayments.Core.Models.User
{
    [Flags]
    public enum UserRole
    {
        /// <summary>
        /// No contact group from Contact service.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(None)), UserTypeDescriptor(UserType = UserType.Unknown)]
        None = 0,

        // <summary>
        /// User role to view contracts and agreements.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ViewContractsAndAgreements)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View contracts and agreements")]
        ViewContractsAndAgreements = 1,

        /// <summary>
        /// User role to sign contracts and agreements.
        /// </summary>
        ///  [Capability(Capability.SignSfaContractsAndAgreements)]

        [ContactServiceDescriptor(GroupName = nameof(SignContractsAndAgreements)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Sign contracts and agreements")]
        SignContractsAndAgreements = 2,

        /// <summary>
        /// User role to view funding claims and reconciliation statements.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(ViewFundingClaimsAndReconciliationStatements)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View funding claims and reconciliation statements")]
        ViewFundingClaimsAndReconciliationStatements = 3,

        /// <summary>
        /// User role to sign funding claims.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(SignFundingClaims)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Sign funding claims")]
        SignFundingClaims = 4,

        /// <summary>
        /// View As Provider group from Contact service.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(ViewAsProvider)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "View as Provider")]
        ViewAsProvider = 5,

        /// <summary>
        /// Sfs Admin group from Contact service.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(SfsAdmin)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "MYESF Admin")]
        SfsAdmin = 6,

        /// <summary>
        /// User role to view previous subcontractor declarations.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ViewPreviousSubcontractorDeclarations)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View previous subcontractor declarations")]
        ViewPreviousSubcontractorDeclarations = 7,

        /// <summary>
        /// User role to submit subcontractor declarations.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(SubmitSubcontractorDeclarations)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Submit subcontractor declarations")]
        SubmitSubcontractorDeclarations = 8,

        /// <summary>
        /// Apprenticeships admin to go between DAS and SFS.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(ApprenticeshipsEditor)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Apprenticeships editor")]
        ApprenticeshipsEditor = 9,

        /// <summary>
        /// Funding Centre Team member.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DocumentExchangeAdministratorFundingCentre)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Document exchange funding centre user")]
        DocumentExchangeAdministratorFundingCentre = 10,

        /// <summary>
        /// Risk Assurance Team member.
        /// </summary>

        [ContactServiceDescriptor(GroupName = nameof(DocumentExchangeAdministratorRiskAssurance)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Document exchange risk assurance user")]
        DocumentExchangeAdministratorRiskAssurance = 11,

        /// <summary>
        /// External Document Exchange User.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DocumentExchangeUser)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Document Exchange User")]
        DocumentExchangeUser = 12,

        /// <summary>
        /// Can View Payments in View Your Payments.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(ViewPaymentHistory)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View payment history")]
        ViewPaymentHistory = 13,

        /// <summary>
        /// Nff Admin user.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(NffAdmin)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "NFF Admin")]
        NffAdmin = 14,

        /// <summary>
        /// Document Exchange admin user.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DocumentExchangeAdmin)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Document exchange administrator")]
        DocumentExchangeAdmin = 15,

        /// <summary>
        /// Document Exchange advanced user.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DocumentExchangeAdvancedUser)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Document exchange advanced user")]
        DocumentExchangeAdvancedUser = 16,

        /// <summary>
        /// Allocation Statements Viewer.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ViewAllocationStatements)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View allocation statements")]
        ViewAllocationStatements = 17,

        /// <summary>
        /// Apprenticeships contributor to go between DAS and SFS.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ApprenticeshipsContrib)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Apprenticeships contributor")]
        ApprenticeshipsContrib = 18,

        /// <summary>
        /// Apprenticeships super contributor to go between DAS and SFS.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ApprenticeshipsSupContrib)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Apprenticeships super contributor")]
        ApprenticeshipsSupContrib = 19,

        /// <summary>
        /// Apprenticeships viewer to go between DAS and SFS.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ApprenticeshipsEditor)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "Apprenticeships viewer")]
        ApprenticeshipsViewer = 20,

        /// <summary>
        /// Internal viewer to download reports anytime.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DownloadReports)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Download Reports")]
        DownloadReports = 21,

        /// <summary>
        /// 1416 Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_1416)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator 1416")]
        AllocationsAdministrator_1416 = 22,

        /// <summary>
        /// 1619 Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_1619)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator 1619")]
        AllocationsAdministrator_1619 = 23,

        /// <summary>
        /// DSG Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_DSG)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator DSG")]
        AllocationsAdministrator_DSG = 24,

        /// <summary>
        /// GAG Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_GAG)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator GAG")]
        AllocationsAdministrator_GAG = 25,

        /// <summary>
        /// NMSS Allocation Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_NMSS)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator NMSS")]
        AllocationsAdministrator_NMSS = 26,

        /// <summary>
        /// PSG Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_PSG)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator PSG")]
        AllocationsAdministrator_PSG = 27,

        /// <summary>
        /// PPG Allocation Administrator.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(AllocationsAdministrator_PPG)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Allocations Administrator PPG")]
        AllocationsAdministrator_PPG = 28,

        /// <summary>
        /// Internal viewer to download SCD reports anytime.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(DownloadSCDReports)), UserTypeDescriptor(UserType = UserType.Internal)]
        [Display(Name = "Download SCD Reports")]
        DownloadSCDReports = 29,

        /// <summary>
        /// User role to view recoupment reports.
        /// </summary>        
        [ContactServiceDescriptor(GroupName = nameof(ViewRecoupmentReports)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View recoupment reports")]
        ViewRecoupmentReports = 30,

        /// <summary>
        /// User role to view apprenticeship grant for employers reports.
        /// </summary>
        [ContactServiceDescriptor(GroupName = nameof(ViewApprenticeshipGrantForEmployersReports)), UserTypeDescriptor(UserType = UserType.External)]
        [Display(Name = "View apprenticeship grant for employers reports")]
        ViewApprenticeshipGrantForEmployersReports = 31
    }
}