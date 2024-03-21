using Abp.Authorization;
using Abp.Dependency;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.Dto;
using TalentV2.Entities;
using NccCore.Paging;
using TalentV2.DomainServices.ApplyCVs.Dtos;
using TalentV2.DomainServices.ApplyCVs;

namespace TalentV2.APIs.Public
{
    public class PublicAppService : TalentV2AppServiceBase
    {
        protected IHttpContextAccessor _httpContextAccessor { get; set; }
        private readonly ICategoryManager _categoryManager;
        private readonly IApplyCvManager _applyCvManager;


        public PublicAppService( ICategoryManager categoryManager, IApplyCvManager applyCvManager)
        {
            _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
            _categoryManager = categoryManager;
            _applyCvManager = applyCvManager;
        }
        [AbpAllowAnonymous]
        [HttpGet]
        //[NccAuth]
        public GetResultConnectDto CheckConnect()
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var securityCodeHeader = header["X-Secret-Key"];
            var result = new GetResultConnectDto();
            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);
                return result;
            }
            result.IsConnected = true;
            result.Message = "Connected";
            return result;
        }

        protected bool IsCheckSecurityCodeCorrectForProject()
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var securityCodeHeader = header["X-Secret-Key"];
            if (string.IsNullOrEmpty(securityCodeHeader))
            {
                securityCodeHeader = header["securityCode"];
            }

            return secretCode == securityCodeHeader;
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<ApplyCVDto> CreateApplyCV([FromForm] CreateApplyCVDto createApplyCVDto)
        {
            var applyCVId = await _applyCvManager.Create(createApplyCVDto);
            return await _applyCvManager.GetApplyCVById(applyCVId);
        }
    }
}
