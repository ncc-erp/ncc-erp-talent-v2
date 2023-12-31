﻿using Abp.Authorization;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Authorization;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.RequestCVs;
using TalentV2.DomainServices.RequestCVs.Dtos;
using TalentV2.Entities;
using TalentV2.Notifications.Mail;
using TalentV2.Notifications.Mail.Dtos;

namespace TalentV2.APIs
{
    [AbpAuthorize]
    public class MailAppService : TalentV2AppServiceBase
    {
        private readonly IRequestCVManager _requestCVManager;
        private readonly IMailService _mailService;
        public MailAppService(
            IRequestCVManager requestCVManager,
            IMailService mailService
        ) 
        {
            _requestCVManager = requestCVManager;
            _mailService = mailService;
        }
        [HttpGet]
        public async Task<MailPreviewInfoDto> GetByIdFakeData(long id)
        {
            return await _mailService.GetByIdFakeData(id);
        }
        [HttpGet]
        public async Task<MailPreviewInfoDto> PreviewBeforeSend(long id, MailFuncEnum emailType)
        {
            return await _mailService.PreviewContentMail(id, emailType);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Mails_SendMail)]
        public async Task<string> SendMail(MailPreviewInfoDto message)
        {
            _mailService.Send(message);
            return "Sent Successfully";
        }
        [HttpGet]
        public async Task<MailPreviewInfoDto> Get(long id)
        {
            return await _mailService.GetById(id);
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Pages_Mails_ViewList)]
        public async Task<List<MailDto>> GetAll()
        {
            return await _mailService.GetAllMailTemplate();
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Pages_Mails_Edit)]
        public async Task<MailDto> Update(MailPreviewInfoDto input)
        {
            var mail = await WorkScope.GetAsync<EmailTemplate>(input.TemplateId);
            mail.BodyMessage = input.BodyMessage;
            mail.Subject = input.Subject;

            var ccs = string.Join(",", input.CCs);
            mail.CCs = ccs;

            await CurrentUnitOfWork.SaveChangesAsync();
            return await _mailService.IQGetEmailTemplate()
                .FirstOrDefaultAsync(s => s.Id == input.TemplateId);
        }
    }
}
