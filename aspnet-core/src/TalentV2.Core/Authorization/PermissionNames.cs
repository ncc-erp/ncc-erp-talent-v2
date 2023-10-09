using Abp.MultiTenancy;
using System.Collections.Generic;
using System.Security;
using TalentV2.Authorization.Roles;
using TalentV2.Constants.Const;

namespace TalentV2.Authorization
{
    public static class PermissionNames
    {
        public const string TabAdmin = "Admin";
        #region Page Tenant
        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Tenants_ViewList = "Pages.Tenants.ViewList";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        #endregion

        #region Page User
        public const string Pages_Users = "Pages.Users";
        public const string Pages_Users_ViewList = "Pages.Users.ViewList";
        public const string Pages_Users_Create = "Pages.Users.Create";
        public const string Pages_Users_Edit = "Pages.Users.Edit";
        public const string Pages_Users_Delete = "Pages.Users.Delete";
        public const string Pages_Users_ResetPassword = "Pages.Users.ResetPassword";
        public const string Pages_Users_Activation = "Pages.Users.Activation";
        #endregion

        #region Page Role
        public const string Pages_Roles = "Pages.Roles";
        public const string Pages_Roles_ViewList = "Pages.Roles.ViewList";
        public const string Pages_Roles_Create = "Pages.Roles.Create";
        public const string Pages_Roles_Edit = "Pages.Roles.Edit";
        public const string Pages_Roles_Delete = "Pages.Roles.Delete";
        #endregion

        #region Page Mail
        public const string Pages_Mails = "Pages.Mails";
        public const string Pages_Mails_ViewList = "Pages.Mails.ViewList";
        public const string Pages_Mails_Preview = "Pages.Mails.Preview";
        public const string Pages_Mails_Edit = "Pages.Mails.Edit";
        public const string Pages_Mails_SendMail = "Pages.Mails.SendMail";
        #endregion

        #region Page Configuration
        public const string Pages_Configurations = "Pages.Configurations";
        public const string Pages_Configurations_ViewEmailSettings = "Pages.Configurations.ViewEmailSettings";
        public const string Pages_Configurations_EditEmailSettings = "Pages.Configurations.EditEmailSettings";
        public const string Pages_Configurations_ViewKomuSettings = "Pages.Configurations.ViewKomuSettings";
        public const string Pages_Configurations_ViewChannelHRITSettings = "Pages.Configurations.ViewChannelHRITSettings";
        public const string Pages_Configurations_EditChannelHRITSettings = "Pages.Configurations.EditChannelHRITSettings";
        public const string Pages_Configurations_ViewHRMSettings = "Pages.Configurations.ViewHRMSettings";
        public const string Pages_Configurations_ViewLMSSettings = "Pages.Configurations.ViewLMSSettings";
        public const string Pages_Configurations_ViewGoogleClientAppSettings = "Pages.Configurations.ViewGoogleClientAppSettings";
        public const string Pages_Configurations_EditGoogleClientAppSettings = "Pages.Configurations.EditGoogleClientAppSettings";
        public const string Pages_Configurations_ViewTalentSecretCode = "Pages.Configurations.ViewTalentSecretCode";
        public const string Pages_Configurations_EditTalentSecretCode = "Pages.Configurations.EditTalentSecretCode";
        public const string Pages_Configurations_ViewTalentNotifyInterviewSettings = "Pages.Configurations.ViewTalentNotifyInterviewSettings";
        public const string Pages_Configurations_EditTalentNotifyInterviewSettings = "Pages.Configurations.EditTalentNotifyInterviewSettings";
        #endregion

        public const string TabCategory = "Category";
        #region Page Sub Position
        public const string Pages_SubPositions = "Pages.SubPositions";
        public const string Pages_SubPositions_ViewList = "Pages.SubPositions.ViewList";
        public const string Pages_SubPositions_Create = "Pages.SubPositions.Create";
        public const string Pages_SubPositions_Edit = "Pages.SubPositions.Edit";
        public const string Pages_SubPositions_Delete = "Pages.SubPositions.Delete";
        #endregion

        #region Page Position Settings
        public const string Pages_PositionSettings = "Pages.PositionSettings";
        public const string Pages_PositionSettings_ViewList = "Pages.PositionSettings.ViewList";
        public const string Pages_PositionSettings_Create = "Pages.PositionSettings.Create";
        public const string Pages_PositionSettings_Edit = "Pages.PositionSettings.Edit";
        public const string Pages_PositionSettings_Delete = "Pages.PositionSettings.Delete";
        #endregion

        #region Page Education
        public const string Pages_Educations = "Pages.Educations";
        public const string Pages_Educations_ViewList = "Pages.Educations.ViewList";
        public const string Pages_Educations_Create = "Pages.Educations.Create";
        public const string Pages_Educations_Edit = "Pages.Educations.Edit";
        public const string Pages_Educations_Delete = "Pages.Educations.Delete";
        #endregion

        #region Page Education Type
        public const string Pages_EducationTypes = "Pages.Education_Types";
        public const string Pages_EducationTypes_ViewList = "Pages.EducationTypes.ViewList";
        public const string Pages_EducationTypes_Create = "Pages.Education_Types.Create";
        public const string Pages_EducationTypes_Edit = "Pages.Education_Types.Edit";
        public const string Pages_EducationTypes_Delete = "Pages.Education_Types.Delete";
        #endregion

        #region Page Skill
        public const string Pages_Skills = "Pages.Skills";
        public const string Pages_Skills_ViewList = "Pages.Skills.ViewList";
        public const string Pages_Skills_Create = "Pages.Skills.Create";
        public const string Pages_Skills_Edit = "Pages.Skills.Edit";
        public const string Pages_Skills_Delete = "Pages.Skills.Delete";
        #endregion

        #region Page CV Source
        public const string Pages_CVSources = "Pages.CVSources";
        public const string Pages_CVSources_ViewList = "Pages.CVSources.ViewList";
        public const string Pages_CVSources_Create = "Pages.CVSources.Create";
        public const string Pages_CVSources_Edit = "Pages.CVSources.Edit";
        public const string Pages_CVSources_Delete = "Pages.CVSources.Delete";
        #endregion

        #region Page JobPosition
        public const string Pages_JobPositions = "Pages.JobPositions";
        public const string Pages_JobPositions_ViewList = "Pages.JobPositions.ViewList";
        public const string Pages_JobPositions_Create = "Pages.JobPositions.Create";
        public const string Pages_JobPositions_Edit = "Pages.JobPositions.Edit";
        public const string Pages_JobPositions_Delete = "Pages.JobPositions.Delete";
        #endregion

        #region Page Branch
        public const string Pages_Branches = "Pages.Branches";
        public const string Pages_Branches_ViewList = "Pages.Branches.View_List";
        public const string Pages_Branches_Create = "Pages.Branches.Create";
        public const string Pages_Branches_Edit = "Pages.Branches.Edit";
        public const string Pages_Branches_Delete = "Pages.Branches.Delete";
        #endregion

        #region Page ExternalCV
        public const string Pages_ExternalCVs = "Pages.ExternalCVs";
        public const string Pages_ExternalCVs_ViewList = "Pages.ExternalCVs.View_List";
        public const string Pages_ExternalCVs_Create = "Pages.ExternalCVs.Create";
        public const string Pages_ExternalCVs_Edit = "Pages.ExternalCVs.Edit";
        public const string Pages_ExternalCVs_Delete = "Pages.ExternalCVs.Delete";
        public const string Pages_ExternalCVs_ViewDetail = "Pages.ExternalCVs.ViewDetail";
        #endregion

        #region Page Post
        public const string Pages_Posts = "Pages.Posts";
        public const string Pages_Posts_ViewList = "Pages.Posts.View_List";
        public const string Pages_Posts_Create = "Pages.Posts.Create";
        public const string Pages_Posts_Edit = "Pages.Posts.Edit";
        public const string Pages_Posts_Delete = "Pages.Posts.Delete";
        #endregion

        #region Page Capability
        public const string Pages_Capabilities = "Pages.Capabilities";
        public const string Pages_Capabilities_ViewList = "Pages.Capabilities.ViewList";
        public const string Pages_Capabilities_Create = "Pages.Capabilities.Create";
        public const string Pages_Capabilities_Edit = "Pages.Capabilities.Edit";
        public const string Pages_Capabilities_Delete = "Pages.Capabilities.Delete";
        #endregion

        #region Page Capability Setting
        public const string Pages_CapabilitySettings = "Pages.CapabilitySettings";
        public const string Pages_CapabilitySettings_ViewList = "Pages.CapabilitySettings.ViewList";
        public const string Pages_CapabilitySettings_Create = "Pages.CapabilitySettings.Create";
        public const string Pages_CapabilitySettings_Edit = "Pages.CapabilitySettings.Edit";
        public const string Pages_CapabilitySettings_Clone = "Pages.CapabilitySettings.Clone";
        public const string Pages_CapabilitySettings_Delete = "Pages.CapabilitySettings.Delete";
        public const string Pages_CapabilitySettings_EditFactor = "Pages.CapabilitySettings.EditFactor";
        #endregion

        #region Page Score Setting
        public const string Pages_ScoreSettings = "Pages.ScoreSettings";
        public const string Pages_ScoreSettings_ViewList = "Pages.ScoreSettings.ViewList";
        public const string Pages_ScoreSettings_Create = "Pages.ScoreSettings.Create";
        public const string Pages_ScoreSettings_Edit = "Pages.ScoreSettings.Edit";
        public const string Pages_ScoreSettings_Delete = "Pages.ScoreSettings.Delete";
        #endregion

        public const string TabCandidate = "Candidate";
        #region Page Candidate Staff
        public const string Pages_CandidateStaff = "Pages.CandidateStaff";
        public const string Pages_CandidateStaff_ViewList = "Pages.CandidateStaff.View_List";
        public const string Pages_CandidateStaff_Create = "Pages.CandidateStaff.Create";
        public const string Pages_CandidateStaff_Export = "Pages.CandidateStaff.Export";
        public const string Pages_CandidateStaff_Clone = "Pages.CandidateStaff.Clone";
        public const string Pages_CandidateStaff_Delete = "Pages.CandidateStaff.Delete";
        public const string Pages_CandidateStaff_EditNote = "Pages.CandidateStaff.EditNote";
        public const string Pages_CandidateStaff_ViewDetail = "Pages.CandidateStaff.ViewDetail";

        public const string Pages_CandidateStaff_ViewDetail_PersonInfo = "Pages.CandidateStaff.ViewDetail.PersonInfo";
        public const string Pages_CandidateStaff_ViewDetail_PersonInfo_View = "Pages.CandidateStaff.ViewDetail.PersonInfo.View";
        public const string Pages_CandidateStaff_ViewDetail_PersonInfo_Edit = "Pages.CandidateStaff.ViewDetail.PersonInfo.Edit";
        public const string Pages_CandidateStaff_ViewDetail_PersonInfo_SendMail = "Pages.CandidateStaff.ViewDetail.PersonInfo.SendMail";

        public const string Pages_CandidateStaff_ViewDetail_Education = "Pages.CandidateStaff.ViewDetail.Education";
        public const string Pages_CandidateStaff_ViewDetail_Education_ViewList = "Pages.CandidateStaff.ViewDetail.Education.ViewList";
        public const string Pages_CandidateStaff_ViewDetail_Education_Create = "Pages.CandidateStaff.ViewDetail.Education.Create";
        public const string Pages_CandidateStaff_ViewDetail_Education_Delete = "Pages.CandidateStaff.ViewDetail.Education.Delete";

