using TalentV2.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class CapabilitySettingDto
    {
        public long Id { get; set; }
        public UserType? UserType { get; set; }
        public string UserTypeName { get =>
             this.UserType.HasValue ? Enum.GetName(typeof(UserType), UserType) : "";
        }
        public long? SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public long? CapabilityId { get; set; }
        public string CapabilityName { get; set; }
        public string GuideLine { get; set; }
        public string Note { get; set; }
        public int Factor { get; set; }
        public bool IsDeleted { get; set; }
        public bool FromType { get; set; }
    }
    public class CapabilityInCapabilitySettingDto
    {
        public long Id { get; set; }
        public long CapabilityId { get; set; }
        public string CapabilityName { get; set; }
        public string Note { get; set; }
        public string GuideLine { get; set; }
        public int Factor { get; set; }
        public DateTime CreationTime { get; set; }
        public bool FromType { get; set; }
    }
    public class GetPagingCapabilitySettingDto
    {
        public UserType UserType { get; set; }
        public string UserTypeName { get => Enum.GetName(typeof(UserType), UserType); }
        public long SubPositionId { get; set; }
        public string SubPositionName { get; set; }
        public List<CapabilityInCapabilitySettingDto> Capabilities { get; set; }
    }

    public class CapabilitySettingCloneDto
    {
        public UserType FromUserType { get; set; }
        public long FromSubPositionId { get; set; }
        public UserType ToUserType { get; set; }
        public long ToSubPositionId { get; set; }
    }

    public class ResponseCapabilitySettingCloneDto : CapabilitySettingCloneDto
    {
        public UserType ToUserType { get; set; }
        public string ToUserTypeName { get => Enum.GetName(typeof(UserType), ToUserType); }
        public string ToSubPositionName { get; set; }

        public UserType FromUserType { get; set; }
        public string FromUserTypeName { get => Enum.GetName(typeof(UserType), FromUserType); }
        public string FromSubPositionName { get; set; }
    }
}
