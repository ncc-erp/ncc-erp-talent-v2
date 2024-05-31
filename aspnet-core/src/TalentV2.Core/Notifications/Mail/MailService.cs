using Abp.Net.Mail;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.RequestCVs.Dtos;
using TalentV2.Entities;
using TalentV2.NccCore;
using TalentV2.Notifications.Mail.Dtos;
using TalentV2.Notifications.Templates;
using TalentV2.Utils;

namespace TalentV2.Notifications.Mail
{
    public class MailService : IMailService
    {
        private readonly IWorkScope _workScope;
        private readonly IEmailSender _emailSender;
        private readonly IAbpSession _abpSession;

        public MailService(
            IEmailSender emailSender,
            IWorkScope workScope,
            IAbpSession abpSession
        )
        {
            _workScope = workScope;
            _emailSender = emailSender;
            _abpSession = abpSession;
        }

        public async Task<MailPreviewInfoDto> GetContentMailCV(long cvId)
        {
            return await PreviewContentMail(cvId, MailFuncEnum.FailedCV);
        }

        public async Task<MailPreviewInfoDto> GetContentMailRequestCV(long requestCVId)
        {
            var requestCV = await _workScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCVId)
                .Select(s => new { s.Status, s.Request.UserType })
                .FirstOrDefaultAsync();

            var mailFuncType = CommonUtils.MapRequestCVStatusToEmailType(requestCV.Status, requestCV.UserType);

            return await PreviewContentMail(requestCVId, mailFuncType);
        }

        public async Task<bool> IsSentMailCV(long cvId, MailFuncEnum mailFuncType)
        {
            return await _workScope.GetAll<EmailStatusHistory>()
                .Where(s => s.CVId == cvId && s.EmailTemplate.Type == mailFuncType)
                .AnyAsync();
        }

        public async Task<MailPreviewInfoDto> PreviewContentMail(long id, MailFuncEnum emailType)
        {
            var email = await _workScope.GetAll<EmailTemplate>()
                .Where(q => q.Type == emailType)
                .FirstOrDefaultAsync();

            var data = await EmailDispatchData(emailType, id);
            var message = GetContentMail(data.Result, email);

            if (!string.IsNullOrEmpty(email.CCs))
                message.CCs = email.CCs.Split(',').ToList();

            return new MailPreviewInfoDto
            {
                BodyMessage = message.BodyMessage,
                Subject = email.Subject,
                PropertiesSupport = message.PropertiesSupport,
                TemplateId = email.Id,
                To = message.To,
                CCs = message.CCs
            };
        }

        public async Task<List<MailStatusHistoryDto>> GetMailStatusHistoryByCVId(long cvId)
        {
            return await _workScope.GetAll<EmailStatusHistory>()
                 .Where(q => q.CVId == cvId)
                 .Select(s => new MailStatusHistoryDto
                 {
                     CVId = s.CVId,
                     CreationTime = s.CreationTime,
                     Description = s.Description,
                     Id = s.Id,
                     MailFuncType = s.EmailTemplate.Type,
                     Subject = s.EmailTemplate.Subject
                 })
                 .OrderByDescending(q => q.CreationTime)
                 .ToListAsync();
        }

        public void Send(MailPreviewInfoDto message)
        {
            if (message.CCs.Any())
            {
                SendToCC(message);
            }
            else
            {
                SendDefault(message);
            }
        }

