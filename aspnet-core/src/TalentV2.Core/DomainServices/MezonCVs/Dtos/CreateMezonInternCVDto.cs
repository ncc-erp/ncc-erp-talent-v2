using System;

namespace TalentV2.DomainServices.MezonCVs.Dtos
{
    public class CreateMezonInternCVDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long SubPositionId { get; set; }
        public long BranchId { get; set; }
        public long? CVSourceId { get; set; }
        public DateTime? Birthday { get; set; }
        public bool IsFemale { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string LinkCV { get; set; }
    }
}
