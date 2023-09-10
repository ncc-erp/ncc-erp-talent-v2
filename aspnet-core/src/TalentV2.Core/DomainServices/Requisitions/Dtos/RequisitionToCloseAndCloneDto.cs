using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.DomainServices.Requisitions.Dtos
{
    [AutoMapTo(typeof(CreateRequisitionInternDto))]
    public class RequisitionToCloseAndCloneDto : FormRequisitionDto
    {
        public long Id { get; set; }

        public List<CandidateRequisitionDto> CandidateRequisitions { get; set; }
    }

    public class CandidateRequisitionDto
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }

        public string LinkCV { get; set; }

        public bool IsFemale { get; set; }

        public bool IsClone => !CommonUtils.ListStatusNotAvailableClone
                                            .Select(c => c.Id)
                                            .Any(id => Status.GetHashCode() == id)
                            && !CommonUtils.ListCVStatusNotAvailableClone
                                            .Select(x => x.Id)
                                            .Any(id => CVStatus.GetHashCode() == id);

        public RequestCVStatus Status { get; set; }

        public string StatusName { get => CommonUtils.GetEnumName(Status); }

        public string CvStatusName { get; set; }

        public CVStatus CVStatus { get; set; }
    }

    public class CloneRequisitionDto : CreateRequisitionInternDto
    {
        public long Id { get; set; }

        public List<long> CVIds { get; set; }
    }
}