using NccCore.Anotations;
using System;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ApplyCVs.Dtos
{
    public class ApplyCVDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        public string FullName { get; set; }
        public bool IsFemale { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PositionType { get; set; }
        [ApplySearchAttribute]
        public string JobTitle { get; set; }
        public string Branch { get; set; }
        public string Avatar { get; set; }
        public string AvatarLink { get => CommonUtils.FullFilePath(Avatar); }
        public string AttachCV { get; set; }
        public string AttachCVLink { get => CommonUtils.FullFilePath(AttachCV); }
        public long PostId { get; set; }
        public long BranchId { get; set; }
        public DateTime? ApplyCVDate { get; set; }
        public bool? Applied { get; set; }

    }
}