        public const string Pages_CandidateStaff_ViewDetail_Skill = "Pages.CandidateStaff.ViewDetail.Skill";
        public const string Pages_CandidateStaff_ViewDetail_Skill_ViewList = "Pages.CandidateStaff.ViewDetail.Skill.ViewList";
        public const string Pages_CandidateStaff_ViewDetail_Skill_Create = "Pages.CandidateStaff.ViewDetail.Skill.Create";
        public const string Pages_CandidateStaff_ViewDetail_Skill_Delete = "Pages.CandidateStaff.ViewDetail.Skill.Delete";
        public const string Pages_CandidateStaff_ViewDetail_Skill_Edit = "Pages.CandidateStaff.ViewDetail.Skill.Edit";

        public const string Pages_CandidateStaff_ViewDetail_RequestCV = "Pages.CandidateStaff.ViewDetail.RequestCV";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_View = "Pages.CandidateStaff.ViewDetail.RequestCV.View";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_Create = "Pages.CandidateStaff.ViewDetail.RequestCV.Create";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_Delete = "Pages.CandidateStaff.ViewDetail.RequestCV.Delete";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview = "Pages.CandidateStaff.ViewDetail.RequestCV.ViewEditInterviewer";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability = "Pages.CandidateStaff.ViewDetail.RequestCV.ViewCapability";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability = "Pages.CandidateStaff.ViewDetail.RequestCV.EditCapability";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult = "Pages.CandidateStaff.ViewDetail.RequestCV.ViewApplicationResult";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult = "Pages.CandidateStaff.ViewDetail.RequestCV.EditApplicationResult";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_CreateAccountLMS = "Pages.CandidateStaff.ViewDetail.RequestCV.CreateAccountLMS";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_SendMail = "Pages.CandidateStaff.ViewDetail.RequestCV.SendMail";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_ViewSalary = "Pages.CandidateStaff.ViewDetail.RequestCV.ViewSalary";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary = "Pages.CandidateStaff.ViewDetail.RequestCV.EditSalary";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel = "Pages.CandidateStaff.ViewDetail.RequestCV.EditInterviewLevel";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus = "Pages.CandidateStaff.ViewDetail.RequestCV.EditApplicationStatus";
        public const string Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult = "Pages.CandidateStaff.ViewDetail.RequestCV.EditFactorCapabilityResult";

        public const string Pages_CandidateStaff_ViewDetail_ApplicationHistory = "Pages.CandidateStaff.ViewDetail.ApplicationHistory";
        #endregion

        #region Page ApplyCV
        public const string Pages_ApplyCV = "Pages.ApplyCV";
        public const string Pages_ApplyCV_ViewList = "Pages.ApplyCV.View_List";
        public const string Pages_ApplyCV_Create = "Pages.ApplyCV.Create";
        #endregion

        #region Page Candidate Intern
        public const string Pages_CandidateIntern = "Pages.CandidateIntern";
        public const string Pages_CandidateIntern_ViewList = "Pages.CandidateIntern.View_List";
        public const string Pages_CandidateIntern_Create = "Pages.CandidateIntern.Create";
        public const string Pages_CandidateIntern_Export = "Pages.CandidateIntern.Export";
        public const string Pages_CandidateIntern_Clone = "Pages.CandidateIntern.Clone";
        public const string Pages_CandidateIntern_Delete = "Pages.CandidateIntern.Delete";
        public const string Pages_CandidateIntern_EditNote = "Pages.CandidateIntern.EditNote";
        public const string Pages_CandidateIntern_ViewDetail = "Pages.CandidateIntern.ViewDetail";

        public const string Pages_CandidateIntern_ViewDetail_PersonInfo = "Pages.CandidateIntern.ViewDetail.PersonInfo";
        public const string Pages_CandidateIntern_ViewDetail_PersonInfo_View = "Pages.CandidateIntern.ViewDetail.PersonInfo.View";
        public const string Pages_CandidateIntern_ViewDetail_PersonInfo_Edit = "Pages.CandidateIntern.ViewDetail.PersonInfo.Edit";
        public const string Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail = "Pages.CandidateIntern.ViewDetail.PersonInfo.SendMail";

        public const string Pages_CandidateIntern_ViewDetail_Education = "Pages.CandidateIntern.ViewDetail.Education";
        public const string Pages_CandidateIntern_ViewDetail_Education_ViewList = "Pages.CandidateIntern.ViewDetail.Education.ViewList";
        public const string Pages_CandidateIntern_ViewDetail_Education_Create = "Pages.CandidateIntern.ViewDetail.Education.Create";
        public const string Pages_CandidateIntern_ViewDetail_Education_Delete = "Pages.CandidateIntern.ViewDetail.Education.Delete";

        public const string Pages_CandidateIntern_ViewDetail_Skill = "Pages.CandidateIntern.ViewDetail.Skill";
        public const string Pages_CandidateIntern_ViewDetail_Skill_ViewList = "Pages.CandidateIntern.ViewDetail.Skill.ViewList";
        public const string Pages_CandidateIntern_ViewDetail_Skill_Create = "Pages.CandidateIntern.ViewDetail.Skill.Create";
        public const string Pages_CandidateIntern_ViewDetail_Skill_Delete = "Pages.CandidateIntern.ViewDetail.Skill.Delete";
        public const string Pages_CandidateIntern_ViewDetail_Skill_Edit = "Pages.CandidateIntern.ViewDetail.Skill.Edit";

        public const string Pages_CandidateIntern_ViewDetail_RequestCV = "Pages.CandidateIntern.ViewDetail.RequestCV";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_View = "Pages.CandidateIntern.ViewDetail.RequestCV.View";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_Create = "Pages.CandidateIntern.ViewDetail.RequestCV.Create";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_Delete = "Pages.CandidateIntern.ViewDetail.RequestCV.Delete";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview = "Pages.CandidateIntern.ViewDetail.RequestCV.ViewEditInterviewer";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability = "Pages.CandidateIntern.ViewDetail.RequestCV.ViewCapability";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability = "Pages.CandidateIntern.ViewDetail.RequestCV.EditCapability";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult = "Pages.CandidateIntern.ViewDetail.RequestCV.ViewApplicationResult";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult = "Pages.CandidateIntern.ViewDetail.RequestCV.EditApplicationResult";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS = "Pages.CandidateIntern.ViewDetail.RequestCV.CreateAccountLMS";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_SendMail = "Pages.CandidateIntern.ViewDetail.RequestCV.SendMail";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_ViewSalary = "Pages.CandidateIntern.ViewDetail.RequestCV.ViewSalary";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary = "Pages.CandidateIntern.ViewDetail.RequestCV.EditSalary";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel = "Pages.CandidateIntern.ViewDetail.RequestCV.EditInterviewLevel";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus = "Pages.CandidateIntern.ViewDetail.RequestCV.EditApplicationStatus";
        public const string Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult = "Pages.CandidateIntern.ViewDetail.RequestCV.EditFactorCapabilityResult";

        public const string Pages_CandidateIntern_ViewDetail_ApplicationHistory = "Pages.CandidateIntern.ViewDetail.ApplicationHistory";
        #endregion

        public const string TabRequisition = "Requisition";
        #region Page Requisition Staff
        public const string Pages_RequisitionStaff = "Pages.RequisitionStaff";
        public const string Pages_RequisitionStaff_ViewList = "Pages.RequisitionStaff.ViewList";
        public const string Pages_RequisitionStaff_Create = "Pages.RequisitionStaff.Create";
        public const string Pages_RequisitionStaff_AddCV = "Pages.RequistionStaff.AddCV";
        public const string Pages_RequisitionStaff_Edit = "Pages.RequisitionStaff.Edit";
        public const string Pages_RequisitionStaff_Delete = "Pages.RequisitionStaff.Delete";
        public const string Pages_RequisitionStaff_ViewDetail = "Pages.RequisitionStaff.ViewDetail";
        public const string Pages_RequisitionStaff_Close = "Pages.RequisitionStaff.Close";
        public const string Pages_RequisitionStaff_Clone = "Pages.RequisitionStaff.Clone";
        public const string Pages_RequisitionStaff_ReOpen = "Pages.RequisitionStaff.ReOpen";
        public const string Pages_RequisitionStaff_DeleteRequestCV = "Pages.RequisitionStaff.DeleteRequestCV";
        #endregion

        #region Page Requisition Intern
        public const string Pages_RequisitionIntern = "Pages.RequsitionIntern";
        public const string Pages_RequisitionIntern_ViewList = "Pages.RequsitionIntern.ViewList";
        public const string Pages_RequisitionIntern_Create = "Pages.RequisitionIntern.Create";
        public const string Pages_RequisitionIntern_Edit = "Pages.RequisitionIntern.Edit";
        public const string Pages_RequisitionIntern_Delete = "Pages.RequisitionIntern.Delete";
        public const string Pages_RequisitionIntern_ViewDetail = "Pages.RequisitionIntern.ViewDetail";
        public const string Pages_RequisitionIntern_AddCV = "Pages.RequisitionIntern.AddCV";
        public const string Pages_RequisitionIntern_Close = "Pages.RequisitionIntern.Close";
        public const string Pages_RequisitionIntern_Clone = "Pages.RequisitionIntern.Clone";
        public const string Pages_RequisitionIntern_ReOpen = "Pages.RequisitionIntern.ReOpen";
        public const string Pages_RequisitionIntern_DeleteRequestCV = "Pages.RequisitionIntern.DeleteRequestCV";
        public const string Pages_RequisitionIntern_CloseAndClone = "Pages.RequisitionIntern.CloseAndClose";
        public const string Pages_RequisitionIntern_CloseAndCloneAll = "Pages.RequisitionIntern.CloseAndCloseAll";
        #endregion

        public const string TabInterview = "Interview";
        #region Page Interview
        public const string Pages_Interviews_ViewList = "Pages.Interviews.ViewList";
        public const string Pages_Interviews_ViewOnlyMe = "Pages.Interviews.ViewOnlyMe";
        #endregion

        public const string TabOffers = "Offers";
        #region Page Offer
        public const string Pages_Offers_ViewList = "Pages.Offers.ViewList";
        public const string Pages_Offers_Edit = "Pages.Offers.Edit";
        public const string Pages_Offers_EditSalary = "Pages.Offers.EditSalary";
        public const string Pages_Offers_ViewSalary = "Pages.Offers.ViewSalary";
        #endregion

        public const string TabOnboard = "Onboard";
        #region Page Onboard
        public const string Pages_Onboards_ViewList = "Pages.Onboards.ViewList";
        public const string Pages_Onboards_Edit = "Pages.Onboards.Edit";
        #endregion

        public const string TabReport = "Reports";
        #region Report
        public const string Pages_Reports_Overview = "Pages.Reports.Overview";
        public const string Pages_Reports_Overview_Export = "Pages.Reports.Overview.Export";
        public const string Pages_Reports_Staff_Performance = "Pages.Report.Staff.Performance";
        public const string Pages_Reports_Intern_Performance = "Pages.Report.Intern.Performance";
        public const string Pages_Reports_Intern_Education = "Pages.Report.Intern.Education";
        public const string Pages_Reports_Intern_Education_Export = "Pages.Report.Intern.Education.Export";
        #endregion

