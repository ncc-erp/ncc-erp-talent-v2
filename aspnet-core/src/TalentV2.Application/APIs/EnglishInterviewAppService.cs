using Abp.Authorization;
using Abp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.AIInterviews;
using TalentV2.DomainServices.AIInterviews.Dtos;
using TalentV2.Entities;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class EnglishInterviewAppService : TalentV2AppServiceBase
    {
        private readonly IAIInterviewManager _aiInterviewManager;

        public EnglishInterviewAppService(IAIInterviewManager aiInterviewManager)
        {
            _aiInterviewManager = aiInterviewManager;
        }

        [HttpGet]
        public async Task<List<EnglishInterviewResultDto>> GetByCVId(long cvId)
        {
            var userType = await WorkScope.GetAll<CV>()
                .Where(x => x.Id == cvId)
                .Select(x => (UserType?)x.UserType)
                .FirstOrDefaultAsync();
            if (!userType.HasValue)
                throw new EntityNotFoundException(typeof(CV), cvId);

            await PermissionChecker.AuthorizeAsync(userType == UserType.Intern
                ? PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View
                : PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View);

            return await _aiInterviewManager.GetResultsByCVId(cvId);
        }
    }
}
