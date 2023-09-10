using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.WebServices.InternalServices.LMS.Dtos
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string RelationInfo { get; set; }
        public string ImageCover { get; set; }
        public string FullPathImageCover { get; set; }
    }
}
