using Abp.Dependency;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.ModelExtends;
using TalentV2.NccCore;
using TalentV2.Notifications.Templates;
using TalentV2.Utils;

namespace TalentV2.DomainServices.RequestCVs.Dtos
{
    public class AllInformationRequestCVDto : IHRSignatureInfo
    {
        public long Id { get; set; }
        public long CVId { get; set; }
        public RequestCVStatus Status { get; set; }
        public string StatusName { get => CommonUtils.GetEnumName(Status); }
        public DateTime? InterviewTimeDate { get; set; }
        public string InterviewTime
        {
            get => InterviewTimeDate.HasValue ? DateTimeUtils.GetTime(InterviewTimeDate) : string.Empty;
        }
        public string InterviewDay { get => InterviewTimeDate.HasValue ? DateTimeUtils.GetDay(InterviewTimeDate).DayOfWeekNameVN : string.Empty; }
        public string InterviewDate { get => DateTimeUtils.ToStringddMMyyyy(InterviewTimeDate); }
        public Level? ApplyLevel { get; set; }
        public string ApplyLevelName
        {
            get
            {
                if (ApplyLevel.HasValue)
                    return CommonUtils.GetEnumName(ApplyLevel.Value);
                return string.Empty;
            }
        }
        public Level? InterviewLevel { get; set; }
        public string InterviewLevelName
        {
            get
            {
                if (InterviewLevel.HasValue)
                    return CommonUtils.GetEnumName(InterviewLevel.Value);
                return string.Empty;
            }
        }
        public Level? FinalLevel { get; set; }
        public string FinalLevelName
        {
            get
            {
                if (FinalLevel.HasValue)
                    return CommonUtils.GetEnumName(FinalLevel.Value);
                return string.Empty;
            }
        }
        public string HRNote { get; set; }
        public DateTime? OnboardDateTime { get; set; }
        public string OnboardDay { get => OnboardDateTime.HasValue ? DateTimeUtils.GetDay(OnboardDateTime).DayOfWeekNameVN : string.Empty; }
        public string OnboardDate { get => DateTimeUtils.ToStringddMMyyyy(OnboardDateTime); }
        public float Salary { get; set; }
        public string LaunchAllowance => string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:#,##0}", TalentConstants.LAUNCH_ALLOWANCE);
        public string SalaryToString => string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:#,##0}", Salary);
        public string TotalSalary => string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:#,##0}", (TalentConstants.LAUNCH_ALLOWANCE + Salary));
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? BirthdayDateTime { get; set; }
        public string Birthday  => DateTimeUtils.ToString(BirthdayDateTime) ?? string.Empty;
        public string Avatar { get; set; }
        public string JobPositionRecruit { get; set; }
        public string CVBranchName { get; set; }
        public string CVBranchAddress { get; set; }
        public string RequestBranchName { get; set; }
        public string RequestBranchAddress { get; set; }
        public string LMSInfo { get; set; }
        public string HRPhone { get; set; }
        public string HRName { get; set; }
        public string HREmail { get; set; }
        public string SignatureContact { get; set; }
        public string Percentage { get; set; }
    }
}
