using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.InternalTools.Dtos
{
    public class ChangeTalentUserStatusDto
    {
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateUserStatusFromHRMv2Dto
    {
        public string EmailAddress { get; set; }
        public DateTime DateAt { get; set; }
    }

}
