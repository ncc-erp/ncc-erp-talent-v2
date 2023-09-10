using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Configuration;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.DomainServices.RequestCVs.Dtos;
using TalentV2.Entities;
using TalentV2.Notifications;
using TalentV2.Notifications.Komu;
using TalentV2.Notifications.Templates;
using TalentV2.Notifications.Templates.Dtos;
using TalentV2.Utils;

namespace TalentV2.DomainServices.RequestCVs
{
    public class RequestCVManager : BaseManager, IRequestCVManager
    {
        private readonly IKomuNotification _komuNotification;
        public RequestCVManager(IKomuNotification komuNotification)
        {
            _komuNotification = komuNotification;
        }
        public IQueryable<CandidateOfferDto> IQGetRequestCV()
        {
            var query = from rc in WorkScope.GetAll<RequestCV>()
                        select new CandidateOfferDto
                        {
                            Id = rc.Id,
                            CVId = rc.CVId,
                            UserType = rc.CV.UserType,
                            PathAvatar = rc.CV.Avatar,
                            Email = rc.CV.Email,
                            FullName = rc.CV.Name,
                            PathLinkCV = rc.CV.LinkCV,
                            Phone = rc.CV.Phone,
                            IsFemale = rc.CV.IsFemale,
                            CvStatus = rc.CV.CVStatus,
                            SubPositionId = rc.CV.SubPositionId,
                            SubPositionName = rc.CV.SubPosition.Name,
                            CreationTime = rc.CV.CreationTime,
                            CreatorName = rc.CV.CreatorUserId.HasValue ?
                                    rc.CV.CreatorUser.FullName :
                                    "",
                            LastModifiedTime = rc.LastModificationTime,
                            LastModifiedName = rc.LastModifierUserId.HasValue ?
                                        rc.LastModifierUser.FullName :
                                        "",
                            BranchId = rc.CV.BranchId,
                            BranchName = rc.CV.Branch.Name,
                            DisplayBranchName = rc.CV.Branch.DisplayName,
                            SubPositionIdRequest = rc.Request.SubPositionId,
                            Note = rc.Request.Note,
                            HRNote = rc.HRNote,
                            SubPositionNameRequest = rc.Request.SubPosition.Name,
                            RequestStatus = rc.Request.Status,
                            RequestCVStatus = rc.Status,
                            FinalLevel = rc.FinalLevel,
                            ApplyLevel = rc.ApplyLevel,
                            InterviewLevel = rc.InterviewLevel,
                            OnboardDate = rc.OnboardDate,
                            Salary = rc.Salary,
                            RequestLevel = rc.Request.Level,
                            NCCEmail = rc.CV.NCCEmail,
                            RequestId = rc.RequestId,
                            RequestSkills = rc.Request.RequestSkills.Select(s => new SkillDto
                            {
                                Id = s.SkillId,
                                Name = s.Skill.Name
                            }).ToList(),
                            RequestBracnhId = rc.Request.BranchId,
                            RequestBranchName = rc.Request.Branch.Name,
                            SubRequestPositionId = rc.Request.SubPositionId,
                            SubRequestoPositionName = rc.Request.SubPosition.Name,
                            ProjectToolRequestId = rc.Request.ProjectToolRequestId,
                        };
            return query;
        }
        public async Task<long> UpdateCandidateOffer(UpdateCandidateOfferDto input)
        {
            var requestCV = await WorkScope.GetAsync<RequestCV>(input.Id);

            bool isSendNotification = ((requestCV.Status == RequestCVStatus.AcceptedOffer && input.Status == RequestCVStatus.RejectedOffer)
                || (requestCV.Status != RequestCVStatus.AcceptedOffer && input.Status == RequestCVStatus.AcceptedOffer))
                || (input.Status == RequestCVStatus.AcceptedOffer && Nullable.Compare<DateTime>(requestCV.OnboardDate, input.OnboardDate) != 0);

            ObjectMapper.Map(input, requestCV);
            await CurrentUnitOfWork.SaveChangesAsync();

            if(isSendNotification)
            {
                var isFirstAcceptedOffer = (requestCV.Status != RequestCVStatus.AcceptedOffer && input.Status == RequestCVStatus.AcceptedOffer)
                    && (!requestCV.OnboardDate.HasValue && input.OnboardDate.HasValue);

                _komuNotification.NotifyAcceptedOrRejectedOffer(requestCV.Status, requestCV.Id, isFirstAcceptedOffer);
            }
            return requestCV.Id;
        }
        public async Task<long> UpdateCandidateOnboard(UpdateCandidateOnboardDto input)
        {
            if (input.Status == RequestCVStatus.Onboarded && !input.OnboardDate.HasValue)
                throw new UserFriendlyException("Onboard Date is required.");

            var requestCV = await WorkScope.GetAsync<RequestCV>(input.Id);

            bool isSendNotification = ((requestCV.Status != RequestCVStatus.RejectedOffer && input.Status == RequestCVStatus.RejectedOffer)
                || (requestCV.Status == RequestCVStatus.RejectedOffer && input.Status != RequestCVStatus.RejectedOffer)
                || (input.Status == RequestCVStatus.AcceptedOffer && requestCV.OnboardDate != input.OnboardDate));
            
            bool isFirstAcceptedOffer = true;
            if (input.Status == RequestCVStatus.AcceptedOffer && requestCV.OnboardDate != input.OnboardDate)
            {
                isFirstAcceptedOffer = false;
            }

            ObjectMapper.Map<UpdateCandidateOnboardDto, RequestCV>(input, requestCV);
            var cv = await WorkScope.GetAsync<CV>(requestCV.CVId);
            cv.NCCEmail = input.NCCEmail;
            await CurrentUnitOfWork.SaveChangesAsync();

            if (isSendNotification)
            {
                _komuNotification.NotifyAcceptedOrRejectedOffer(requestCV.Status, requestCV.Id, isFirstAcceptedOffer);
            }

            return requestCV.Id;
        }
    }
}
