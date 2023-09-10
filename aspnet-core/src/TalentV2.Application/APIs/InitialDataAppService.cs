using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.Entities;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class InitialDataAppService : TalentV2AppServiceBase
    {
        private readonly ICategoryManager _categoryManager;
        public InitialDataAppService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpGet]
        public async Task<IActionResult> LoadCapabilityForRequestCV()
        {
            var capabilitySettings = _categoryManager.IQGetAllCapabilitySettingsGroupBy()
                .ToList();
            var requestCVs =WorkScope.GetAll<RequestCV>()
                .Where(q => q.Request.Status == StatusRequest.InProgress)
                .Where(q => !q.RequestCVCapabilityResults.Any(x => !x.IsDeleted))
                .Select(s => new { s.Id, s.Request.UserType, s.Request.SubPositionId })
                .AsEnumerable()
                .Select(x => new 
                {
                    RequestCVId = x.Id,
                    Capabilities = capabilitySettings
                                    .Where(ql => ql.UserType == x.UserType && ql.SubPositionId == x.SubPositionId)
                                    .Select(ql => ql.Capabilities)
                                    .FirstOrDefault() ?? new(),
                })
                .ToList();
            List<RequestCVCapabilityResult> results = new();
            foreach(var requestCV in requestCVs)
            {
                foreach(var capability in requestCV.Capabilities)
                {
                    results.Add(new RequestCVCapabilityResult
                    {
                        CapabilityId = capability.CapabilityId,
                        RequestCVId = requestCV.RequestCVId
                    });
                }
            }
            await AddRangeAsync(results);
            return new OkObjectResult("Successfully");
        }
    }
}
