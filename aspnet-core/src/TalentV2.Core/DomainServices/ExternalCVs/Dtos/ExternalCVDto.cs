using NccCore.Anotations;
using System;
using TalentV2.DomainServices.Dto;
using TalentV2.ModelExtends;
using TalentV2.Utils;

namespace TalentV2.DomainServices.ExternalCVs.Dtos
{
    public class CreateExternalCVResultDto : GetResultConnectDto
    {
        public ExternalCVDto ExternalCV { get; set; }
    }

    public class ExternalCVDto : CreateUpdateAudit
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ReferenceName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
        public string AvatarUrl { get => CommonUtils.FullFilePath(Avatar); }
        public string UserTypeName { get; set; }
        public bool IsFemale { get; set; }
        public string LinkCV { get; set; }
        public string LinkCVUrl { get => CommonUtils.FullFilePath(LinkCV); }
        public string NCCEmail { get; set; }
        public string Note { get; set; }
        public string CVSourceName { get; set; }
        public string BranchName { get; set; }
        public string PositionName { get; set; }
        public string Metadata { get; set; }
    }
}
