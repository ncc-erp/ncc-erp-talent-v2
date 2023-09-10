using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Utils;

namespace TalentV2.DomainServices.PositionSettings.Dtos
{
    public class PositionSettingDto
    {
        public long Id { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName { get => CommonUtils.GetEnumName(UserType); }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public string LMSCourseName { get; set; }
        public Guid? LMSCourseId { get; set; }
        public string ProjectInfo { get; set; }
        public string DiscordInfo { get; set; }
        public string IMSInfo { get; set; }
    }
}
