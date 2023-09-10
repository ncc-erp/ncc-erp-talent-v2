using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Entities
{
	public class Position : FullAuditedEntity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		[MaxLength(50)]
		public string Code { get; set; }
		[MaxLength(20)]
		public string ColorCode { get; set; }

		public ICollection<SubPosition> SubPositions { get; set; }
	}
}