        public async Task SendAsync(MailPreviewInfoDto message)
        {
            var mailMessage = new MailMessage()
            {
                Subject = message.Subject,
                Body = message.BodyMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(message.To);

            if (message.CCs.Any())
            {
                message.CCs.ForEach(cc => mailMessage.CC.Add(cc));
            }

            await _emailSender.SendAsync(mailMessage);
        }

        private void SendDefault(MailPreviewInfoDto message)
        {
            _emailSender.SendAsync(
                to: message.To,
                subject: message.Subject,
                body: message.BodyMessage,
                isBodyHtml: true
            );
        }

        private void SendToCC(MailPreviewInfoDto message)
        {
            var mailMessage = new MailMessage()
            {
                Body = message.BodyMessage,
                Subject = message.Subject
            };
            mailMessage.To.Add(message.To);
            message.CCs.ForEach(cc => mailMessage.CC.Add(cc));
            mailMessage.IsBodyHtml = true;
            _emailSender.SendAsync(mailMessage);
        }

        public async Task CreateEmailHistory(long cvId, long emailTemplateId, string description = "")
        {
            await _workScope.InsertAsync(new EmailStatusHistory
            {
                CVId = cvId,
                EmailTemplateId = emailTemplateId,
                Description = description
            });
        }

        public async Task<MailPreviewInfoDto> GetByIdFakeData(long id)
        {
            var template = await _workScope.GetAsync<EmailTemplate>(id);
            var data = await EmailDispatchData(template.Type);
            return GetContentMail(data.Result, template);
        }

        public async Task<MailPreviewInfoDto> GetById(long id)
        {
            var template = await _workScope.GetAsync<EmailTemplate>(id);
            var data = await EmailDispatchData(template.Type);
            var ccs = string.IsNullOrEmpty(template.CCs) ? new List<string>() : template.CCs.Split(",").ToList();
            return new MailPreviewInfoDto
            {
                BodyMessage = template.BodyMessage,
                Subject = template.Subject,
                TemplateId = template.Id,
                To = "",
                PropertiesSupport = data.PropertiesSupport,
                CCs = ccs
            };
        }

        public IQueryable<MailDto> IQGetEmailTemplate()
        {
            return _workScope.GetAll<EmailTemplate>()
                    .Select(s => new MailDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Type = s.Type,
                        CCs = s.CCs,
                    });
        }

        public async Task<List<MailDto>> GetAllMailTemplate()
        {
            return await IQGetEmailTemplate()
                .OrderBy(s => s.Type)
                .ToListAsync();
        }

        private MailPreviewInfoDto GetContentMail<TDto, TEntity>(TDto data, TEntity mailEntity) where TDto : class where TEntity : class
        {
            Type typeOfEntity = typeof(TEntity);
            Type typeOfDto = typeof(TDto);
            var templateId = typeOfEntity.GetProperty("Id").GetValue(mailEntity) as long?;

            var bodyMessage = typeOfEntity.GetProperty("BodyMessage").GetValue(mailEntity) as string;
            var subject = typeOfEntity.GetProperty("Subject").GetValue(mailEntity) as string;

            var properties = typeOfDto.GetProperties().Select(s => s.Name).ToArray();
            foreach (var property in properties)
            {
                bodyMessage = bodyMessage.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
                subject = subject.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
            }
            var sendTo = typeOfDto.GetProperty("Email").GetValue(data) as string;

            var ccs = typeOfEntity.GetProperty("CCs").GetValue(mailEntity) as string;
            var listCCs = string.IsNullOrEmpty(ccs) ? new List<string>() : ccs.Split(",").ToList();

            var type = typeOfEntity.GetProperty("Type").GetValue(mailEntity) as MailFuncEnum?;

            return new MailPreviewInfoDto
            {
                MailFuncType = type.HasValue ? type.Value : default,
                BodyMessage = bodyMessage,
                Subject = subject,
                TemplateId = templateId.HasValue ? templateId.Value : 0,
                To = sendTo,
                CCs = listCCs
            };
        }

        private async Task<dynamic> EmailDispatchData(MailFuncEnum EmailType, long id = default)
        {
            switch (EmailType)
            {
                case MailFuncEnum.FailedInterview:
                case MailFuncEnum.FailedTest:
                case MailFuncEnum.ScheduledTest:
                case MailFuncEnum.ScheduledInterview:
                case MailFuncEnum.AcceptedOfferJob:
                case MailFuncEnum.AcceptedOfferInternship:
                case MailFuncEnum.RejectedOffer:
                    return await GetDataRequestCV(id);

                case MailFuncEnum.FailedCV:
                    return await GetDataCandidate(id);

                default:
                    return null;
            }
        }

