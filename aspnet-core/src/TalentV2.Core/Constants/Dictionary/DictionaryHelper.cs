using System.Collections.Generic;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
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

        public static readonly Dictionary<string, string> NCCBranchNames =
             new()
            {
                { "Ho Chi Minh", "HCM" },
                { "Ha Noi 1", "HN1" },
                { "Ha Noi 2", "HN2" },
                { "Ha Noi 3", "HN3" },
                { "Da Nang", "DN" },
                { "Vinh", "Vinh" },
                { "Quy Nhon", "QN" },

            };
        public static readonly Dictionary<MailFuncEnum, List<MailInfo>> SeedMailDic = new Dictionary<MailFuncEnum, List<MailInfo>>()
        {
            {
                MailFuncEnum.AcceptedOfferJob,
                new List<MailInfo>()
                {
                    new() {
                        Name = "[Accepted Offer] Job invitation",
                        Description = "Job invitation, sent when candidate has accepted HR's offer",
                        Subject = "[NCC]_THƯ MỜI NHẬN VIỆC",
                    }
                }
            },
            {
                MailFuncEnum.FailedInterview,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Failed Interview] Interview result",
                        Description = "Interview result announcement, sent when candidate has passed the interview",
                        Subject = "[NCC]_THÔNG BÁO KẾT QUẢ",
                    }
                }
            },
            {
                MailFuncEnum.FailedCV,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Failed CV] Thank-you mail",
                        Description = "Thank-you mail, sent when the CV has been considered failed",
                        Subject = "[NCC]_THƯ CẢM ƠN",
                    }
                }
            },
            {
                MailFuncEnum.AcceptedOfferInternship,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Accepted Offer] Internship invitation",
                        Description = "Internship invitation, sent when candidate has accepted HR's offer",
                        Subject = "[NCC]_THƯ MỜI THỰC TẬP",
                    }
                }
            },
            {
                MailFuncEnum.ScheduledTest,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Scheduled Test] Test invitation",
                        Description = "Test invitation, sent when candidate has passed the test and is scheduled for interview",
                        Subject = "[NCC]_THƯ MỜI LÀM BÀI TEST"
                    },
                    new()
                    {
                        Name = "[Scheduled Test] Developer Test invitation",
                        Description = "Developer Test invitation, sent when candidate has passed the test and is scheduled for interview",
                        Subject = "[NCC]_THƯ MỜI LÀM BÀI TEST",
                        Version = "Developer"
                    },
                    new()
                    {
                        Name = "[Scheduled Test] Tester Test invitation",
                        Description = "Tester Test invitation, sent when candidate has passed the test and is scheduled for interview",
                        Subject = "[NCC]_THƯ MỜI LÀM BÀI TEST",
                        Version = "Tester"
                    }
                }
            },
            {
                MailFuncEnum.ScheduledInterview,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Scheduled Interview] Interview invitation",
                        Description = "Interview invitation, sent when candidate has passed the test",
                        Subject = "[NCC]_THƯ MỜI THAM GIA PHỎNG VẤN",
                    }
                }
            },
            {
                MailFuncEnum.FailedTest,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Failed Test] Test result",
                        Description = "Test result announcement, sent when candidate has failed the test",
                        Subject = "[NCC]_THƯ THÔNG BÁO KẾT QUẢ BÀI TEST",
                    }
                }
            },
            {
                MailFuncEnum.RejectedOffer,
                new List<MailInfo>()
                {
                    new()
                    {
                        Name = "[Rejected Offer] Thank-you mail",
                        Description = "Thank-you mail, sent when candidate has rejected the offer",
                        Subject = "[NCC]_THƯ CẢM ƠN",
                    }
                }
            },
        };

        public static readonly Dictionary<RequestCVStatus, string> RequestCVStatusDict = new Dictionary<RequestCVStatus, string>
        {
            {RequestCVStatus.AddedCV, "Added CV" },
            {RequestCVStatus.ScheduledTest, "Scheduled Test" },
            {RequestCVStatus.FailedTest, "Failed Test" },
            {RequestCVStatus.PassedTest, "Passed Test" },
            {RequestCVStatus.ScheduledInterview, "Scheduled Interview" },
            {RequestCVStatus.RejectedInterview, "Rejected Interview" },
            {RequestCVStatus.PassedInterview, "Passed Interview" },
            {RequestCVStatus.FailedInterview, "Failed Interview" },
            {RequestCVStatus.AcceptedOffer, "Accepted Offer" },
            {RequestCVStatus.RejectedOffer, "Rejected Offer" },
            {RequestCVStatus.Onboarded, "Onboarded" },
            {RequestCVStatus.RejectedTest, "Rejected Test" },
            {RequestCVStatus.RejectedApply, "Rejected Apply" },
        };

        public static readonly Dictionary<Level, LevelDto> LevelDict = new Dictionary<Level, LevelDto>
        {
            {
                Level.Intern_0,
                new LevelDto
                {
                    Id = Level.Intern_0.GetHashCode(),
                    DefaultName = Level.Intern_0.ToString(),
                    StandardName = "Intern 0",
                    ShortName = "I0"
                }
            },
            {
                Level.Intern_1,
                new LevelDto
                {
                    Id = Level.Intern_1.GetHashCode(),
                    DefaultName = Level.Intern_1.ToString(),
                    StandardName = "Intern 1",
                    ShortName = "I1"
                }
            },
            {
                Level.Intern_2,
                new LevelDto
                {
                    Id = Level.Intern_2.GetHashCode(),
                    DefaultName = Level.Intern_2.ToString(),
                    StandardName = "Intern 2",
                    ShortName = "I2"
                }
            },
            {
                Level.Intern_3,
                new LevelDto
                {
                    Id = Level.Intern_3.GetHashCode(),
                    DefaultName = Level.Intern_3.ToString(),
                    StandardName = "Intern 3",
                    ShortName = "I3"
                }
            },
            {
                Level.FresherMinus,
                new LevelDto
                {
                    Id = Level.FresherMinus.GetHashCode(),
                    DefaultName = Level.FresherMinus.ToString(),
                    StandardName = "Fresher-",
                    ShortName = "F-"
                }
            },
            {
                Level.Fresher,
                new LevelDto
                {
                    Id = Level.Fresher.GetHashCode(),
                    DefaultName = Level.Fresher.ToString(),
                    StandardName = "Fresher",
                    ShortName = "F"
                }
            },
            {
                Level.FresherPlus,
                new LevelDto
                {
                    Id = Level.FresherPlus.GetHashCode(),
                    DefaultName = Level.FresherPlus.ToString(),
                    StandardName = "Fresher+",
                    ShortName = "F+"
                }
            },
            {
                Level.JuniorMinus,
                new LevelDto
                {
                    Id = Level.JuniorMinus.GetHashCode(),
                    DefaultName = Level.JuniorMinus.ToString(),
                    StandardName = "Junior-",
                    ShortName = "J-"
                }
            },
            {
                Level.Junior,
                new LevelDto
                {
                    Id = Level.Junior.GetHashCode(),
                    DefaultName = Level.Junior.ToString(),
                    StandardName = "Junior",
                    ShortName = "J"
                }
            },
            {
                Level.JuniorPlus,
                new LevelDto
                {
                    Id = Level.JuniorPlus.GetHashCode(),
                    DefaultName = Level.JuniorPlus.ToString(),
                    StandardName = "Junior+",
                    ShortName = "J+"
                }
            },
            {
                Level.MiddleMinus,
                new LevelDto
                {
                    Id = Level.MiddleMinus.GetHashCode(),
                    DefaultName = Level.MiddleMinus.ToString(),
                    StandardName = "Middle-",
                    ShortName = "M-"
                }
            },
            {
                Level.Middle,
                new LevelDto
                {
                    Id = Level.Middle.GetHashCode(),
                    DefaultName = Level.Middle.ToString(),
                    StandardName = "Middle",
                    ShortName = "M"
                }
            },
            {
                Level.MiddlePlus,
                new LevelDto
                {
                    Id = Level.MiddlePlus.GetHashCode(),
                    DefaultName = Level.MiddlePlus.ToString(),
                    StandardName = "Middle+",
                    ShortName = "M+"
                }
            },
            {
                Level.SeniorMinus,
                new LevelDto
                {
                    Id = Level.SeniorMinus.GetHashCode(),
                    DefaultName = Level.SeniorMinus.ToString(),
                    StandardName = "Senior-",
                    ShortName = "S-"
                }
            },
            {
                Level.Senior,
                new LevelDto
                {
                    Id = Level.Senior.GetHashCode(),
                    DefaultName = Level.Senior.ToString(),
                    StandardName = "Senior",
                    ShortName = "S"
                }
            },
            {
                Level.Principal,
                new LevelDto
                {
                    Id = Level.Principal.GetHashCode(),
                    DefaultName = Level.Principal.ToString(),
                    StandardName = "Principal",
                    ShortName = "P"
                }
            },
            {
                Level.AnyLevel,
                new LevelDto
                {
                    Id = Level.AnyLevel.GetHashCode(),
                    DefaultName = Level.AnyLevel.ToString(),
                    StandardName = "AnyLevel",
                    ShortName = "A"
                }
            },
            {
                Level.Fail,
                new LevelDto
                {
                    Id = Level.Fail.GetHashCode(),
                    DefaultName = Level.Fail.ToString(),
                    StandardName = "Fail",
                    ShortName = "F"
                }
            },
        };
    }
}
