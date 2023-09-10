using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.Notifications.Templates.Dtos
{
    public class CandidateOfferTemplate
    {
        public long CVId { get; set; }
        public string FullName { get; set; }
        public string BranchName { get; set; }
        public string SubPositionName { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName => CommonUtils.GetEnumName(UserType);
        public string Skills { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string NCCEmail { get; set; }
    }
    public class CandidateOfferAcceptedTemplate : CandidateOfferTemplate
    {
        public DateTime? OnboardDateTime { get; set; }
        public string OnboardDate { get => DateTimeUtils.ToStringddMMyyyy(OnboardDateTime); }
    }
    public class CandidateOfferRejectedTemplate : CandidateOfferTemplate { }
}
