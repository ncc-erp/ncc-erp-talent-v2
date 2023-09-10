using System;
using System.Collections.Generic;
using System.Text;

namespace TalentV2.APIs.NccCVs.Project.Dto
{
    public class ReportOfProject
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long WorkingExpId { get; set; }
    }
}
