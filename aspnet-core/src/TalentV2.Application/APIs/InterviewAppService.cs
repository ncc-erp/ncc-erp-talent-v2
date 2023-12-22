using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Interview.Dtos;
using TalentV2.DomainServices.Interviews;
using TalentV2.DomainServices.Interviews.Dtos;
using TalentV2.DomainServices.Requisitions;
using TalentV2.DomainServices.Requisitions.Dtos;
using TalentV2.Entities;
using TalentV2.ModelExtends;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class InterviewAppService : TalentV2AppServiceBase
    {
        private readonly IInterviewManager _interviewManager;
        private readonly IRequisitionManager _requisitionManager;
        public InterviewAppService(
            IInterviewManager interviewManager,
            IRequisitionManager requisitionManager 
        ) 
        {
            _interviewManager = interviewManager;
            _requisitionManager = requisitionManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Interviews_ViewList,PermissionNames.Pages_Interviews_ViewOnlyMe)]
        public async Task<GridResult<CVRequisitionDto>> GetAllPaging(InterviewFilterPagingDto param)
        {
            var query = _interviewManager
                     .IQCVsByInterviewer()
                     .WhereIf(param.InterviewerIds != null, q => q.Interviews.Any(s => param.InterviewerIds.Contains(s.InterviewerId)));

            //TODO:: view only interviewer
            bool isViewOnlyMe = PermissionChecker.IsGranted(PermissionNames.Pages_Interviews_ViewOnlyMe);
            bool isViewAll= PermissionChecker.IsGranted(PermissionNames.Pages_Interviews_ViewList);

            var filterStatus = param.FilterItems.FirstOrDefault(s => s.PropertyName == "requestCVStatus");
            if (filterStatus != null && filterStatus.Value != null)
            {
                if (Enum.TryParse<RequestCVStatus>(filterStatus.Value.ToString(), out var status))
                {
                    if (status != RequestCVStatus.FailedInterview &&  status != RequestCVStatus.PassedInterview)
                    {
                        query = query.Where(s => s.RequestStatus == StatusRequest.InProgress);
                    }
                }
            }

            if(isViewOnlyMe && !isViewAll)
                query = query.Where(q => q.Interviews.Any(s => s.InterviewerId == AbpSession.UserId));
            return await query.GetGridResult(query, param);
        }
        //[HttpDelete]
        //[AbpAuthorize(PermissionNames.Pages_Interviews_Delete)]
        //public async Task<string> DeleteRequestCV(long requestCvId)
        //{
        //    await _requisitionManager.DeleteRequestCV(requestCvId);
        //    return "Deleted Successfully";
        //}
        [HttpGet]
        public async Task<List<DropdownUserInfoDto>> GetAllInterview()
        {
            return await WorkScope.GetAll<RequestCVInterview>()
                .Select(s => new DropdownUserInfoDto
                {
                    Id = s.InterviewId,
                    Name = s.Interview.Name,
                    Surname = s.Interview.Surname,
                    Email = s.Interview.EmailAddress,
                    UserName = s.Interview.UserName,
                }).Distinct().ToListAsync();
        }
    }
}
