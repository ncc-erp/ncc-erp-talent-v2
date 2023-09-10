using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class Education : NccAuditEntity, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }

		public long EducationTypeId { get; set; }
		[ForeignKey(nameof(EducationTypeId))]
		public EducationType EducationType { get;set;}
		[MaxLength(20)]
        public string ColorCode { get; set; }

        public ICollection<CVEducation> CVEducations { get; set; }
}
}
