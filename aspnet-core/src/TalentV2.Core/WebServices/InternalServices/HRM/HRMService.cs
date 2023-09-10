using Abp.Runtime.Session;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Dto;
using TalentV2.WebServices.InternalServices.HRM.Dtos;

namespace TalentV2.WebServices.InternalServices.HRM
{
    public class HRMService : BaseWebService
    {
        private const string baseUrl = "api/services/app/Talent";

        public HRMService(
            HttpClient httpClient,
            ILogger<HRMService> logger,
            IAbpSession abpSession)
        : base(
            httpClient,
            logger,
            abpSession)
        {
        }

        public void UpdateHRMTempEmployee(UpdateHRMTempEmployeeDto input)
        {
              Post(baseUrl + "/CreateTempEmployeeFromTalent", input);
        }
        public  GetResultConnectDto CheckConnectToHRM()
        {
            var res =  GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to HRM"
                };
            }
            if(res.Error != null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = res.Error.Message
                };
            }
            return res.Result;
        }
    }
}
