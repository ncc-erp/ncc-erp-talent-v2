using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.DomainServices.Candidates.Dtos;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Users.Dtos;
using TalentV2.Entities;
using TalentV2.Utils;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class CommonAppService : TalentV2AppServiceBase
    {
        public CommonAppService(){}
        [HttpGet]
        public List<CategoryDto> GetRequestStatus()
        {
            return CommonUtils.ListRequestStatus;
        }
        [HttpGet]
        public List<CategoryDto> GetRequestCVStatus()
        {
            return CommonUtils.ListRequestCVStatus;
        }
        [HttpGet]
        public List<CategoryDto> GetPriority()
        {
            return CommonUtils.ListPriority;
        }
        [HttpGet]
        public List<LevelDto> GetLevel()
        {
            return CommonUtils.ListLevel;
        }
        [HttpGet]
        public List<CategoryDto> GetStatusCandidateOffer()
        {
            return CommonUtils.ListStatusCandidateOffer;
        }
        [HttpGet]
        public List<CategoryDto> GetStatusCandidateOnboard()
        {
            return CommonUtils.ListStatusCandidateOnboard;
        }
        [HttpGet] 
        public List<CategoryDto> GetStatusRequest()
        {
            return CommonUtils.ListRequestStatus;
        }
        [HttpGet]
        public async Task<List<UserReferenceDto>> GetAllUser()
        {
            return await WorkScope.GetAll<User>()
                .Select(s => new UserReferenceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.EmailAddress,
                    FullName = s.FullName,
                    SurName = s.Surname,
                    UserName = s.UserName,
                }).ToListAsync();
        }
        [HttpGet] 
        public async Task<List<DropdownPositionDto>> GetDropdownPositions()
        {
            return await WorkScope.GetAll<SubPosition>()
                .GroupBy(s => new { s.PositionId , s.Position.Name})
                .Select(gr => new DropdownPositionDto
                {
                    Id = gr.Key.PositionId,
                    Position = gr.Key.Name,
                    Items = gr.Select(s => new DropdownSubPositionDto
                    { 
                        Id = s.Id,
                        SubPosition = s.Name
                    }).ToList()
                })
                .OrderBy(s => s.Position)
                .ToListAsync();
        }
        [HttpGet]
        public List<CategoryDto> GetRequestLevel()
        {
            return CommonUtils.ListRequestLevel;
        }
        [HttpGet]
        public List<InternSalaryDto> GetInternSalary()
        {
            return CommonUtils.ListSalaryIntern;
        }
        [HttpGet]
        public List<LevelDto> GetLevelStaff()
        {
            return CommonUtils.ListLevelStaff;
        }
        [HttpGet]
        public List<CategoryDto> GetListCVStatus()
        {
            return CommonUtils.ListCVStatus;
        }
        [HttpGet]
        public List<CategoryDto> GetListCVSourceReferenceType()
        {
            return CommonUtils.ListCVSourceReferenceType;
        }
        [HttpGet]
        public List<CategoryDto> GetListInterviewStatus()
        {
            return CommonUtils.ListInterviewStatus;
        }
        [HttpGet]
        public List<LevelDto> GetLevelInterviewStaff()
        {
            return CommonUtils.ListLevelInterviewStaff;
        }
        [HttpGet]
        public List<LevelDto> GetLevelFinalStaff()
        {
            return CommonUtils.ListLevelFinalStaff;
        }
        [HttpGet]
        public List<InternSalaryDto> GetLevelFinalIntern()
        {
            return CommonUtils.ListLevelFinalIntern;
        }
    }
}
