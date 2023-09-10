using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.ModelExtends
{
    public class CreateUpdateAudit
    {
        public long? CreatorUserId { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string LastModifiedName { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationTime { get; set; }
        public string UpdatedName
        {
            get => LastModifiedName  != "" ? LastModifiedName : CreatorName;
        }
        public DateTime? UpdatedTime
        {
            get => LastModifiedTime.HasValue ? LastModifiedTime : CreationTime;
        }
    }
}
