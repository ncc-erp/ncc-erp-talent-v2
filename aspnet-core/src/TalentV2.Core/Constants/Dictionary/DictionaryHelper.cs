using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Notifications.Templates.Dtos;

namespace TalentV2.Constants.Dictionary
{
    public static class DictionaryHelper
    {
        public static readonly Dictionary<string, string[]> FileTypeDic =
            new Dictionary<string, string[]>()
            {
                {"IMAGE", new string[] { "jpeg", "png", "svg", "jpg"} },
                {"DOCUMENT", new string[]{ "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt"} }
            };
        public static readonly Dictionary<MailFuncEnum, MailInfo> SeedMailDic = new Dictionary<MailFuncEnum, MailInfo>()
        {
            {
                MailFuncEnum.AcceptedOfferJob,
                new MailInfo
                {
                    Name = "[Accepted Offer] Job invitation",
                    Description = "Job invitation, sent when candidate has accepted HR's offer",
                    Subject = "[NCC]_THƯ MỜI NHẬN VIỆC",
                }
            },
            {
                MailFuncEnum.FailedInterview,
                new MailInfo
                {
                    Name = "[Failed Interview] Interview result",
                    Description = "Interview result announcement, sent when candidate has passed the interview",
                    Subject = "[NCC]_THÔNG BÁO KẾT QUẢ",
                }
            },
            {
                MailFuncEnum.FailedCV,
                new MailInfo
                {
                    Name = "[Failed CV] Thank-you mail",
                    Description = "Thank-you mail, sent when the CV has been considered failed",
                    Subject = "[NCC]_THƯ CẢM ƠN",
                }
            },
            {
                MailFuncEnum.AcceptedOfferInternship,
                new MailInfo
                {
                    Name = "[Accepted Offer] Internship invitation",
                    Description = "Internship invitation, sent when candidate has accepted HR's offer",
                    Subject = "[NCC]_THƯ MỜI THỰC TẬP",
                }
            },
            {
                MailFuncEnum.ScheduledTest,
                new MailInfo
                {
                    Name = "[Scheduled Test] Test invitation",
                    Description = "Test invitation, sent when candidate has passed the test and is scheduled for interview",
                    Subject = "[NCC]_THƯ MỜI LÀM BÀI TEST",
                }
            },
            {
                MailFuncEnum.ScheduledInterview,
                new MailInfo
                {
                    Name = "[Scheduled Interview] Interview invitation",
                    Description = "Interview invitation, sent when candidate has passed the test",
                    Subject = "[NCC]_THƯ MỜI THAM GIA PHỎNG VẤN",
                }
            },
            {
                MailFuncEnum.FailedTest,
                new MailInfo
                {
                    Name = "[Failed Test] Test result",
                    Description = "Test result announcement, sent when candidate has failed the test",
                    Subject = "[NCC]_THƯ THÔNG BÁO KẾT QUẢ BÀI TEST",
                }
            },
            {
                MailFuncEnum.RejectedOffer,
                new MailInfo
                {
                    Name = "[Rejected Offer] Thank-you mail",
                    Description = "Thank-you mail, sent when candidate has rejected the offer",
                    Subject = "[NCC]_THƯ CẢM ƠN",
                }
            },
        };
    }
}
