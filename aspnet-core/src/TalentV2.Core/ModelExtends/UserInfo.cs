using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.ModelExtends
{
    public class UserInfo : CreateUpdateAudit
    {
        [ApplySearchAttribute]
        public string FullName { get; set; }
        [ApplySearchAttribute]
        public string Phone { get; set; }
        [ApplySearchAttribute]
        public string Email { get; set; }
        public string Avatar { get => CommonUtils.FullFilePath(PathAvatar); }
        public string PathAvatar { get; set; }
        public string PathLinkCV { get; set; }
        public string LinkCV { get => CommonUtils.FullFilePath(PathLinkCV); }
        public bool IsFemale { get; set; }
        public CVStatus CvStatus { get; set; }
        public string CvStatusName { get => CommonUtils.GetEnumName(CvStatus); }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName { get => CommonUtils.GetEnumName(UserType); }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public string DisplayBranchName { get; set; }
        public string Note { get; set; }
        public bool? isClone { get; set; }
    }
}