        #region NCC Fake CV
        public const string TabFakeCV = "Pages Fake CV Employee";
        public const string MyProfile = "MyProfile";
        public const string MyProfile_Education = "MyProfile.Education";
        public const string MyProfile_Education_Create = "MyProfile.Education.Create";
        public const string MyProfile_Education_Edit = "MyProfile.Education.Edit";
        public const string MyProfile_Education_Delete = "MyProfile.Education.Delete";
        public const string MyProfile_Education_View = "MyProfile.Education.View";
        public const string MyProfile_TechnicalExpertise = "MyProfile.TechnicalExpertise";
        public const string MyProfile_TechnicalExpertise_View = "MyProfile.TechnicalExpertise.View";
        public const string MyProfile_TechnicalExpertise_Create = "MyProfile.TechnicalExpertise.Create";
        public const string MyProfile_TechnicalExpertise_Edit = "MyProfile.TechnicalExpertise.Edit";
        public const string MyProfile_PersonalAttribute = "MyProfile.PersonalAttribute";
        public const string MyProfile_PersonalAttribute_Create = "MyProfile.PersonalAttribute.Create";
        public const string MyProfile_PersonalAttribute_Edit = "MyProfile.PersonalAttribute.Edit";
        public const string MyProfile_PersonalAttribute_View = "MyProfile.PersonalAttribute.View";
        public const string MyProfile_PersonalAttribute_Delete = "MyProfile.PersonalAttribute.Delete";
        public const string MyProfile_WorkingExperience = "MyProfile.WorkingExperience";
        public const string MyProfile_WorkingExperience_Create = "MyProfile.WorkingExperience.Create";
        public const string MyProfile_WorkingExperience_Edit = "MyProfile.WorkingExperience.Edit";
        public const string MyProfile_WorkingExperience_Delete = "MyProfile.WorkingExperience.Delete";
        public const string MyProfile_WorkingExperience_View = "MyProfile.WorkingExperience.View";
        public const string MyProfile_Version = "MyProfile.Version";
        public const string MyProfile_Version_Create = "MyProfile.Version.Create";
        public const string MyProfile_Version_Delete = "MyProfile.Version.Delete";
        public const string MyProfile_Certification = "MyProfile.Certification";
        public const string MyProfile_Certification_Create = "MyProfile.Certification.Create";
        public const string MyProfile_Certification_Edit = "MyProfile.Certification.Edit";
        public const string MyProfile_Certification_View = "MyProfile.Certification.View";
        public const string MyProfile_Certification_ExportCV = "MyProfile.Certification.ExportCV";
        public const string MyProfile_User = "MyProfile.User";
        public const string MyProfile_User_View = "MyProfile.User.View";
        public const string MyProfile_User_Edit = "MyProfile.User.Edit";
        // Permission for Employee
        public const string Employee = "Employee";
        public const string Employee_ViewList = "Employee.ViewList";
        public const string Employee_ViewDetail = "Employee.ViewDetail";
        public const string Employee_EditAsPM = "Employee.EditAsPM";
        // Permission for Project
        public const string Project = "Project";
        public const string Project_Create = "Project.Create";
        public const string Project_Edit = "Project.Edit";
        public const string Project_ViewList = "Project.ViewList";
        // Permisssion for WorkingExperience
        public const string WorkingExperience = "WorkingExperience";
        public const string WorkingExperience_ViewAll = "WorkingExperience.ViewAll";
        //Permission for GroupSkill
        public const string GroupSkill = "GroupSkill";
        public const string GroupSkill_View = "GroupSkill.View";
        public const string GroupSkill_Create = "GroupSkill.Create";
        public const string GroupSkill_Edit = "GroupSkill.Edit";
        public const string GroupSkill_Delete = "GroupSkill.Delete";
        //Permission for Skill
        public const string FakeSkill = "FakeSkill";
        public const string FakeSkill_View = "FakeSkill.View";
        public const string FakeSkill_Create = "FakeSkill.Create";
        public const string FakeSkill_Edit = "FakeSkill.Edit";
        public const string FakeSkill_Delete = "FakeSkill.Delete";
        //Permission for EmployeePosition
        public const string EmployeePosition = "EmployeePosition";
        public const string EmployeePosition_View = "EmployeePosition.View";
        public const string EmployeePosition_Create = "EmployeePosition.Create";
        public const string EmployeePosition_Edit = "EmployeePosition.Edit";
        public const string EmployeePosition_Delete = "EmployeePosition.Delete";
        #endregion

    }
    public class GrantPermissionRoles
    {
        public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>()
        {
            {
                RoleNames.Host_Admin,
                new List<string>()
                {
                    #region Tab Admin
                    PermissionNames.TabAdmin,
                    //Roles
                    PermissionNames.Pages_Roles,
                    PermissionNames.Pages_Roles_ViewList,
                    PermissionNames.Pages_Roles_Create,
                    PermissionNames.Pages_Roles_Edit,
                    PermissionNames.Pages_Roles_Delete,
                    //Tenants
                    PermissionNames.Pages_Tenants,
                    PermissionNames.Pages_Tenants_ViewList,
                    PermissionNames.Pages_Tenants_Create,
                    PermissionNames.Pages_Tenants_Edit,
                    PermissionNames.Pages_Tenants_Delete,
                    //Users
                    PermissionNames.Pages_Users,
                    PermissionNames.Pages_Users_ViewList,
                    PermissionNames.Pages_Users_Create,
                    PermissionNames.Pages_Users_Edit,
                    PermissionNames.Pages_Users_Activation,
                    PermissionNames.Pages_Users_Delete,
                    PermissionNames.Pages_Users_ResetPassword,
                    //Configurations
                    PermissionNames.Pages_Configurations,
                    PermissionNames.Pages_Configurations_EditChannelHRITSettings,
                    PermissionNames.Pages_Configurations_EditEmailSettings,
                    PermissionNames.Pages_Configurations_ViewChannelHRITSettings,
                    PermissionNames.Pages_Configurations_ViewEmailSettings,
                    PermissionNames.Pages_Configurations_ViewHRMSettings,
                    PermissionNames.Pages_Configurations_ViewKomuSettings,
                    PermissionNames.Pages_Configurations_ViewLMSSettings,
                    PermissionNames.Pages_Configurations_ViewGoogleClientAppSettings,
                    PermissionNames.Pages_Configurations_EditGoogleClientAppSettings,
                    PermissionNames.Pages_Configurations_ViewTalentSecretCode,
                    PermissionNames.Pages_Configurations_EditTalentSecretCode,
                    PermissionNames.Pages_Configurations_ViewTalentNotifyInterviewSettings,
                    PermissionNames.Pages_Configurations_EditTalentNotifyInterviewSettings,
                    //Mails
                    PermissionNames.Pages_Mails,
                    PermissionNames.Pages_Mails_ViewList,
                    PermissionNames.Pages_Mails_Edit,
                    PermissionNames.Pages_Mails_Preview,
                    PermissionNames.Pages_Mails_SendMail,
                    #endregion

                    #region Tab Category
                    PermissionNames.TabCategory,
                    //Education Types
                    PermissionNames.Pages_EducationTypes,
                    PermissionNames.Pages_EducationTypes_ViewList,
                    PermissionNames.Pages_EducationTypes_Create,
                    PermissionNames.Pages_EducationTypes_Edit,
                    PermissionNames.Pages_EducationTypes_Delete,
                    //Educations
                    PermissionNames.Pages_Educations,
                    PermissionNames.Pages_Educations_ViewList,
                    PermissionNames.Pages_Educations_Create,
                    PermissionNames.Pages_Educations_Edit,
                    PermissionNames.Pages_Educations_Delete,
                    //Skills
                    PermissionNames.Pages_Skills,
                    PermissionNames.Pages_Skills_ViewList,
                    PermissionNames.Pages_Skills_Create,
                    PermissionNames.Pages_Skills_Edit,
                    PermissionNames.Pages_Skills_Delete,
                    //CVSources
                    PermissionNames.Pages_CVSources,
                    PermissionNames.Pages_CVSources_ViewList,
                    PermissionNames.Pages_CVSources_Create,
                    PermissionNames.Pages_CVSources_Edit,
                    PermissionNames.Pages_CVSources_Delete,
                    //Job Positions
                    PermissionNames.Pages_JobPositions,
                    PermissionNames.Pages_JobPositions_ViewList,
                    PermissionNames.Pages_JobPositions_Create,
                    PermissionNames.Pages_JobPositions_Edit,
                    PermissionNames.Pages_JobPositions_Delete,
                    //Sub Positions
                    PermissionNames.Pages_SubPositions,
                    PermissionNames.Pages_SubPositions_ViewList,
                    PermissionNames.Pages_SubPositions_Create,
                    PermissionNames.Pages_SubPositions_Edit,
                    PermissionNames.Pages_SubPositions_Delete,
                    //Branches
                    PermissionNames.Pages_Branches,
                    PermissionNames.Pages_Branches_ViewList,
                    PermissionNames.Pages_Branches_Create,
                    PermissionNames.Pages_Branches_Edit,
                    PermissionNames.Pages_Branches_Delete,
                    //ExternalCVs
                    PermissionNames.Pages_ExternalCVs,
                    PermissionNames.Pages_ExternalCVs_ViewList,
                    PermissionNames.Pages_ExternalCVs_Create,
                    PermissionNames.Pages_ExternalCVs_Edit,
                    PermissionNames.Pages_ExternalCVs_Delete,
                    PermissionNames.Pages_ExternalCVs_ViewDetail,
                    //Posts
                    PermissionNames.Pages_Posts,
                    PermissionNames.Pages_Posts_ViewList,
                    PermissionNames.Pages_Posts_Create,
                    PermissionNames.Pages_Posts_Edit,
                    PermissionNames.Pages_Posts_Delete,
                    //Capabilities
                    PermissionNames.Pages_Capabilities,
                    PermissionNames.Pages_Capabilities_ViewList,
                    PermissionNames.Pages_Capabilities_Create,
                    PermissionNames.Pages_Capabilities_Edit,
                    PermissionNames.Pages_Capabilities_Delete,
                    //CapabilitySettings
                    PermissionNames.Pages_CapabilitySettings,
                    PermissionNames.Pages_CapabilitySettings_ViewList,
                    PermissionNames.Pages_CapabilitySettings_Create,
                    PermissionNames.Pages_CapabilitySettings_Edit,
                    PermissionNames.Pages_CapabilitySettings_Clone,
                    PermissionNames.Pages_CapabilitySettings_Delete,
                    PermissionNames.Pages_CapabilitySettings_EditFactor,
                    //PositionSettings
                    PermissionNames.Pages_PositionSettings,
                    PermissionNames.Pages_PositionSettings_ViewList,
                    PermissionNames.Pages_PositionSettings_Create,
                    PermissionNames.Pages_PositionSettings_Edit,
                    PermissionNames.Pages_PositionSettings_Delete,
                    //ScoreSettings
                    PermissionNames.Pages_ScoreSettings,
                    PermissionNames.Pages_ScoreSettings_Create,
                    PermissionNames.Pages_ScoreSettings_Delete,
                    PermissionNames.Pages_ScoreSettings_Edit,
                    PermissionNames.Pages_ScoreSettings_ViewList,
                    #endregion

                    #region Page ApplyCV
                    PermissionNames.Pages_ApplyCV_ViewList,
                    PermissionNames.Pages_ApplyCV_Create,
                    #endregion

                    #region Tab Candidate
                    PermissionNames.TabCandidate,
                    //Intern
                    PermissionNames.Pages_CandidateIntern,
                    PermissionNames.Pages_CandidateIntern_ViewList,
                    PermissionNames.Pages_CandidateIntern_Create,
                    PermissionNames.Pages_CandidateIntern_Export,
                    PermissionNames.Pages_CandidateIntern_Delete,
                    PermissionNames.Pages_CandidateIntern_EditNote,
                    PermissionNames.Pages_CandidateIntern_ViewDetail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Edit,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewSalary,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult,
                    //Staff
                    PermissionNames.Pages_CandidateStaff,
                    PermissionNames.Pages_CandidateStaff_ViewList,
                    PermissionNames.Pages_CandidateStaff_Create,
                    PermissionNames.Pages_CandidateStaff_Export,
                    PermissionNames.Pages_CandidateStaff_Delete,
                    PermissionNames.Pages_CandidateStaff_EditNote,
                    PermissionNames.Pages_CandidateStaff_ViewDetail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_Edit,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_SendMail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Edit,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_CreateAccountLMS,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_SendMail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewSalary,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult,
                    #endregion

                    #region Tab Requisition
                    PermissionNames.TabRequisition,
                    //Staff
                    PermissionNames.Pages_RequisitionStaff,
                    PermissionNames.Pages_RequisitionStaff_ViewList,
                    PermissionNames.Pages_RequisitionStaff_AddCV,
                    PermissionNames.Pages_RequisitionStaff_Clone,
                    PermissionNames.Pages_RequisitionStaff_Close,
                    PermissionNames.Pages_RequisitionStaff_ReOpen,
                    PermissionNames.Pages_RequisitionStaff_Create,
                    PermissionNames.Pages_RequisitionStaff_Delete,
                    PermissionNames.Pages_RequisitionStaff_DeleteRequestCV,
                    PermissionNames.Pages_RequisitionStaff_Edit,
                    PermissionNames.Pages_RequisitionStaff_ViewDetail,
                    //Intern
                    PermissionNames.Pages_RequisitionIntern,
                    PermissionNames.Pages_RequisitionIntern_ViewList,
                    PermissionNames.Pages_RequisitionIntern_AddCV,
                    PermissionNames.Pages_RequisitionIntern_Clone,
                    PermissionNames.Pages_RequisitionIntern_Close,
                    PermissionNames.Pages_RequisitionIntern_ReOpen,
                    PermissionNames.Pages_RequisitionIntern_CloseAndClone,
                    PermissionNames.Pages_RequisitionIntern_CloseAndCloneAll,
                    PermissionNames.Pages_RequisitionIntern_Create,
                    PermissionNames.Pages_RequisitionIntern_Delete,
                    PermissionNames.Pages_RequisitionIntern_DeleteRequestCV,
                    PermissionNames.Pages_RequisitionIntern_Edit,
                    PermissionNames.Pages_RequisitionIntern_ViewDetail,
                    #endregion

                    #region Tab Interview
                    PermissionNames.TabInterview,
                    PermissionNames.Pages_Interviews_ViewList,
                    PermissionNames.Pages_Interviews_ViewOnlyMe,
                    #endregion

                    #region Tab Offers
                    PermissionNames.TabOffers,
                    PermissionNames.Pages_Offers_ViewList,
                    PermissionNames.Pages_Offers_Edit,
                    PermissionNames.Pages_Offers_EditSalary,
                    PermissionNames.Pages_Offers_ViewSalary,
                    #endregion

                    #region Tab Onboard
                    PermissionNames.TabOnboard,
                    PermissionNames.Pages_Onboards_ViewList,
                    PermissionNames.Pages_Onboards_Edit,
                    #endregion

                    #region Tab Report
                    PermissionNames.TabReport,
                    PermissionNames.Pages_Reports_Overview,
                    PermissionNames.Pages_Reports_Overview_Export,
                    PermissionNames.Pages_Reports_Staff_Performance,
                    PermissionNames.Pages_Reports_Intern_Performance,
                    PermissionNames.Pages_Reports_Intern_Education,
                    PermissionNames.Pages_Reports_Intern_Education_Export,
                    #endregion

                    #region NCC Fake CV
                    PermissionNames.MyProfile,
                    PermissionNames.MyProfile_Certification_Create,
                    PermissionNames.MyProfile_Certification_Edit,
                    PermissionNames.MyProfile_Certification_ExportCV,
                    PermissionNames.MyProfile_Certification_View,
                    PermissionNames.MyProfile_Education_Create,
                    PermissionNames.MyProfile_Education_Edit,
                    PermissionNames.MyProfile_Education_Delete,
                    PermissionNames.MyProfile_Education_View,
                    PermissionNames.MyProfile_TechnicalExpertise_Create,
                    PermissionNames.MyProfile_TechnicalExpertise_Edit,
                    PermissionNames.MyProfile_TechnicalExpertise_View,
                    PermissionNames.MyProfile_PersonalAttribute_Delete,
                    PermissionNames.MyProfile_PersonalAttribute_Create,
                    PermissionNames.MyProfile_PersonalAttribute_Edit,
                    PermissionNames.MyProfile_PersonalAttribute_View,
                    PermissionNames.MyProfile_WorkingExperience_Create,
                    PermissionNames.MyProfile_WorkingExperience_Edit,
                    PermissionNames.MyProfile_WorkingExperience_View,
                    PermissionNames.MyProfile_WorkingExperience_Delete,
                    PermissionNames.MyProfile_Version_Create,
                    PermissionNames.MyProfile_Version_Delete,
                    PermissionNames.MyProfile_User_Edit,
                    PermissionNames.MyProfile_User_View,
                    PermissionNames.Employee,
                    PermissionNames.Employee_ViewDetail,
                    PermissionNames.Employee_ViewList,
                    PermissionNames.Employee_EditAsPM,
                    PermissionNames.Project,
                    PermissionNames.Project_Create,
                    PermissionNames.Project_Edit,
                    PermissionNames.Project_ViewList,
                    PermissionNames.WorkingExperience,
                    PermissionNames.WorkingExperience_ViewAll,
                    PermissionNames.GroupSkill,
                    PermissionNames.GroupSkill_View,
                    PermissionNames.GroupSkill_Create,
                    PermissionNames.GroupSkill_Delete,
                    PermissionNames.GroupSkill_Edit,
                    PermissionNames.FakeSkill,
                    PermissionNames.FakeSkill_View,
                    PermissionNames.FakeSkill_Create,
                    PermissionNames.FakeSkill_Delete,
                    PermissionNames.FakeSkill_Edit,
                    PermissionNames.EmployeePosition,
                    PermissionNames.EmployeePosition_View,
                    PermissionNames.EmployeePosition_Create,
                    PermissionNames.EmployeePosition_Delete,
                    PermissionNames.EmployeePosition_Edit
                    #endregion
                }
            },
            {
                StaticRoleNames.Tenants.Recruitment,
                new List<string>()
                {
                    #region Tab Admin
                    PermissionNames.TabAdmin,
                    //Mails
                    PermissionNames.Pages_Mails,
                    PermissionNames.Pages_Mails_ViewList,
                    PermissionNames.Pages_Mails_Edit,
                    PermissionNames.Pages_Mails_Preview,
                    PermissionNames.Pages_Mails_SendMail,
                    #endregion

                    #region Tab Category
                    PermissionNames.TabCategory,
                    //Education Types
                    PermissionNames.Pages_EducationTypes,
                    PermissionNames.Pages_EducationTypes_ViewList,
                    PermissionNames.Pages_EducationTypes_Create,
                    PermissionNames.Pages_EducationTypes_Edit,
                    PermissionNames.Pages_EducationTypes_Delete,
                    //Educations
                    PermissionNames.Pages_Educations,
                    PermissionNames.Pages_Educations_ViewList,
                    PermissionNames.Pages_Educations_Create,
                    PermissionNames.Pages_Educations_Edit,
                    PermissionNames.Pages_Educations_Delete,
                    //Skills
                    PermissionNames.Pages_Skills,
                    PermissionNames.Pages_Skills_ViewList,
                    PermissionNames.Pages_Skills_Create,
                    PermissionNames.Pages_Skills_Edit,
                    PermissionNames.Pages_Skills_Delete,
                    //CVSources
                    PermissionNames.Pages_CVSources,
                    PermissionNames.Pages_CVSources_ViewList,
                    PermissionNames.Pages_CVSources_Create,
                    PermissionNames.Pages_CVSources_Edit,
                    PermissionNames.Pages_CVSources_Delete,
                    //Job Positions
                    PermissionNames.Pages_JobPositions,
                    PermissionNames.Pages_JobPositions_ViewList,
                    PermissionNames.Pages_JobPositions_Create,
                    PermissionNames.Pages_JobPositions_Edit,
                    PermissionNames.Pages_JobPositions_Delete,
                    //Sub Positions
                    PermissionNames.Pages_SubPositions,
                    PermissionNames.Pages_SubPositions_ViewList,
                    PermissionNames.Pages_SubPositions_Create,
                    PermissionNames.Pages_SubPositions_Edit,
                    PermissionNames.Pages_SubPositions_Delete,
                    //Branches
                    PermissionNames.Pages_Branches,
                    PermissionNames.Pages_Branches_ViewList,
                    PermissionNames.Pages_Branches_Create,
                    PermissionNames.Pages_Branches_Edit,
                    PermissionNames.Pages_Branches_Delete,
                    //ExternalCVs
                    PermissionNames.Pages_ExternalCVs,
                    PermissionNames.Pages_ExternalCVs_ViewList,
                    PermissionNames.Pages_ExternalCVs_Create,
                    PermissionNames.Pages_ExternalCVs_Edit,
                    PermissionNames.Pages_ExternalCVs_Delete,
                    PermissionNames.Pages_ExternalCVs_ViewDetail,
                    //Posts
                    PermissionNames.Pages_Posts,
                    PermissionNames.Pages_Posts_ViewList,
                    PermissionNames.Pages_Posts_Create,
                    PermissionNames.Pages_Posts_Edit,
                    PermissionNames.Pages_Posts_Delete,
                    //Capabilities
                    PermissionNames.Pages_Capabilities,
                    PermissionNames.Pages_Capabilities_ViewList,
                    PermissionNames.Pages_Capabilities_Create,
                    PermissionNames.Pages_Capabilities_Edit,
                    PermissionNames.Pages_Capabilities_Delete,
                    //CapabilitySettings
                    PermissionNames.Pages_CapabilitySettings,
                    PermissionNames.Pages_CapabilitySettings_ViewList,
                    PermissionNames.Pages_CapabilitySettings_Create,
                    PermissionNames.Pages_CapabilitySettings_Edit,
                    PermissionNames.Pages_CapabilitySettings_Clone,
                    PermissionNames.Pages_CapabilitySettings_Delete,
                    PermissionNames.Pages_CapabilitySettings_EditFactor,
                    //PositionSettings
                    PermissionNames.Pages_PositionSettings,
                    PermissionNames.Pages_PositionSettings_ViewList,
                    PermissionNames.Pages_PositionSettings_Create,
                    PermissionNames.Pages_PositionSettings_Edit,
                    PermissionNames.Pages_PositionSettings_Delete,
                    #endregion

                    #region Page ApplyCV
                    PermissionNames.Pages_ApplyCV_ViewList,
                    PermissionNames.Pages_ApplyCV_Create,
                    #endregion

                    #region Tab Candidate
                    PermissionNames.TabCandidate,
                    //Intern
                    PermissionNames.Pages_CandidateIntern,
                    PermissionNames.Pages_CandidateIntern_ViewList,
                    PermissionNames.Pages_CandidateIntern_Create,
                    PermissionNames.Pages_CandidateIntern_Export,
                    PermissionNames.Pages_CandidateIntern_Clone,
                    PermissionNames.Pages_CandidateIntern_Delete,
                    PermissionNames.Pages_CandidateIntern_EditNote,
                    PermissionNames.Pages_CandidateIntern_ViewDetail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Edit,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Create,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Delete,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewSalary,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult,
                    //Staff
                    PermissionNames.Pages_CandidateStaff,
                    PermissionNames.Pages_CandidateStaff_ViewList,
                    PermissionNames.Pages_CandidateStaff_Create,
                    PermissionNames.Pages_CandidateStaff_Export,
                    PermissionNames.Pages_CandidateStaff_Clone,
                    PermissionNames.Pages_CandidateStaff_Delete,
                    PermissionNames.Pages_CandidateStaff_EditNote,
                    PermissionNames.Pages_CandidateStaff_ViewDetail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_Edit,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_SendMail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Edit,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Create,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Delete,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_CreateAccountLMS,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_SendMail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewSalary,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult,
                    #endregion

                    #region Tab Requisition
                    PermissionNames.TabRequisition,
                    //Staff
                    PermissionNames.Pages_RequisitionStaff,
                    PermissionNames.Pages_RequisitionStaff_ViewList,
                    PermissionNames.Pages_RequisitionStaff_AddCV,
                    PermissionNames.Pages_RequisitionStaff_Clone,
                    PermissionNames.Pages_RequisitionStaff_Close,
                    PermissionNames.Pages_RequisitionStaff_ReOpen,
                    PermissionNames.Pages_RequisitionStaff_Create,
                    PermissionNames.Pages_RequisitionStaff_Delete,
                    PermissionNames.Pages_RequisitionStaff_DeleteRequestCV,
                    PermissionNames.Pages_RequisitionStaff_Edit,
                    PermissionNames.Pages_RequisitionStaff_ViewDetail,
                    //Intern
                    PermissionNames.Pages_RequisitionIntern,
                    PermissionNames.Pages_RequisitionIntern_ViewList,
                    PermissionNames.Pages_RequisitionIntern_AddCV,
                    PermissionNames.Pages_RequisitionIntern_Clone,
                    PermissionNames.Pages_RequisitionIntern_Close,
                    PermissionNames.Pages_RequisitionIntern_ReOpen,
                    PermissionNames.Pages_RequisitionIntern_CloseAndClone,
                    PermissionNames.Pages_RequisitionIntern_CloseAndCloneAll,
                    PermissionNames.Pages_RequisitionIntern_Create,
                    PermissionNames.Pages_RequisitionIntern_Delete,
                    PermissionNames.Pages_RequisitionIntern_DeleteRequestCV,
                    PermissionNames.Pages_RequisitionIntern_Edit,
                    PermissionNames.Pages_RequisitionIntern_ViewDetail,
                    #endregion

                    #region Tab Interview
                    PermissionNames.TabInterview,
                    PermissionNames.Pages_Interviews_ViewList,
                    #endregion

                    #region Tab Offers
                    PermissionNames.TabOffers,
                    PermissionNames.Pages_Offers_ViewList,
                    PermissionNames.Pages_Offers_Edit,
                    PermissionNames.Pages_Offers_EditSalary,
                    PermissionNames.Pages_Offers_ViewSalary,
                    #endregion

                    #region Tab Onboard
                    PermissionNames.TabOnboard,
                    PermissionNames.Pages_Onboards_ViewList,
                    PermissionNames.Pages_Onboards_Edit,
                    #endregion

                    #region Tab Report
                    PermissionNames.TabReport,
                    PermissionNames.Pages_Reports_Overview,
                    PermissionNames.Pages_Reports_Overview_Export,
                    PermissionNames.Pages_Reports_Staff_Performance,
                    PermissionNames.Pages_Reports_Intern_Performance,
                    PermissionNames.Pages_Reports_Intern_Education,
                    PermissionNames.Pages_Reports_Intern_Education_Export,
                    #endregion

                    #region NCC Fake CV
                    PermissionNames.TabFakeCV,
                    PermissionNames.MyProfile,
                    PermissionNames.MyProfile_Certification_Create,
                    PermissionNames.MyProfile_Certification_Edit,
                    PermissionNames.MyProfile_Certification_ExportCV,
                    PermissionNames.MyProfile_Certification_View,
                    PermissionNames.MyProfile_Education_Create,
                    PermissionNames.MyProfile_Education_Edit,
                    PermissionNames.MyProfile_Education_Delete,
                    PermissionNames.MyProfile_Education_View,
                    PermissionNames.MyProfile_TechnicalExpertise_Create,
                    PermissionNames.MyProfile_TechnicalExpertise_Edit,
                    PermissionNames.MyProfile_TechnicalExpertise_View,
                    PermissionNames.MyProfile_PersonalAttribute_Delete,
                    PermissionNames.MyProfile_PersonalAttribute_Create,
                    PermissionNames.MyProfile_PersonalAttribute_Edit,
                    PermissionNames.MyProfile_PersonalAttribute_View,
                    PermissionNames.MyProfile_WorkingExperience_Create,
                    PermissionNames.MyProfile_WorkingExperience_Edit,
                    PermissionNames.MyProfile_WorkingExperience_View,
                    PermissionNames.MyProfile_WorkingExperience_Delete,
                    PermissionNames.MyProfile_Version_Create,
                    PermissionNames.MyProfile_Version_Delete,
                    PermissionNames.MyProfile_User_Edit,
                    PermissionNames.MyProfile_User_View
                    #endregion
                }
            },
            {
                StaticRoleNames.Tenants.Interviewer,
                new List<string>()
                {
                    #region Page ApplyCV
                    PermissionNames.Pages_ApplyCV_ViewList,
                    #endregion

                    #region Tab Candidate
                    PermissionNames.TabCandidate,
                    //Intern
                    PermissionNames.Pages_CandidateIntern,
                    PermissionNames.Pages_CandidateIntern_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel,
                    //Staff
                    PermissionNames.Pages_CandidateStaff,
                    PermissionNames.Pages_CandidateStaff_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_Education_ViewList,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_View,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_ApplicationHistory,
                    PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel,
                    #endregion
                 
                    #region Tab Interview
                    PermissionNames.TabInterview,
                    PermissionNames.Pages_Interviews_ViewOnlyMe,
                    #endregion

                    #region NCC Fake CV
                    PermissionNames.TabFakeCV,
                    PermissionNames.MyProfile,
                    PermissionNames.MyProfile_Certification_Create,
                    PermissionNames.MyProfile_Certification_Edit,
                    PermissionNames.MyProfile_Certification_ExportCV,
                    PermissionNames.MyProfile_Certification_View,
                    PermissionNames.MyProfile_Education_Create,
                    PermissionNames.MyProfile_Education_Edit,
                    PermissionNames.MyProfile_Education_Delete,
                    PermissionNames.MyProfile_Education_View,
                    PermissionNames.MyProfile_TechnicalExpertise_Create,
                    PermissionNames.MyProfile_TechnicalExpertise_Edit,
                    PermissionNames.MyProfile_TechnicalExpertise_View,
                    PermissionNames.MyProfile_PersonalAttribute_Delete,
                    PermissionNames.MyProfile_PersonalAttribute_Create,
                    PermissionNames.MyProfile_PersonalAttribute_Edit,
                    PermissionNames.MyProfile_PersonalAttribute_View,
                    PermissionNames.MyProfile_WorkingExperience_Create,
                    PermissionNames.MyProfile_WorkingExperience_Edit,
                    PermissionNames.MyProfile_WorkingExperience_View,
                    PermissionNames.MyProfile_WorkingExperience_Delete,
                    PermissionNames.MyProfile_Version_Create,
                    PermissionNames.MyProfile_Version_Delete,
                    PermissionNames.MyProfile_User_Edit,
                    PermissionNames.MyProfile_User_View,
                    #endregion
                }
            },
            {
                StaticRoleNames.Tenants.Sale,
                new List<string>()
                {
                    #region NCC Fake CV
                    PermissionNames.TabFakeCV,
                    PermissionNames.MyProfile,
                    PermissionNames.MyProfile_Certification_Create,
                    PermissionNames.MyProfile_Certification_Edit,
                    PermissionNames.MyProfile_Certification_ExportCV,
                    PermissionNames.MyProfile_Certification_View,
                    PermissionNames.MyProfile_Education_Create,
                    PermissionNames.MyProfile_Education_Edit,
                    PermissionNames.MyProfile_Education_Delete,
                    PermissionNames.MyProfile_Education_View,
                    PermissionNames.MyProfile_TechnicalExpertise_Create,
                    PermissionNames.MyProfile_TechnicalExpertise_Edit,
                    PermissionNames.MyProfile_TechnicalExpertise_View,
                    PermissionNames.MyProfile_PersonalAttribute_Delete,
                    PermissionNames.MyProfile_PersonalAttribute_Create,
                    PermissionNames.MyProfile_PersonalAttribute_Edit,
                    PermissionNames.MyProfile_PersonalAttribute_View,
                    PermissionNames.MyProfile_WorkingExperience_Create,
                    PermissionNames.MyProfile_WorkingExperience_Edit,
                    PermissionNames.MyProfile_WorkingExperience_View,
                    PermissionNames.MyProfile_WorkingExperience_Delete,
                    PermissionNames.MyProfile_Version_Create,
                    PermissionNames.MyProfile_Version_Delete,
                    PermissionNames.MyProfile_User_Edit,
                    PermissionNames.MyProfile_User_View,
                    PermissionNames.Employee,
                    PermissionNames.Employee_ViewDetail,
                    PermissionNames.Employee_ViewList,
                    PermissionNames.Employee_EditAsPM,
                    PermissionNames.Project,
                    PermissionNames.Project_Create,
                    PermissionNames.Project_Edit,
                    PermissionNames.Project_ViewList,
                    PermissionNames.WorkingExperience,
                    PermissionNames.WorkingExperience_ViewAll,
                    PermissionNames.GroupSkill,
                    PermissionNames.GroupSkill_View,
                    PermissionNames.GroupSkill_Create,
                    PermissionNames.GroupSkill_Delete,
                    PermissionNames.GroupSkill_Edit,
                    PermissionNames.FakeSkill,
                    PermissionNames.FakeSkill_View,
                    PermissionNames.FakeSkill_Create,
                    PermissionNames.FakeSkill_Delete,
                    PermissionNames.FakeSkill_Edit,
                    PermissionNames.EmployeePosition,
                    PermissionNames.EmployeePosition_View,
                    PermissionNames.EmployeePosition_Create,
                    PermissionNames.EmployeePosition_Delete,
                    PermissionNames.EmployeePosition_Edit
                    #endregion
                }
            },
            {
            StaticRoleNames.Tenants.BasicUser,
            new List<string>()
            {
                #region NCC Fake CV
                PermissionNames.TabFakeCV,
                PermissionNames.MyProfile,
                PermissionNames.MyProfile_Certification_Create,
                PermissionNames.MyProfile_Certification_Edit,
                PermissionNames.MyProfile_Certification_ExportCV,
                PermissionNames.MyProfile_Certification_View,
                PermissionNames.MyProfile_Education_Create,
                PermissionNames.MyProfile_Education_Edit,
                PermissionNames.MyProfile_Education_Delete,
                PermissionNames.MyProfile_Education_View,
                PermissionNames.MyProfile_TechnicalExpertise_Create,
                PermissionNames.MyProfile_TechnicalExpertise_Edit,
                PermissionNames.MyProfile_TechnicalExpertise_View,
                PermissionNames.MyProfile_PersonalAttribute_Delete,
                PermissionNames.MyProfile_PersonalAttribute_Create,
                PermissionNames.MyProfile_PersonalAttribute_Edit,
                PermissionNames.MyProfile_PersonalAttribute_View,
                PermissionNames.MyProfile_WorkingExperience_Create,
                PermissionNames.MyProfile_WorkingExperience_Edit,
                PermissionNames.MyProfile_WorkingExperience_View,
                PermissionNames.MyProfile_WorkingExperience_Delete,
                PermissionNames.MyProfile_Version_Create,
                PermissionNames.MyProfile_Version_Delete,
                PermissionNames.MyProfile_User_Edit,
                PermissionNames.MyProfile_User_View
                #endregion
            }
        },
        };
        public class SystemPermission
        {
            public string Name { get; set; }
            public MultiTenancySides MultiTenancySides { get; set; }
            public string DisplayName { get; set; }
            public bool IsConfiguration { get; set; }
            public List<SystemPermission> Children { get; set; }

