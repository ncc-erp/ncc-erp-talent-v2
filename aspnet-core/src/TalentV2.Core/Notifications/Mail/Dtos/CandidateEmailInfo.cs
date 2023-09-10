using Abp.Dependency;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.ModelExtends;
using TalentV2.NccCore;
using TalentV2.Notifications.Templates;

namespace TalentV2.Notifications.Mail.Dtos
{
    public class CandidateEmailInfo : UserInfo, IHRSignatureInfo
    {
        public long CVId { get; set; }
        public string CVBranchAddress { get; set; }
        public string HRPhone { get; set; }
        public string HRName { get; set; }
        public string HREmail { get; set; }
        public string SignatureContact { get; set; }
    }
}
