using Abp.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.NccCore;
using TalentV2.Notifications.Komu.Dtos;
using TalentV2.Notifications.Templates;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.WebServices.ExternalServices.Komu;

namespace TalentV2.Notifications.Komu
{
    public class KomuNotification : IKomuNotification
    {
        private readonly IWorkScope _ws;
        private readonly KomuService _komuService;
        private readonly ISettingManager _settingManager;
        private readonly ILogger<KomuNotification> _logger;
        public KomuNotification(IWorkScope ws, KomuService komuService, ISettingManager settingManager, ILogger<KomuNotification> logger)
        {
            _ws = ws;
            _komuService = komuService;
            _settingManager = settingManager;
            _logger = logger;   
        }
        public void NotifyAcceptedOrRejectedOffer(RequestCVStatus status, long requestCvId, bool isFirstAcceptedOffer = true)
        {
            var dataTemplate = _ws.GetAll<RequestCV>()
                    .Where(q => q.Id == requestCvId)
                    .Select(s => new CandidateOfferAcceptedTemplate
                    {
                        CVId = s.CVId,
                        FullName = s.CV.Name,
                        BranchName = s.CV.Branch.DisplayName,
                        OnboardDateTime = s.OnboardDate,
                        Skills = string.Join(",", s.CV.CVSkills.Select(s => s.Skill.Name).ToList()),
                        Email = s.CV.Email,
                        NCCEmail = s.CV.NCCEmail,
                        UserType = s.CV.UserType,
                        Phone = s.CV.Phone,
                    }).FirstOrDefault();

            var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuHRITChannelId);

            var message = status == RequestCVStatus.RejectedOffer ? TemplateHelper.RejectedOfferTemplate(dataTemplate) : TemplateHelper.AcceptedOfferTemplate(dataTemplate, isFirstAcceptedOffer);

            _komuService.NotifyToChannel(message, channelId);
        }

        public async Task NotifyUpdatedPersonalInfoTemplate(long requestCvId)
        {
            var dataTemplate = await _ws.GetAll<RequestCV>()
                    .Where(q => q.Id == requestCvId)
                    .Select(s => new CandidateOfferAcceptedTemplate
                    {
                        CVId = s.CVId,
                        FullName = s.CV.Name,
                        BranchName = s.CV.Branch.DisplayName,
                        OnboardDateTime = s.OnboardDate,
                        Skills = string.Join(",", s.CV.CVSkills.Select(s => s.Skill.Name).ToList()),
                        Email = s.CV.Email,
                        NCCEmail = s.CV.NCCEmail,
                        UserType = s.CV.UserType,
                        Phone = s.CV.Phone,
                    }).FirstOrDefaultAsync();

            var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuHRITChannelId);

            var message = TemplateHelper.UpdatedPersonalInfoTemplate(dataTemplate);

            _komuService.NotifyToChannel(message, channelId);
        }

        public async Task NotifyRequestFromProject(NotificationRequestFromProject input)
        {
            var branchName = await _ws.GetAll<Branch>().Where(s => s.Id == input.BranchId).Select(s => s.Name).FirstOrDefaultAsync();
            var subPosisitonName = await _ws.GetAll<SubPosition>().Where(s => s.Id == input.SubPositionId).Select(s => s.Name).FirstOrDefaultAsync();

            await SendNotifyRequest(new MessageNotificationRequestFromProject
            {
                BranchName = branchName,    
                UserType = input.UserType,
                Note = input.Note,
                RequestId = input.RequestId,
                SubPositionName = subPosisitonName,
                URL = TalentConstants.BaseFEAddress + input.URI,
                Level = input.Level,
            });
        }
        private async Task SendNotifyRequest(MessageNotificationRequestFromProject input)
        {
            if (input.UserType == UserType.Intern)
            {
                var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuResourceRequestInternChannelId);
                var message = TemplateHelper.RequestInternFromProject(input);
                _komuService.NotifyToChannel(message, channelId);
            }
            else if(input.UserType == UserType.Staff)
            {
                var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuResourceRequestStaffChannelId);
                var message = TemplateHelper.RequestStaffFromProject(input);
                _komuService.NotifyToChannel(message, channelId);
            }
            else
            {
                _logger.LogError($"User Type: {input.UserType.ToString()} Not Implemented");
            }
        }
    }
}
