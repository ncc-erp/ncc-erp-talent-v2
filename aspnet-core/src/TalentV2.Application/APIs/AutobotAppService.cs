using Abp.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TalentV2.DomainServices.CVAutomation.Dto;
using TalentV2.WebServices.ExternalServices.Autobot;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class AutobotAppService : TalentV2AppServiceBase
    {
        private readonly AutobotService _autobotService;

        public AutobotAppService(AutobotService autobotService)
        {
            _autobotService = autobotService;
        }

        [HttpPost]
        public async Task<CVExtractionData> GetCVExtractionData(IFormFile file)
        {
            return await _autobotService.ExtractCVInformationAsync<CVExtractionData>(file);
        }
    }
}
