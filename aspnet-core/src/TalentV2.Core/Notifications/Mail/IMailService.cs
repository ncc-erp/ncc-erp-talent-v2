using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Notifications.Mail.Dtos;

namespace TalentV2.Notifications.Mail
{
    public interface IMailService : ITransientDependency
    {
        Task<MailPreviewInfoDto> GetContentMailRequestCV(long requestCVId);
        Task<MailPreviewInfoDto> GetContentMailRequestCV(long requestCVId, string mailVersion);
        Task<MailPreviewInfoDto> GetContentMailCV(long cvId);
        Task<bool> IsSentMailCV(long cvId, MailFuncEnum mailFuncType);
        Task<List<MailStatusHistoryDto>> GetMailStatusHistoryByCVId(long cvId);
        Task<MailPreviewInfoDto> PreviewContentMail(long id, MailFuncEnum mailFuncType);
        Task<MailPreviewInfoDto> PreviewContentMail(long id, MailFuncEnum emailType, string mailVersion);
        void Send(MailPreviewInfoDto content);
        Task<List<MailDto>> GetAllMailTemplate();
        Task<MailPreviewInfoDto> GetById(long id);
        Task<MailPreviewInfoDto> GetByIdFakeData(long id);
        IQueryable<MailDto> IQGetEmailTemplate();
        Task CreateEmailHistory(long cvId, long emailTemplateId, string description = "");
    }
}
