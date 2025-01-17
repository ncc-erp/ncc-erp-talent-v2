using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.Notifications.Templates.Dtos;

namespace TalentV2.EntityFrameworkCore.Seed.MezonTemplates
{
    public class DefaultNotificationSettingsCreator
    {
        private readonly TalentV2DbContext _context;
        private int? _tenantId;
        public DefaultNotificationSettingsCreator(TalentV2DbContext context, int? tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateNotificationSettings();
        }

        private void CreateNotificationSettings() 
        {
            var notificationSettings = new List<NotificationSetting>();

            var existNotiSettings = _context.NotificationSettings.IgnoreQueryFilters().Where(q => q.TenantId == _tenantId).Select(x => new
            {
                x.Type,
            }).ToList();

            Enum.GetValues(typeof(NotificationType))
                .Cast<NotificationType>()
                .ToList()
                .ForEach(n =>
                {
                    var seedTemplate = SeedDictionary[n];
                    if (seedTemplate != null)
                    {
                        if (!existNotiSettings.Any(x => x.Type.Equals(n)))
                        {
                            notificationSettings.Add(
                                new NotificationSetting
                                {
                                    TenantId = _tenantId,
                                    Type = n,
                                    WebhookUrl = "",
                                    BodyMessage = seedTemplate.BodyMessage,
                                    Description = "",
                                    IsActived = true
                                }
                            );
                        }
                    }
                });

            _context.AddRange(notificationSettings);
            _context.SaveChanges();
        }

        public readonly Dictionary<NotificationType, NotificationSetting> SeedDictionary = new Dictionary<NotificationType, NotificationSetting>()
        {
            {
                NotificationType.AcceptedOffer,
                new NotificationSetting()
                {
                    BodyMessage = "<p>#{{CVId}} **{{FullName}}** will onboard on {{OnboardDate}} with following info{{CVStatus}}:</p><p>```</p><p>User type: {{UserTypeName}}</p><p>Position: {{SubPositionName}}</p><p>Branch: {{BranchName}}</p><p>Phone: {{Phone}}</p><p>Email: {{Email}}</p><p>NCC Email: {{NCCEmail}}</p><p>```</p>"}
            },
            {
                NotificationType.RejectedOffer,
                new NotificationSetting()
                {
                    BodyMessage = "<p>**[CANCELLED]** #{{CVId}} **{{FullName}}** will onboard on {{OnboardDate}}</p>"
                }
            },
            {
                NotificationType.UpdatedPersonalInfo,
                new NotificationSetting()
                {
                    BodyMessage = "<p>#{{CVId}} **{{FullName}}** will onboarding on **{{OnboardDate}}** has been changed with following info:</p><p>```</p><p>User type: {{UserTypeName}}</p><p>Branch: {{BranchName}}</p><p>Phone: {{Phone}}</p><p>Email: {{Email}}</p><p>NCC Email: {{NCCEmail}}</p><p>```</p>",
                }
            },
            {
                NotificationType.ChannelNotice_InterviewRemind,
                new NotificationSetting()
                {
                    BodyMessage = "<p>Người phỏng vấn: {{InterviewerEmails}}</p><p>```</p><p>Bạn có lịch phỏng vấn ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.ChannelNotice_CandidateEvaluation,
                new NotificationSetting()
                {
                    BodyMessage = "<p>Người phỏng vấn: {{InterviewerEmails}}</p><p>```</p><p>Bạn cần nhập đánh giá ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.ChannelNotice_CandidateEvaluationAndLevel,
                new NotificationSetting()
                {
                    BodyMessage = "<p>Người phỏng vấn: {{InterviewerEmails}}</p><p>```</p><p>Bạn cần nhập đánh giá ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>Bạn cần nhập Interviewer suggest level: ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.UserNotice_InterviewRemind,
                new NotificationSetting()
                {
                    BodyMessage = "<p>```</p><p>Bạn có lịch phỏng vấn ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.UserNotice_CandidateEvaluation,
                new NotificationSetting()
                {
                    BodyMessage = "<p>```</p><p>Bạn cần nhập đánh giá ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.UserNotice_CandidateEvaluationAndLevel,
                new NotificationSetting()
                {
                    BodyMessage = "<p>```</p><p>Bạn cần nhập đánh giá ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>Bạn cần nhập Interviewer suggest level: ứng viên {{CandidateFulName}} [{{BranchName}}] {{UserType}} {{PositionName}} phỏng vấn ngày: {{TimeInterview}}</p><p>{{CVLink}}</p><p>```</p>",
                }
            },
            {
                NotificationType.CrawlCV_MessageToChannel,
                new NotificationSetting()
                {
                    BodyMessage = "<p>Automatically created CV from NCC Career successfully:</p><p>**Intern CV: {{InternCVQuantity}}** --- [New Intern CVs Here]({{InternLink}})</p><p>**Staff CV: {{StaffCVQuantity}}** --- [New Staff CVs Here]({{StaffLink}})</p><p>HR: {{TagNames}}</p><p>Please check the created CV information at {{LinkOrTalent}}.</p>",
                }
            },
            {
                NotificationType.CrawlCV_MessageToUser,
                new NotificationSetting()
                {
                    BodyMessage = "<p>Automatically created CV from NCC Career successfully:</p><p>**Intern CV: {{InternCVQuantity}}** --- [New Intern CVs Here]({{InternLink}})</p><p>**Staff CV: {{StaffCVQuantity}}** --- [New Staff CVs Here]({{StaffLink}})</p><p>Please check the created CV information at {{LinkOrTalent}}.</p>",
                }
            },
        };
    }
}