        private async Task<ResultTemplateEmail<AllInformationRequestCVDto>> GetDataRequestCV(long requestCVId)
        {
            if (requestCVId == default)
            {
                var fakeData = new AllInformationRequestCVDto
                {
                    Address = "Ha Noi",
                    ApplyLevel = Level.Intern_0,
                    Avatar = "",
                    BirthdayDateTime = new DateTime(2022, 12, 11),
                    Email = "",
                    FinalLevel = Level.Intern_0,
                    FullName = "Nguyen Van A",
                    HRNote = "",
                    Id = 0,
                    InterviewLevel = Level.Intern_0,
                    InterviewTimeDate = new DateTime(2022, 10, 11),
                    JobPositionRecruit = "Java Developer",
                    OnboardDateTime = new DateTime(2022, 12, 11),
                    Phone = "0912845678",
                    Salary = 1000000,
                    Status = RequestCVStatus.AcceptedOffer,
                    Percentage = "",
                };
                SetSignatureContact(fakeData);
                return new ResultTemplateEmail<AllInformationRequestCVDto>
                {
                    Result = fakeData
                };
            }

            var result = await _workScope.GetAll<RequestCV>()
                .Where(q => q.Id == requestCVId)
                .Select(s => new AllInformationRequestCVDto
                {
                    Id = s.Id,
                    CVId = s.CVId,
                    Address = s.CV.Address,
                    ApplyLevel = s.ApplyLevel,
                    Avatar = s.CV.Avatar,
                    BirthdayDateTime = s.CV.Birthday,
                    Email = s.CV.Email,
                    FinalLevel = s.FinalLevel,
                    FullName = s.CV.Name,
                    HRNote = s.HRNote,
                    InterviewLevel = s.InterviewLevel,
                    InterviewTimeDate = s.InterviewTime,
                    JobPositionRecruit = s.Request.SubPosition.Name,
                    OnboardDateTime = s.OnboardDate,
                    Phone = s.CV.Phone,
                    Salary = s.Salary,
                    Status = s.Status,
                    LMSInfo = s.LMSInfo,
                    Percentage = s.Percentage,
                    RequestBranchName = s.Request.Branch.Name,
                    RequestBranchAddress = s.Request.Branch.Address,
                    CVBranchAddress = s.CV.Branch.Address,
                    CVBranchName = s.CV.Branch.Name,
                }).FirstOrDefaultAsync();
            SetSignatureContact(result);
            return new ResultTemplateEmail<AllInformationRequestCVDto>
            {
                Result = result,
            };
        }

        private async Task<ResultTemplateEmail<CandidateEmailInfo>> GetDataCandidate(long cvId)
        {
            if (cvId == default)
            {
                var fakeCandidateInfo = new CandidateEmailInfo
                {
                    Phone = "0971330333",
                    FullName = "Nguyễn Văn B"
                };
                SetSignatureContact(fakeCandidateInfo);
                return new ResultTemplateEmail<CandidateEmailInfo>
                {
                    Result = fakeCandidateInfo
                };
            }

            var result = await _workScope.GetAll<CV>()
                .Where(q => q.Id == cvId)
                .Select(s => new CandidateEmailInfo
                {
                    CVId = s.Id,
                    Phone = s.Phone,
                    FullName = s.Name,
                    PathAvatar = s.Avatar,
                    Email = s.Email,
                    UserType = s.UserType,
                    CVBranchAddress = s.Branch.Address,
                }).FirstOrDefaultAsync();
            SetSignatureContact(result);
            return new ResultTemplateEmail<CandidateEmailInfo>()
            {
                Result = result
            };
        }

        private void SetSignatureContact<TDto>(TDto dto) where TDto : class
        {
            var user = _workScope.GetAll<User>().Where(q => q.Id == _abpSession.UserId).FirstOrDefault();
            Type type = typeof(TDto);
            var propHRName = type.GetProperty("HRName");
            propHRName.SetValue(dto, user?.Name ?? "HRName");

            var propHRPhone = type.GetProperty("HRPhone");
            propHRPhone.SetValue(dto, user?.PhoneNumber ?? "0123456789");

            var propHREmail = type.GetProperty("HREmail");
            propHREmail.SetValue(dto, user?.EmailAddress ?? "hr@ncc.asia");

            var propSignatureContact = type.GetProperty("SignatureContact");
            propSignatureContact.SetValue(dto, user?.SignatureContact ?? TemplateHelper.DefaulSignature);
        }
    }
}