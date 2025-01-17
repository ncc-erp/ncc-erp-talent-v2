using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Notifications.MezonWebhook.Dtos
{
    public class CrawlCVDto
    {
        public string InternCVQuantity {  get; set; }
        public string StaffCVQuantity {  get; set; }

    }

    public class CrawlCVResult : CrawlCVDto
    {
        public string InternLink { get; set; }
        public string StaffLink { get; set; }
        public string TagNames { get; set; }
        public string LinkOrTalent { get; set; }
    }
}
