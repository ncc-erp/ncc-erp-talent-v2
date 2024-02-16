using Abp.Authorization;
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
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
using static TalentV2.DomainServices.Categories.Dtos.CandidateLanguageDto;

namespace TalentV2.APIs
{
    public class CandidateLanguageAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public CandidateLanguageAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpPost]
        public async Task<GridResult<CandidateLanguageDto>> GetAllPaging(GridParam param)
        {
            var query = _categoryManager
                .IQGetAllLanguage();
            return await query.GetGridResult(query, param);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_CreateLanguage_ViewDetail_Languages_Create)]
        public async Task<CandidateLanguageDto> Create(CreateLanguageDto input)
        {
            return await _categoryManager.CreateLanguage(input);
        }
        [HttpPut]

        public async Task<CandidateLanguageDto> Update(UpdateLanguageDto input)
        {
            return await _categoryManager.UpdateLanguage(input);
        }

        [HttpDelete]
        public async Task<string> Delete(long Id)
        {
            await _categoryManager.DeleteLanguage(Id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<List<CandidateLanguageDto>> GetAll()
        {
            return await _categoryManager
                .IQGetAllLanguage()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
