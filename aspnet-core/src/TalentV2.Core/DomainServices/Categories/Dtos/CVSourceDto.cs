using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Categories.Dtos
{
    [AutoMapTo(typeof(CVSource))]
    public class CVSourceDto : CategoryDto
    {
        public CVSourceReferenceType? ReferenceType { get; set; }
        public string ReferenceTypeName
        {
            get
            {
                if(!ReferenceType.HasValue) return string.Empty;
                return CommonUtils.GetEnumName(ReferenceType.Value);
            }
        }
        [MaxLength(20)]
        public string ColorCode { get; set; }
    }
}
