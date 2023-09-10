using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Utils;

namespace TalentV2.Roles.Dto
{
    public class UserRoleInfoDto
    {
        public long UserRoleId { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string AvatarPath { get; set; }
        public string LinkAvatar => CommonUtils.FullFilePath(AvatarPath);
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BranchName { get; set; }
    }
}
