using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    public class CreateFromRequestProjectDto
    {
        public long ResourceRequestId { get; set; }
        public long SubPositionId { get; set; }
        public long BranchId { get; set; }
        public int Quantity { get; set; }
        public List<string> SkillNames { get; set; }
        public Level Level { get; set; }
        public DateTime TimeNeed { get; set; }
        public Priority Priority { get; set; }
        public string Note { get; set; }
    }
}