            public static List<SystemPermission> ListPermissions = new List<SystemPermission>()
            {
                #region Tab Admin
                new SystemPermission{ Name = PermissionNames.TabAdmin,DisplayName = "Tab Admin",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //Role
                new SystemPermission{ Name = PermissionNames.Pages_Roles,DisplayName = "Page Role",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_Roles_ViewList, DisplayName = "View List Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_Roles_Create, DisplayName = "Create Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_Roles_Edit, DisplayName = "Edit Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_Roles_Delete, DisplayName = "Delete Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                //Tenant
                new SystemPermission { Name = PermissionNames.Pages_Tenants,DisplayName = "Page Tenant",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Tenants_ViewList, DisplayName = "View List Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Tenants_Create, DisplayName = "Create Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Tenants_Edit, DisplayName = "Edit Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Tenants_Delete, DisplayName = "Delete Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //User
                new SystemPermission { Name = PermissionNames.Pages_Users, DisplayName = "Page User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_ViewList, DisplayName = "View List User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_Create, DisplayName = "Create User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_Edit, DisplayName = "Edit User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_Delete, DisplayName = "Delete User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_Activation, DisplayName = "Activatiion User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission { Name = PermissionNames.Pages_Users_ResetPassword, DisplayName = "Reset Password", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                 //Configurations
                new SystemPermission{Name = PermissionNames.Pages_Configurations, DisplayName = "Page Configuration", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewChannelHRITSettings, DisplayName = "View Setting Channel HR IT", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditChannelHRITSettings, DisplayName = "Edit Setting Channel HR IT", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewKomuSettings, DisplayName = "View Setting Komu", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewEmailSettings, DisplayName = "View Setting Email", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditEmailSettings, DisplayName = "Edit Setting Email", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewHRMSettings, DisplayName = "View Setting HRM", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewLMSSettings, DisplayName = "View Setting LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewGoogleClientAppSettings, DisplayName = "View Setting Google Client App", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditGoogleClientAppSettings, DisplayName = "Edit Setting Google Client App", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewTalentSecretCode, DisplayName = "View Talent Secret Code", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditTalentSecretCode, DisplayName = "Edit Talent Secret Code", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewTalentNotifyInterviewSettings, DisplayName = "View Talent Notify Interview Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditTalentNotifyInterviewSettings, DisplayName = "Edit Talent Notify Interview Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //Mail
                new SystemPermission {Name = PermissionNames.Pages_Mails, DisplayName = "Page Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Mails_ViewList, DisplayName = "View List Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Mails_Edit, DisplayName = "Edit Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Mails_Preview, DisplayName = "Preview Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Mails_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region Tab Category
                new SystemPermission {Name = PermissionNames.TabCategory, DisplayName = "Tab Category", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Education Type
                new SystemPermission {Name = PermissionNames.Pages_EducationTypes, DisplayName = "Pages Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_ViewList, DisplayName = "View List Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Create, DisplayName = "Create Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Edit, DisplayName = "Edit Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Delete, DisplayName = "Delete Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Education
                new SystemPermission {Name = PermissionNames.Pages_Educations, DisplayName = "Pages Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Educations_ViewList, DisplayName = "View List Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Educations_Create, DisplayName = "Create Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Educations_Edit, DisplayName = "Edit Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Educations_Delete, DisplayName = "Delete Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Skill
                new SystemPermission {Name = PermissionNames.Pages_Skills, DisplayName = "Pages Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Skills_ViewList, DisplayName = "View List Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Skills_Create, DisplayName = "Create Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Skills_Edit, DisplayName = "Edit Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Skills_Delete, DisplayName = "Delete Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // CV Source
                new SystemPermission {Name = PermissionNames.Pages_CVSources, DisplayName = "Pages CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CVSources_ViewList, DisplayName = "View List CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CVSources_Create, DisplayName = "Create CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CVSources_Edit, DisplayName = "Edit CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CVSources_Delete, DisplayName = "Delete CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Job Position 
                new SystemPermission {Name = PermissionNames.Pages_JobPositions, DisplayName = "Pages JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_JobPositions_ViewList, DisplayName = "View List JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Create, DisplayName = "Create JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Edit, DisplayName = "Edit JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Delete, DisplayName = "Delete JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Sub Position
                new SystemPermission {Name = PermissionNames.Pages_SubPositions, DisplayName = "Pages SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_SubPositions_ViewList, DisplayName = "View List SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Create, DisplayName = "Create SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Edit, DisplayName = "Edit SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Delete, DisplayName = "Delete SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Branch
                new SystemPermission {Name = PermissionNames.Pages_Branches, DisplayName = "Pages Branch", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Branches_ViewList, DisplayName = "View List Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Branches_Create, DisplayName = "Create Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Branches_Edit, DisplayName = "Edit Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Branches_Delete, DisplayName = "Delete Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // ExternalCV
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs, DisplayName = "Pages ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_ViewList, DisplayName = "View List ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Create, DisplayName = "Create ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Edit, DisplayName = "Edit ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Delete, DisplayName = "Delete ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_ViewDetail, DisplayName = "View Detail ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Post
                new SystemPermission {Name = PermissionNames.Pages_Posts, DisplayName = "Pages Post", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Posts_ViewList, DisplayName = "View List Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Posts_Create, DisplayName = "Create Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Posts_Edit, DisplayName = "Edit Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Posts_Delete, DisplayName = "Delete Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Capability
                new SystemPermission {Name = PermissionNames.Pages_Capabilities, DisplayName = "Pages Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Capabilities_ViewList, DisplayName = "View List Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Create, DisplayName = "Create Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Edit, DisplayName = "Edit Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Delete, DisplayName = "Delete Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Capability Settings
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings, DisplayName = "Pages Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_ViewList, DisplayName = "View List Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Create, DisplayName = "Create Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Edit, DisplayName = "Edit Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Clone, DisplayName = "Clone Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Delete, DisplayName = "Delete Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_EditFactor, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                // Position Setting
                new SystemPermission {Name = PermissionNames.Pages_PositionSettings, DisplayName = "Pages Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_ViewList, DisplayName = "View List Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Create, DisplayName = "Create Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Edit, DisplayName = "Edit Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Delete, DisplayName = "Delete Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                // Score Setting
                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings, DisplayName = "Pages Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_ViewList, DisplayName = "View List Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Create, DisplayName = "Create Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Edit, DisplayName = "Edit Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Delete, DisplayName = "Delete Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                
                #endregion

                #region Page ApplyCV
                new SystemPermission{ Name = PermissionNames.Pages_ApplyCV_ViewList, DisplayName = "View List ApplyCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_ApplyCV_Create, DisplayName = "Create ApplyCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                #endregion

                #region Tab Candidate
                new SystemPermission {Name = PermissionNames.TabCandidate, DisplayName = "Tab Candidate", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //Intern
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern, DisplayName = "Pages Candidate Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewList, DisplayName = "View List Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Create, DisplayName = "Create Candidate Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                 new SystemPermission { Name = PermissionNames.Pages_CandidateIntern_Export, DisplayName = "Export Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Delete, DisplayName = "Delete Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_EditNote, DisplayName = "Edit Note Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail,DisplayName = "View Detail Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Clone, DisplayName = "Clone Candidate Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },

                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo, DisplayName = "View Tab Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View, DisplayName = "View Detail Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit, DisplayName = "Edit Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail, DisplayName = "Send Mail FailCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill, DisplayName = "View Tab Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList, DisplayName = "View List Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Create, DisplayName = "Create Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Edit, DisplayName = "Edit Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Delete, DisplayName = "Delete Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education, DisplayName = "View Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList, DisplayName = "View List Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Create, DisplayName = "Create Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Delete, DisplayName = "Delete Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV, DisplayName = "View Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_View, DisplayName = "View Current Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Create, DisplayName = "Add CV into Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Delete, DisplayName = "Delete CV from Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview, DisplayName = "View/Edit Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability, DisplayName = "View Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability, DisplayName = "Edit Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult, DisplayName = "View Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult, DisplayName = "Edit Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS, DisplayName = "Create account LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary, DisplayName = "Edit Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel, DisplayName = "Edit Interview Level", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus, DisplayName = "Edit Application Status", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_ApplicationHistory, DisplayName = "View Application History", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //Staff
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff, DisplayName = "Pages Candidate Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewList, DisplayName = "View List Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Create, DisplayName = "Create Candidate Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission { Name = PermissionNames.Pages_CandidateStaff_Export, DisplayName = "Export Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Delete, DisplayName = "Delete Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_EditNote, DisplayName = "Edit Note Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail,DisplayName = "View Detail Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Clone, DisplayName = "Clone Candidate Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },

                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo, DisplayName = "View Tab Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View, DisplayName = "View Detail Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_Edit, DisplayName = "Edit Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_SendMail, DisplayName = "Send Mail FailCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill, DisplayName = "View Tab Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_ViewList, DisplayName = "View List", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Create, DisplayName = "Create Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Edit, DisplayName = "Edit Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Delete, DisplayName = "Delete Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education, DisplayName = "View Tab Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_ViewList, DisplayName = "View List Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Create, DisplayName = "Create Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Delete, DisplayName = "Delete Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV, DisplayName = "View Tab Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_View, DisplayName = "View Current Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Create, DisplayName = "Add CV into Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Delete, DisplayName = "Delete CV from Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview, DisplayName = "View/Edit Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability, DisplayName = "View Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability, DisplayName = "Edit Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult, DisplayName = "View Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult, DisplayName = "Edit Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_CreateAccountLMS, DisplayName = "Create account LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary, DisplayName = "Edit Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel, DisplayName = "Edit Interview Level", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus, DisplayName = "Edit Application Status", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_ApplicationHistory, DisplayName = "View Application History", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region Tab Requisition
                new SystemPermission{ Name = PermissionNames.TabRequisition, DisplayName = "Tab Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                //Requisition Intern
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern,DisplayName = "Page Requisition Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ViewList, DisplayName = "View Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ViewDetail, DisplayName = "View Detail Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Create, DisplayName = "Create Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Edit, DisplayName = "Edit Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Delete, DisplayName = "Delete Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_AddCV, DisplayName = "Add CV to Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Clone, DisplayName = "Clone Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Close, DisplayName = "Close Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_CloseAndClone, DisplayName = "Close & Clone Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_CloseAndCloneAll, DisplayName = "Close & Clone All Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ReOpen, DisplayName = "Reopen Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_DeleteRequestCV, DisplayName = "Delete CV from Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                
                //Requisition Staff
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff, DisplayName = "Page Requisition Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ViewList, DisplayName = "View Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ViewDetail, DisplayName = "View Detail Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Create, DisplayName = "Create Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Edit, DisplayName = "Edit Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Delete, DisplayName = "Delete Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_AddCV, DisplayName = "Add CV to Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Clone, DisplayName = "Clone Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Close, DisplayName = "Close Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ReOpen, DisplayName = "Reopen Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_DeleteRequestCV, DisplayName = "Delete CV from Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region Tab Interview
                new SystemPermission {Name = PermissionNames.TabInterview, DisplayName = "Tab Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Interviews_ViewList, DisplayName = "View All Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Interviews_ViewOnlyMe, DisplayName = "View Only Me", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                #endregion
                #region Tab Offers
                new SystemPermission {Name = PermissionNames.TabOffers, DisplayName = "Tab Offers", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Offers_ViewList, DisplayName = "View List Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Offers_Edit, DisplayName = "Edit Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Offers_EditSalary, DisplayName = "Edit Salary Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Offers_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region Tab Onboard
                new SystemPermission {Name = PermissionNames.TabOnboard, DisplayName = "Tab Onboard", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Onboards_ViewList, DisplayName = "View List Candidate Onboard", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Onboards_Edit, DisplayName = "Edit Candidate Onboard", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region Tab Report
                new SystemPermission {Name = PermissionNames.TabReport, DisplayName = "Tab Report", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Overview, DisplayName = "View Report Overview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Overview_Export, DisplayName = "View Report Overview Export", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Staff_Performance, DisplayName = "View Report Staff Perfomance", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Performance, DisplayName = "View Report Intern Perfomance", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Education, DisplayName = "View Report Intern Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Education_Export, DisplayName = "View Report Intern Education Export", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                #endregion
                #region NCC Fake CVs
                new SystemPermission{ Name =  PermissionNames.TabFakeCV, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Fake CV Employee" },
                //for myprofile
                new SystemPermission{ Name =  PermissionNames.MyProfile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "MyProfile" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Certification" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Certification" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Certification" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Certification" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification_ExportCV, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Certification" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Education, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Education" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Education" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Education" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Education" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Education" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Technical Expertise" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Technical Expertise" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Technical Expertise" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Technical Expertise" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Personal Attribute" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Personal Attribute" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Personal Attribute" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Personal Attribute" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Personal Attribute" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Working Experience" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Working Experience" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Working Experience" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Working Experience" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Working Experience" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Version, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Version" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Version_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Version" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_Version_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Version" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_User, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Profile User" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit User" },
                new SystemPermission{ Name =  PermissionNames.MyProfile_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View User" },
                //for employee
                new SystemPermission{ Name =  PermissionNames.Employee, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee" },
                new SystemPermission{ Name =  PermissionNames.Employee_ViewDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Detail Employee" },
                new SystemPermission{ Name =  PermissionNames.Employee_ViewList, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View List Employee" },
                new SystemPermission{ Name =  PermissionNames.Employee_EditAsPM, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit As PM" },
                //for project
                new SystemPermission{ Name =  PermissionNames.Project, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Project" },
                new SystemPermission{ Name =  PermissionNames.Project_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Project" },
                new SystemPermission{ Name =  PermissionNames.Project_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Project" },
                new SystemPermission{ Name =  PermissionNames.Project_ViewList, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View list Project" },
                //for WorkingExperience
                new SystemPermission{ Name =  PermissionNames.WorkingExperience, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Working Experience" },
                new SystemPermission{ Name =  PermissionNames.WorkingExperience_ViewAll, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View All Working Experience" },
                //for FakeSkill
                new SystemPermission{ Name =  PermissionNames.FakeSkill, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Fake Skill" },
                new SystemPermission{ Name =  PermissionNames.FakeSkill_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Fake Skill" },
                new SystemPermission{ Name =  PermissionNames.FakeSkill_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Fake Skill" },
                new SystemPermission{ Name =  PermissionNames.FakeSkill_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Fake Skill" },
                new SystemPermission{ Name =  PermissionNames.FakeSkill_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Fake Skill" },
                //for GroupSkill
                new SystemPermission{ Name =  PermissionNames.GroupSkill, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Group Skill" },
                new SystemPermission{ Name =  PermissionNames.GroupSkill_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Group Skill" },
                new SystemPermission{ Name =  PermissionNames.GroupSkill_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Group Skill" },
                new SystemPermission{ Name =  PermissionNames.GroupSkill_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Group Skill" },
                new SystemPermission{ Name =  PermissionNames.GroupSkill_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Group Skill" },
                //for EmployeePosition
                new SystemPermission{ Name =  PermissionNames.EmployeePosition, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee Position" },
                new SystemPermission{ Name =  PermissionNames.EmployeePosition_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Employee Position" },
                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Employee Position" },
                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Employee Position" },
                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Employee Position" },
                #endregion
            };
            public static List<SystemPermission> TreePermissions = new List<SystemPermission>()
            {
                #region Tab Admin
                new SystemPermission
                {
                    Name = PermissionNames.TabAdmin,
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    DisplayName = "Admin",
                    Children = new List<SystemPermission>()
                    {
                        #region Page Roles
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Roles,
                            DisplayName = "Page Roles",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_Roles_ViewList, DisplayName = "View List Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_Roles_Create, DisplayName = "Create Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_Roles_Edit, DisplayName = "Edit Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_Roles_Delete, DisplayName = "Delete Role", MultiTenancySides = MultiTenancySides.Host |  MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Page Tenants
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Tenants,
                            DisplayName = "Page Tenants",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission { Name = PermissionNames.Pages_Tenants_ViewList, DisplayName = "View List Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Tenants_Create, DisplayName = "Create Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Tenants_Edit, DisplayName = "Edit Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Tenants_Delete, DisplayName = "Delete Tenant", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Page Users
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Users,
                            DisplayName = "Page Users",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission { Name = PermissionNames.Pages_Users_ViewList, DisplayName = "View List User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Users_Create, DisplayName = "Create User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Users_Edit, DisplayName = "Edit User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Users_Delete, DisplayName = "Delete User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Users_Activation, DisplayName = "Activatiion User", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission { Name = PermissionNames.Pages_Users_ResetPassword, DisplayName = "Reset Password", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Page Configurations
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Configurations,
                            DisplayName = "Page Configurations",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewChannelHRITSettings, DisplayName = "View List Discord Channel", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditChannelHRITSettings, DisplayName = "Edit List Discord Channel", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewKomuSettings, DisplayName = "View Setting Komu", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewEmailSettings, DisplayName = "View Setting Email", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditEmailSettings, DisplayName = "Edit Setting Email", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewHRMSettings, DisplayName = "View Setting HRM", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewLMSSettings, DisplayName = "View Setting LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewGoogleClientAppSettings, DisplayName = "View Setting Google Client App", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditGoogleClientAppSettings, DisplayName = "Edit Setting Google Client App", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewTalentSecretCode, DisplayName = "View Talent Secret Code", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditTalentSecretCode, DisplayName = "Edit Talent Secret Code", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_ViewTalentNotifyInterviewSettings, DisplayName = "View Talent Notify Interview Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{Name = PermissionNames.Pages_Configurations_EditTalentNotifyInterviewSettings, DisplayName = "Edit Talent Notify Interview Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                            }
                        },
                        #endregion
                        #region Page Mails
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Mails,
                            DisplayName = "Page Mails",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Mails_ViewList, DisplayName = "View List Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Mails_Edit, DisplayName = "Edit Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Mails_Preview, DisplayName = "Preview Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Mails_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                    }
                },
                #endregion
                #region Tab Category
                new SystemPermission
                {
                    Name = PermissionNames.TabCategory,
                    DisplayName = "Category",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>()
                    {
                        #region Pages EducationTypes
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_EducationTypes,
                            DisplayName = "Page Education Types",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_ViewList, DisplayName = "View List Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Create, DisplayName = "Create Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Edit, DisplayName = "Edit Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_EducationTypes_Delete, DisplayName = "Delete Education Types", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Educations
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Educations,
                            DisplayName = "Page Educations",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Educations_ViewList, DisplayName = "View List Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Educations_Create, DisplayName = "Create Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Educations_Edit, DisplayName = "Edit Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Educations_Delete, DisplayName = "Delete Educations", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Skills
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Skills,
                            DisplayName = "Page Skills",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Skills_ViewList, DisplayName = "View List Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Skills_Create, DisplayName = "Create Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Skills_Edit, DisplayName = "Edit Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Skills_Delete, DisplayName = "Delete Skills", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages CVSources
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_CVSources,
                            DisplayName = "Page CVSources",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_CVSources_ViewList, DisplayName = "View List CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CVSources_Create, DisplayName = "Create CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CVSources_Edit, DisplayName = "Edit CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CVSources_Delete, DisplayName = "Delete CVSources", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages JobPositions 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_JobPositions,
                            DisplayName = "Page JobPositions",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_JobPositions_ViewList, DisplayName = "View List JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Create, DisplayName = "Create JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Edit, DisplayName = "Edit JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_JobPositions_Delete, DisplayName = "Delete JobPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages SubPositions 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_SubPositions,
                            DisplayName = "Page SubPositions",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_SubPositions_ViewList, DisplayName = "View List SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Create, DisplayName = "Create SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Edit, DisplayName = "Edit SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_SubPositions_Delete, DisplayName = "Delete SubPositions", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Branches 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Branches,
                            DisplayName = "Page Branches",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Branches_ViewList, DisplayName = "View List Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Branches_Create, DisplayName = "Create Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Branches_Edit, DisplayName = "Edit Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Branches_Delete, DisplayName = "Delete Branches", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages ExternalCVs 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_ExternalCVs,
                            DisplayName = "Page ExternalCVs",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_ViewList, DisplayName = "View List ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Create, DisplayName = "Create ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Edit, DisplayName = "Edit ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_Delete, DisplayName = "Delete ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ExternalCVs_ViewDetail, DisplayName = "View Detail ExternalCVs", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Posts 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Posts,
                            DisplayName = "Page Posts",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Posts_ViewList, DisplayName = "View List Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Posts_Create, DisplayName = "Create Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Posts_Edit, DisplayName = "Edit Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Posts_Delete, DisplayName = "Delete Posts", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Capabilities 
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_Capabilities,
                            DisplayName = "Page Capabilities",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_Capabilities_ViewList, DisplayName = "View List Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Create, DisplayName = "Create Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Edit, DisplayName = "Edit Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_Capabilities_Delete, DisplayName = "Delete Capabilities", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages CapabilitySettings
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_CapabilitySettings,
                            DisplayName = "Page Capability Settings",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_ViewList, DisplayName = "View List Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Create, DisplayName = "Create Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Edit, DisplayName = "Edit Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Clone, DisplayName = "Clone Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_Delete, DisplayName = "Delete Capability Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_CapabilitySettings_EditFactor, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages PositionSettings
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_PositionSettings,
                            DisplayName = "Page PositionSettings",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_ViewList, DisplayName = "View List Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Create, DisplayName = "Create Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Edit, DisplayName = "Edit Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_PositionSettings_Delete, DisplayName = "Delete Position Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages ScoreSettings
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_ScoreSettings,
                            DisplayName = "Page ScoreSettings",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_ViewList, DisplayName = "View List Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Create, DisplayName = "Create Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Delete, DisplayName = "Delete Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission {Name = PermissionNames.Pages_ScoreSettings_Edit, DisplayName = "Edit Score Settings", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                            }
                        }
                        #endregion
                    }
                },
                #endregion

                #region Page ApplyCV
                 new SystemPermission
                        {
                            Name = PermissionNames.Pages_ApplyCV,
                            DisplayName = "Page ApplyCV",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_ApplyCV_ViewList, DisplayName = "View List ApplyCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_ApplyCV_Create, DisplayName = "Create ApplyCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                            }
                        },
                #endregion

                #region Tab Candidate
                new SystemPermission
                {
                    Name = PermissionNames.TabCandidate,
                    DisplayName = "Candidate",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        #region Pages Candidate Staff
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_CandidateStaff,
                            DisplayName = "Page Candidate Staff",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewList, DisplayName = "View List Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Create, DisplayName = "Create Candidate Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission { Name = PermissionNames.Pages_CandidateStaff_Export, DisplayName = "Export Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Clone, DisplayName = "Clone Candidate Staff",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_Delete, DisplayName = "Delete Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_EditNote, DisplayName = "Edit Note Candidate Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission
                                {
                                    Name = PermissionNames.Pages_CandidateStaff_ViewDetail,
                                    DisplayName = "View Detail Staff",
                                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                    Children = new List<SystemPermission>
                                    {
                                        #region tab person info
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo,
                                            DisplayName = "View Tab Person Info",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View, DisplayName = "View Detail Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_Edit, DisplayName = "Edit Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_SendMail, DisplayName = "Send Mail FailCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab skills
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill,
                                            DisplayName = "View Tab Skills",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_ViewList, DisplayName = "View List Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Create, DisplayName = "Create Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Edit, DisplayName = "Edit Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Skill_Delete, DisplayName = "Delete Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab educations
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education,
                                            DisplayName = "View Tab Educations",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_ViewList, DisplayName = "View List Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Create, DisplayName = "Create Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_Education_Delete, DisplayName = "Delete Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab current requisitions
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV,
                                            DisplayName = "View Tab Current Requisitions",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_View, DisplayName = "View Current Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Create, DisplayName = "Add CV into Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_Delete, DisplayName = "Delete CV from Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewEditInterview, DisplayName = "View/Edit Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewCapability, DisplayName = "View Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditCapability, DisplayName = "Edit Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewApplicationResult, DisplayName = "View Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationResult, DisplayName = "Edit Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_CreateAccountLMS, DisplayName = "Create account LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditSalary, DisplayName = "Edit Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditInterviewLevel, DisplayName = "Edit Interview Level", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditApplicationStatus, DisplayName = "Edit Application Status", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_RequestCV_EditFactorCapabilityResult, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},

                                            }
                                        },
                                        #endregion
                                        #region tab application history
                                        new SystemPermission{ Name = PermissionNames.Pages_CandidateStaff_ViewDetail_ApplicationHistory, DisplayName = "View Tab Application History", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                        #endregion
                                    }
                                },
                            }
                        },
                        #endregion
                        #region Pages Candidate Intern
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_CandidateIntern,
                            DisplayName = "Page Candidate Intern",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewList, DisplayName = "View List Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Create, DisplayName = "Create Candidate Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission { Name = PermissionNames.Pages_CandidateIntern_Export, DisplayName = "Export Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Clone, DisplayName = "Clone Candidate Intern",MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_Delete, DisplayName = "Delete Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_EditNote, DisplayName = "Edit Note Candidate Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant },
                                new SystemPermission
                                {
                                    Name = PermissionNames.Pages_CandidateIntern_ViewDetail,
                                    DisplayName = "View Detail Intern",
                                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                    Children = new List<SystemPermission>
                                    {
                                        #region tab person info
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo,
                                            DisplayName = "View Tab Person Info",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View, DisplayName = "View Detail Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_Edit, DisplayName = "Edit Person", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_SendMail, DisplayName = "Send Mail FailCV", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab skills
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill,
                                            DisplayName = "View Tab Skills",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_ViewList, DisplayName = "View List Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Create, DisplayName = "Create Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Edit, DisplayName = "Edit Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Skill_Delete, DisplayName = "Delete Skill", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab educations
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education,
                                            DisplayName = "View Tab Educations",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_ViewList, DisplayName = "View List Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Create, DisplayName = "Create Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_Education_Delete, DisplayName = "Delete Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab current requisitions
                                        new SystemPermission
                                        {
                                            Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV,
                                            DisplayName = "View Tab Current Requisitions",
                                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                                            Children = new List<SystemPermission>
                                            {
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_View, DisplayName = "View Current Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Create, DisplayName = "Add CV into Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_Delete, DisplayName = "Delete CV from Request", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewEditInterview, DisplayName = "View/Edit Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewCapability, DisplayName = "View Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditCapability, DisplayName = "Edit Capability", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewApplicationResult, DisplayName = "View Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationResult, DisplayName = "Edit Application Result", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_CreateAccountLMS, DisplayName = "Create account LMS", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_SendMail, DisplayName = "Send Mail", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditSalary, DisplayName = "Edit Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditInterviewLevel, DisplayName = "Edit Interview Level", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditApplicationStatus, DisplayName = "Edit Application Status", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                                new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_RequestCV_EditFactorCapabilityResult, DisplayName = "Edit Factor", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                            }
                                        },
                                        #endregion
                                        #region tab application history
                                        new SystemPermission{ Name = PermissionNames.Pages_CandidateIntern_ViewDetail_ApplicationHistory, DisplayName = "View Tab Application History", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                        #endregion
                                    }
                                },
                            }
                        }
                        #endregion
                    }
                },
                #endregion
                #region Tab Requisitions
                new SystemPermission
                {
                    Name = PermissionNames.TabRequisition,
                    DisplayName = "Requisition",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        #region Pages Requisition Staff
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_RequisitionStaff,
                            DisplayName = "Page Requisition Staff",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ViewList, DisplayName = "View Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ViewDetail, DisplayName = "View Detail Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Create, DisplayName = "Create Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Edit, DisplayName = "Edit Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Delete, DisplayName = "Delete Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_AddCV, DisplayName = "Add CV to Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Clone, DisplayName = "Clone Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_Close, DisplayName = "Close Request Staff", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_ReOpen, DisplayName = "Reopen Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionStaff_DeleteRequestCV, DisplayName = "Delete CV from Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        },
                        #endregion
                        #region Pages Requisition Intern
                        new SystemPermission
                        {
                            Name = PermissionNames.Pages_RequisitionIntern,
                            DisplayName = "Page Requisition Intern",
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ViewList, DisplayName = "View Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ViewDetail, DisplayName = "View Detail Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Create, DisplayName = "Create Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Edit, DisplayName = "Edit Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Delete, DisplayName = "Delete Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_AddCV, DisplayName = "Add CV to Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Clone, DisplayName = "Clone Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_Close, DisplayName = "Close Request Intern", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_CloseAndClone, DisplayName = "Close & Clone Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_CloseAndCloneAll, DisplayName = "Close & Clone All Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_ReOpen, DisplayName = "Reopen Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                                new SystemPermission{ Name = PermissionNames.Pages_RequisitionIntern_DeleteRequestCV, DisplayName = "Delete CV from Requisition", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                            }
                        }
                        #endregion
                    }
                },
                #endregion
                #region Tab Interview
                new SystemPermission
                {
                    Name = PermissionNames.TabInterview,
                    DisplayName = "Interview",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        new SystemPermission {Name = PermissionNames.Pages_Interviews_ViewList, DisplayName = "View All Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Interviews_ViewOnlyMe, DisplayName = "View My Interview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                    }
                },
                #endregion
                #region Tab Offers
                new SystemPermission
                {
                    Name = PermissionNames.TabOffers,
                    DisplayName = "Offers",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        new SystemPermission {Name = PermissionNames.Pages_Offers_ViewList, DisplayName = "View List Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Offers_Edit, DisplayName = "Edit Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Offers_EditSalary, DisplayName = "Edit Salary Candidate Offer", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Offers_ViewSalary, DisplayName = "View Salary", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                    }
                },
                #endregion
                #region Tab Onboard 
                new SystemPermission
                {
                    Name = PermissionNames.TabOnboard,
                    DisplayName = "Onboard",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        new SystemPermission {Name = PermissionNames.Pages_Onboards_ViewList, DisplayName = "View List Candidate Onboard", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Onboards_Edit, DisplayName = "Edit Candidate Onboard", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                    }
                },
                #endregion
                #region Tab Report
                new SystemPermission
                {
                    Name = PermissionNames.TabReport,
                    DisplayName = "Report",
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    Children = new List<SystemPermission>
                    {
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Overview, DisplayName = "View Report Overview", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Overview_Export, DisplayName = "View Report Overview Export", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Staff_Performance, DisplayName = "View Report Staff Perfomance", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Performance, DisplayName = "View Report Intern Perfomance", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Education, DisplayName = "View Report Intern Education", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                        new SystemPermission {Name = PermissionNames.Pages_Reports_Intern_Education_Export, DisplayName = "View Report Intern Education Export", MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant},
                    }
                },
                #endregion 
                #region NCC Fake CVs
                new SystemPermission
                {
                    Name =  PermissionNames.TabFakeCV,
                    MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                    DisplayName = "Employee Profile",
                    Children = new List<SystemPermission>
                    {
                        new SystemPermission{ Name =  PermissionNames.MyProfile, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "My Profile",
                            Children = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.MyProfile_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View My Profile" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Information Profile" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Version_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Version" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Version_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Version" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Education" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Education" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Education" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Education_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Education" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Technical Expertise" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Technical Expertise" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_TechnicalExpertise_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Technical Expertise" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Personal Attribute" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Personal Attribute" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Personal Attribute" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_PersonalAttribute_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Personal Attribute" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Working Experience" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Working Experience" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Working Experience" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_WorkingExperience_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Working Experience" },
                                new SystemPermission{ Name =  PermissionNames.MyProfile_Certification_ExportCV, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export" },
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.Employee, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee",
                           Children = new List<SystemPermission>()
                           {
                               new SystemPermission{ Name =  PermissionNames.Employee_ViewList, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View List Employee" },
                               new SystemPermission{ Name =  PermissionNames.Employee_ViewDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Detail Employee" },
                               new SystemPermission{ Name =  PermissionNames.Employee_EditAsPM, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit working experience other employee" },
                           }
                        },
                        new SystemPermission{ Name =  PermissionNames.Project, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Project",
                           Children = new List<SystemPermission>()
                           {
                                new SystemPermission{ Name =  PermissionNames.Project_ViewList, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View list Project" },
                                new SystemPermission{ Name =  PermissionNames.Project_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Project" },
                                new SystemPermission{ Name =  PermissionNames.Project_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Project" },
                           }
                        },
                        new SystemPermission{ Name =  PermissionNames.WorkingExperience, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Working Experience",
                           Children = new List<SystemPermission>()
                           {
                                new SystemPermission{ Name =  PermissionNames.WorkingExperience_ViewAll, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View All Working Experience" },
                           }
                        },
                        new SystemPermission
                        {
                            Name =  PermissionNames.FakeSkill,
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            DisplayName = "Fake Skill" ,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name =  PermissionNames.FakeSkill_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Fake Skill" },
                                new SystemPermission{ Name =  PermissionNames.FakeSkill_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Fake Skill" },
                                new SystemPermission{ Name =  PermissionNames.FakeSkill_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Fake Skill" },
                                new SystemPermission{ Name =  PermissionNames.FakeSkill_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Fake Skill" },
                            }
                        },
                        new SystemPermission
                        {
                            Name =  PermissionNames.GroupSkill,
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            DisplayName = "Group Skill" ,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name =  PermissionNames.GroupSkill_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Group Skill" },
                                new SystemPermission{ Name =  PermissionNames.GroupSkill_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Group Skill" },
                                new SystemPermission{ Name =  PermissionNames.GroupSkill_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Group Skill" },
                                new SystemPermission{ Name =  PermissionNames.GroupSkill_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Group Skill" },
                            }
                        },
                        new SystemPermission
                        {
                            Name =  PermissionNames.EmployeePosition,
                            MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
                            DisplayName = "Employee Position" ,
                            Children = new List<SystemPermission>
                            {
                                new SystemPermission{ Name =  PermissionNames.EmployeePosition_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View Employee Position" },
                                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Employee Position" },
                                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Employee Position" },
                                new SystemPermission{ Name =  PermissionNames.EmployeePosition_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Employee Position" },
                            }
                        },
                    }
                },
                #endregion
            };
        }
    }
}
