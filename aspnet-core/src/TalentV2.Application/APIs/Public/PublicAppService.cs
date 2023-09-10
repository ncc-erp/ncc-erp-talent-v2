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
using TalentV2.DomainServices.ExternalCVs;
using TalentV2.DomainServices.ExternalCVs.Dtos;
using TalentV2.Entities;
using TalentV2.DomainServices.Posts;
using NccCore.Paging;
using TalentV2.DomainServices.ApplyCVs.Dtos;
using TalentV2.DomainServices.ApplyCVs;

namespace TalentV2.APIs.Public
{
    public class PublicAppService : TalentV2AppServiceBase
    {
        protected IHttpContextAccessor _httpContextAccessor { get; set; }
        private readonly IExternalCVManager _externalCVManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IApplyCvManager _applyCvManager;


        public PublicAppService(IExternalCVManager externalCVManager, ICategoryManager categoryManager, IApplyCvManager applyCvManager)
        {
            _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
            _externalCVManager = externalCVManager;
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

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<CreateExternalCVResultDto> CreateExternalCV([FromForm] CreateExternalCVDto param)
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var result = new CreateExternalCVResultDto();
            var securityCodeHeader = header["X-Secret-Key"];

            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);

                return result;
            }

            result.IsConnected = true;
            result.Message = "Connected";

            var existExternalCV = await WorkScope.GetAll<ExternalCV>()
                .AnyAsync(e => e.ExternalId == param.ExternalId && e.CVSourceName == param.CVSourceName);

            if (existExternalCV)
                throw new UserFriendlyException($"Already exist ExternalCV with Id {param.ExternalId} from {param.CVSourceName}");

            if (!string.IsNullOrEmpty(param.Metadata) && !param.Metadata.IsValidJson())
                throw new UserFriendlyException($"{param.Metadata} is not a valid JSON");

            var cvId = await _externalCVManager.CreateExternalCV(param);
            result.ExternalCV = await _externalCVManager.GetExternalCVById(cvId);

            return result;
        }

        [AbpAllowAnonymous]
        [HttpPut]
        public async Task<UpdatePostResultDto> UpdatePostsMetadata([FromForm] UpdatePostsMetadataDto param)
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var result = new UpdatePostResultDto();
            var securityCodeHeader = header["X-Secret-Key"];

            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);

                return result;
            }

            result.IsConnected = true;
            result.Message = "Connected";

            if (!string.IsNullOrEmpty(param.Metadata) && !param.Metadata.IsValidJson())
                throw new UserFriendlyException($"{param.Metadata} is not a valid JSON");

            result.Post = await _categoryManager.UpdatePostsMetadata(param);

            return result;
        }


        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<PostResultDto> GetAllPost()
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var result = new PostResultDto();
            var securityCodeHeader = header["X-Secret-Key"];

            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);

                return result;
            }

            result.IsConnected = true;
            result.Message = "Connected";

            result.Posts = await _categoryManager.IQGetAllPosts().ToListAsync();
            return result;
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<PostGridResultDto> GetPostPaging(GridParam param)
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var result = new PostGridResultDto();
            var securityCodeHeader = header["X-Secret-Key"];

            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);

                return result;
            }

            result.IsConnected = true;
            result.Message = "Connected";

            var query = _categoryManager.IQGetAllPosts();
            result.Result = await query.GetGridResult(query, param);
            return result;
        }

        [AbpAllowAnonymous]
        [HttpPut]
        public async Task<CreateExternalCVResultDto> UpdateExternalCV([FromForm] UpdateExternalCVDto input)
        {
            var secretCode = SettingManager.GetSettingValue(AppSettingNames.TalentSecurityCode);
            var header = _httpContextAccessor.HttpContext.Request.Headers;

            var result = new CreateExternalCVResultDto();
            var securityCodeHeader = header["X-Secret-Key"];

            if (!IsCheckSecurityCodeCorrectForProject())
            {
                result.IsConnected = false;
                result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);

                return result;
            }

            result.IsConnected = true;
            result.Message = "Connected";

            var existExternalCV = await WorkScope.GetAll<ExternalCV>()
                .AnyAsync(e => e.ExternalId == input.ExternalId && e.CVSourceName == input.CVSourceName);

            if (!existExternalCV)
                throw new UserFriendlyException($"Does not exist ExternalCV with Id {input.ExternalId} from {input.CVSourceName}");

            if (!string.IsNullOrEmpty(input.Metadata) && !input.Metadata.IsValidJson())
                throw new UserFriendlyException($"{input.Metadata} is not a valid JSON");

            result.ExternalCV = await _externalCVManager.UpdateExternalCV(input);

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
