using System;
using TalentV2.Utils;

namespace TalentV2.DomainServices.MezonCVs.Dtos
{
    public class MezonInternCVDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public long? CVSourceId { get; set; }
        public string CVSourceName { get; set; }
        public DateTime? Birthday { get; set; }
        public bool IsFemale { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string LinkCV { get; set; }
        public string LinkCVUrl { get => CommonUtils.FullFilePath(LinkCV); }
    }
}
