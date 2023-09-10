using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TalentV2.DomainServices.Dto;
using TalentV2.NccCore;
using TalentV2.WebServices.InternalServices.LMS.Dtos;

namespace TalentV2.WebServices.InternalServices.LMS
{
    public class LMSService : BaseWebService
    {
        private const string baseUrl = "api/services/app/Talent";

        public LMSService(
            HttpClient httpClient,
            ILogger<LMSService> logger,
            IAbpSession abpSession)
        : base(
            httpClient,
            logger,
            abpSession)
        {
        }

        public async Task<StudentDto> CreateAccountStudent(StudentDto student)
        {
            var response = await PostAsync<AbpResponseResult<StudentDto>>(baseUrl + "/CreateAccountStudent", student);
            return response?.Result;
        }

        public async Task<List<CourseDto>> GetListCourse()
        {
            var response = await GetAsync<AbpResponseResult<List<CourseDto>>>(baseUrl + "/GetListCourse");
            return response?.Result;
        }
        public  GetResultConnectDto CheckConnectToLMS()
        {
            var res =  GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to LMS"
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