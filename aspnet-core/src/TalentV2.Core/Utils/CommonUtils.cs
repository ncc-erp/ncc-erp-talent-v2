using Abp.UI;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TalentV2.Constants.Const;
using TalentV2.Constants.Dictionary;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.Categories.Dtos;
using TalentV2.ModelExtends;

namespace TalentV2.Utils
{
    public class CommonUtils
    {
        public static string GetEnumName<T>(T t) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T must be enumerated type.");
            return Enum.GetName(typeof(T), t);
        }

        public static List<T> GetListEnum<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T must be enumerated type.");
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<CategoryDto> ListRequestCVStatus = new List<CategoryDto>
        {
            new CategoryDto { Id = RequestCVStatus.AddedCV.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.AddedCV] },
            new CategoryDto { Id = RequestCVStatus.RejectedApply.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedApply] },
            new CategoryDto { Id = RequestCVStatus.ScheduledTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.ScheduledTest] },
            new CategoryDto { Id = RequestCVStatus.RejectedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedTest] },
            new CategoryDto { Id = RequestCVStatus.FailedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedTest] },
            new CategoryDto { Id = RequestCVStatus.PassedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.PassedTest] },
            new CategoryDto { Id = RequestCVStatus.ScheduledInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.ScheduledInterview] },
            new CategoryDto { Id = RequestCVStatus.RejectedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedInterview] },
            new CategoryDto { Id = RequestCVStatus.PassedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.PassedInterview] },
            new CategoryDto { Id = RequestCVStatus.FailedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedInterview] },
            new CategoryDto { Id = RequestCVStatus.AcceptedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.AcceptedOffer] },
            new CategoryDto { Id = RequestCVStatus.RejectedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedOffer] },
            new CategoryDto { Id = RequestCVStatus.Onboarded.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.Onboarded] },
        };

        public static List<CategoryDto> ListInterviewStatus = new List<CategoryDto>
        {
            new CategoryDto {Id = RequestCVStatus.ScheduledInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.ScheduledInterview]},
            new CategoryDto {Id = RequestCVStatus.PassedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.PassedInterview]},
            new CategoryDto {Id = RequestCVStatus.FailedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedInterview]},
        };

        public static List<CategoryDto> ListStatusDeadline = new List<CategoryDto>
        {
            new CategoryDto {Id = RequestCVStatus.FailedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedTest]},
            new CategoryDto {Id = RequestCVStatus.ScheduledTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.ScheduledTest]},
            new CategoryDto {Id = RequestCVStatus.ScheduledInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.ScheduledInterview]},
            new CategoryDto {Id = RequestCVStatus.FailedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedInterview]},
            new CategoryDto {Id = RequestCVStatus.AcceptedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.AcceptedOffer]},
            new CategoryDto {Id = RequestCVStatus.RejectedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedOffer]},
        };

        public static List<CategoryDto> ListStatusCandidateOffer = new List<CategoryDto>
        {
            new CategoryDto {Id = RequestCVStatus.PassedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.PassedInterview]},
            new CategoryDto {Id = RequestCVStatus.AcceptedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.AcceptedOffer]},
            new CategoryDto {Id = RequestCVStatus.RejectedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedOffer]},
        };

        public static List<CategoryDto> ListStatusCandidateOnboard = new List<CategoryDto>
        {
            new CategoryDto {Id = RequestCVStatus.AcceptedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.AcceptedOffer]},
            new CategoryDto {Id = RequestCVStatus.RejectedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedOffer]},
            new CategoryDto {Id = RequestCVStatus.Onboarded.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.Onboarded]},
        };

        public static List<CategoryDto> ListStatusNotAvailableClone = new List<CategoryDto>
        {
            new CategoryDto {Id = RequestCVStatus.RejectedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedTest]},
            new CategoryDto {Id = RequestCVStatus.RejectedApply.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedApply]},
            new CategoryDto {Id = RequestCVStatus.FailedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedInterview]},
            new CategoryDto {Id = RequestCVStatus.RejectedOffer.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedOffer]},
            new CategoryDto {Id = RequestCVStatus.Onboarded.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.Onboarded]},
            new CategoryDto {Id = RequestCVStatus.FailedTest.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.FailedTest]},
            new CategoryDto {Id = RequestCVStatus.RejectedInterview.GetHashCode(), Name = DictionaryHelper.RequestCVStatusDict[RequestCVStatus.RejectedInterview]},
        };

        public static List<CategoryDto> ListCVStatusNotAvailableClone = new List<CategoryDto> {
            new CategoryDto {Id = CVStatus.Failed.GetHashCode(), Name = CVStatus.Failed.ToString()},
        };

        public static List<CategoryDto> ListRequestStatus = new List<CategoryDto>
        {
            new CategoryDto {Id = StatusRequest.InProgress.GetHashCode(), Name = "In Progress"},
            new CategoryDto {Id= StatusRequest.Closed.GetHashCode(), Name = "Closed"}
        };

        public static List<LevelDto> ListLevel = new List<LevelDto>
        {
            DictionaryHelper.LevelDict[Level.Intern_0],
            DictionaryHelper.LevelDict[Level.Intern_1],
            DictionaryHelper.LevelDict[Level.Intern_2],
            DictionaryHelper.LevelDict[Level.Intern_3],
            DictionaryHelper.LevelDict[Level.FresherMinus],
            DictionaryHelper.LevelDict[Level.Fresher],
            DictionaryHelper.LevelDict[Level.FresherPlus],
            DictionaryHelper.LevelDict[Level.JuniorMinus],
            DictionaryHelper.LevelDict[Level.Junior],
            DictionaryHelper.LevelDict[Level.JuniorPlus],
            DictionaryHelper.LevelDict[Level.MiddleMinus],
            DictionaryHelper.LevelDict[Level.Middle],
            DictionaryHelper.LevelDict[Level.MiddlePlus],
            DictionaryHelper.LevelDict[Level.SeniorMinus],
            DictionaryHelper.LevelDict[Level.Senior],
            DictionaryHelper.LevelDict[Level.Principal],
            DictionaryHelper.LevelDict[Level.AnyLevel],
        };

        public static List<CategoryDto> ListPriority = Enum.GetValues(typeof(Priority))
            .Cast<Priority>()
            .Select(x => new CategoryDto
            {
                Id = x.GetHashCode(),
                Name = x.ToString(),
            }).ToList();

        public static List<CategoryDto> ListRequestLevel = new List<CategoryDto>()
        {
            new CategoryDto { Id = Level.AnyLevel.GetHashCode(), Name = DictionaryHelper.LevelDict[Level.AnyLevel].StandardName },
            new CategoryDto { Id = Level.Fresher.GetHashCode(), Name = DictionaryHelper.LevelDict[Level.Fresher].StandardName },
            new CategoryDto { Id = Level.Junior.GetHashCode(), Name = DictionaryHelper.LevelDict[Level.Junior].StandardName },
            new CategoryDto { Id = Level.Middle.GetHashCode(), Name = DictionaryHelper.LevelDict[Level.Middle].StandardName },
            new CategoryDto { Id = Level.Senior.GetHashCode(), Name = DictionaryHelper.LevelDict[Level.Senior].StandardName }
        };

        public static List<InternSalaryDto> ListSalaryIntern = new List<InternSalaryDto>()
        {
            new InternSalaryDto
            {
                Id = Level.Intern_0.GetHashCode(),
                DefaultName = DictionaryHelper.LevelDict[Level.Intern_0].DefaultName,
                StandardName = DictionaryHelper.LevelDict[Level.Intern_0].StandardName,
                ShortName = DictionaryHelper.LevelDict[Level.Intern_0].ShortName,
                Salary = 0
            },
            new InternSalaryDto
            {
                Id = Level.Intern_1.GetHashCode(),
                DefaultName = DictionaryHelper.LevelDict[Level.Intern_1].DefaultName,
                StandardName = DictionaryHelper.LevelDict[Level.Intern_1].StandardName,
                ShortName = DictionaryHelper.LevelDict[Level.Intern_1].ShortName,
                Salary = 1000000
            },
            new InternSalaryDto
            {
                Id = Level.Intern_2.GetHashCode(),
                DefaultName = DictionaryHelper.LevelDict[Level.Intern_2].DefaultName,
                StandardName = DictionaryHelper.LevelDict[Level.Intern_2].StandardName,
                ShortName = DictionaryHelper.LevelDict[Level.Intern_2].ShortName,
                Salary = 2000000
            },
            new InternSalaryDto
            {
                Id = Level.Intern_3.GetHashCode(),
                DefaultName = DictionaryHelper.LevelDict[Level.Intern_3].DefaultName,
                StandardName = DictionaryHelper.LevelDict[Level.Intern_3].StandardName,
                ShortName = DictionaryHelper.LevelDict[Level.Intern_3].ShortName,
                Salary = 4000000
            }
        };

        public static List<LevelDto> ListLevelStaff = new List<LevelDto>()
        {
            DictionaryHelper.LevelDict[Level.FresherMinus],
            DictionaryHelper.LevelDict[Level.Fresher],
            DictionaryHelper.LevelDict[Level.FresherPlus],
            DictionaryHelper.LevelDict[Level.JuniorMinus],
            DictionaryHelper.LevelDict[Level.Junior],
            DictionaryHelper.LevelDict[Level.JuniorPlus],
            DictionaryHelper.LevelDict[Level.MiddleMinus],
            DictionaryHelper.LevelDict[Level.Middle],
            DictionaryHelper.LevelDict[Level.MiddlePlus],
            DictionaryHelper.LevelDict[Level.SeniorMinus],
            DictionaryHelper.LevelDict[Level.Senior],
            DictionaryHelper.LevelDict[Level.Principal],
            DictionaryHelper.LevelDict[Level.AnyLevel],
        };

        public static List<LevelDto> ListLevelInterviewStaff = new List<LevelDto>(ListLevelStaff)
        {
            DictionaryHelper.LevelDict[Level.Intern_0],
            DictionaryHelper.LevelDict[Level.Intern_1],
            DictionaryHelper.LevelDict[Level.Intern_2],
            DictionaryHelper.LevelDict[Level.Intern_3],
        };

        public static List<LevelDto> ListLevelFinalStaff = new List<LevelDto>(ListLevelStaff)
        {
            DictionaryHelper.LevelDict[Level.Intern_0],
            DictionaryHelper.LevelDict[Level.Intern_1],
            DictionaryHelper.LevelDict[Level.Intern_2],
            DictionaryHelper.LevelDict[Level.Intern_3],
        };

        public static List<InternSalaryDto> ListLevelFinalIntern = new List<InternSalaryDto>(ListSalaryIntern)
        {
            new InternSalaryDto{ Id = Level.FresherMinus.GetHashCode(), DefaultName = Level.FresherMinus.ToString(), StandardName = "Fresher-", ShortName = "F-", Salary = 0},
            new InternSalaryDto{ Id = Level.Fresher.GetHashCode(), DefaultName = Level.Fresher.ToString(), StandardName = "Fresher", ShortName = "F", Salary = 0},
            new InternSalaryDto{ Id = Level.FresherPlus.GetHashCode(), DefaultName = Level.FresherPlus.ToString(), StandardName = "Fresher+", ShortName = "F+", Salary = 0},
        };

        public static void CheckFormatFile(IFormFile file, string typeFile)
        {
            if (file == null)
                return;
            var fileExt = Path.GetExtension(file.FileName).Substring(1).ToLower();
            if (!DictionaryHelper.FileTypeDic[typeFile].Contains(fileExt))
            {
                throw new UserFriendlyException($"Wrong Format! Allow File Type: {string.Join(",", DictionaryHelper.FileTypeDic[typeFile])}");
            }
        }

        public static void CheckFormatFile(string fileName, string typeFile)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            var fileExt = Path.GetExtension(fileName).Substring(1).ToLower();
            if (!DictionaryHelper.FileTypeDic[typeFile].Contains(fileExt))
            {
                throw new UserFriendlyException($"Wrong Format! Allow File Type: {string.Join(",", DictionaryHelper.FileTypeDic[typeFile])}");
            }
        }

        public static void CheckSizeFile(IFormFile file)
        {
            if (file.Length > TalentConstants.MaxSizeFile)
                throw new UserFriendlyException($"File cannot be larger {TalentConstants.MaxSizeFile / TalentConstants.MEGA_BYTE}MB");
        }

        public static string FullFilePath(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                return string.Empty;
            if (TalentConstants.UploadFileProvider == TalentConstants.AmazoneS3)
            {
                return AmazoneS3Constant.CloudFront.TrimEnd('/') + "/" + filePath;
            }
            else
            {
                return TalentConstants.BaseBEAddress.TrimEnd('/') + "/" + filePath;
            }
        }

        public static List<CategoryDto> ListCVStatus => Enum.GetValues(typeof(CVStatus))
            .Cast<CVStatus>()
            .Select(s => new CategoryDto
            {
                Id = s.GetHashCode(),
                Name = s.ToString(),
            }).ToList();

        public static List<CategoryDto> ListCVSourceReferenceType => Enum.GetValues(typeof(CVSourceReferenceType))
            .Cast<CVSourceReferenceType>()
            .Select(s => new CategoryDto
            {
                Id = s.GetHashCode(),
                Name = s.ToString(),
            }).ToList();

        public static List<UserTypeDto> ListUserType => Enum.GetValues(typeof(UserType))
            .Cast<UserType>()
            .Select(s => new UserTypeDto
            {
                Id = s.GetHashCode(),
                UserTypeName = s.ToString()
            }).ToList();

        public static RequestCVStatus[] GetOppositionStatus(RequestCVStatus status) =>
             status switch
             {
                 RequestCVStatus.RejectedApply => new RequestCVStatus[] {
                    RequestCVStatus.ScheduledTest,
                    RequestCVStatus.FailedTest,
                    RequestCVStatus.RejectedTest,
                    RequestCVStatus.PassedTest,
                    RequestCVStatus.PassedInterview,
                    RequestCVStatus.FailedInterview,
                    RequestCVStatus.AcceptedOffer,
                    RequestCVStatus.RejectedOffer,
                    RequestCVStatus.RejectedInterview,
                    RequestCVStatus.ScheduledInterview,
                    RequestCVStatus.Onboarded
                 },
                 RequestCVStatus.RejectedTest => new RequestCVStatus[] {
                    RequestCVStatus.RejectedApply,
                    RequestCVStatus.FailedTest,
                    RequestCVStatus.PassedTest,
                    RequestCVStatus.PassedInterview,
                    RequestCVStatus.FailedInterview,
                    RequestCVStatus.AcceptedOffer,
                    RequestCVStatus.RejectedOffer,
                    RequestCVStatus.RejectedInterview,
                    RequestCVStatus.ScheduledInterview,
                    RequestCVStatus.Onboarded
                 },
                 RequestCVStatus.ScheduledTest => new RequestCVStatus[] { RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.FailedTest => new RequestCVStatus[] { RequestCVStatus.PassedTest, RequestCVStatus.RejectedTest, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.PassedTest => new RequestCVStatus[] { RequestCVStatus.FailedTest, RequestCVStatus.RejectedTest, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.PassedInterview => new RequestCVStatus[] { RequestCVStatus.FailedInterview, RequestCVStatus.RejectedInterview, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.FailedInterview => new RequestCVStatus[] { RequestCVStatus.PassedInterview, RequestCVStatus.RejectedInterview, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.AcceptedOffer => new RequestCVStatus[] { RequestCVStatus.RejectedOffer, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.RejectedOffer => new RequestCVStatus[] { RequestCVStatus.AcceptedOffer, RequestCVStatus.Onboarded, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.RejectedInterview => new RequestCVStatus[] { RequestCVStatus.PassedInterview, RequestCVStatus.FailedInterview, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.ScheduledInterview => new RequestCVStatus[] { RequestCVStatus.RejectedInterview, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 RequestCVStatus.Onboarded => new RequestCVStatus[] { RequestCVStatus.RejectedOffer, RequestCVStatus.RejectedApply, RequestCVStatus.RejectedTest },
                 _ => default
             };

        public static MailFuncEnum MapRequestCVStatusToEmailType(RequestCVStatus requestCVStatus, UserType userType = default) =>
            requestCVStatus switch
            {
                RequestCVStatus.FailedTest => MailFuncEnum.FailedTest,
                RequestCVStatus.ScheduledInterview => MailFuncEnum.ScheduledInterview,
                RequestCVStatus.AcceptedOffer => userType == UserType.Intern ? MailFuncEnum.AcceptedOfferInternship : MailFuncEnum.AcceptedOfferJob,
                RequestCVStatus.FailedInterview => MailFuncEnum.FailedInterview,
                RequestCVStatus.ScheduledTest => MailFuncEnum.ScheduledTest,
                RequestCVStatus.RejectedOffer => MailFuncEnum.RejectedOffer,
                _ => default
            };

        public static List<RequestCVStatus> ListRequestCVStatusSendMail = new List<RequestCVStatus>
        {
            RequestCVStatus.FailedInterview,
            RequestCVStatus.FailedTest,
            RequestCVStatus.ScheduledInterview,
            RequestCVStatus.AcceptedOffer,
            RequestCVStatus.ScheduledTest
        };

        public static List<RequestCVStatus> ListEndRequestCVStatus = new List<RequestCVStatus>
        {
            RequestCVStatus.FailedTest,
            RequestCVStatus.FailedInterview,
            RequestCVStatus.RejectedInterview,
            RequestCVStatus.RejectedOffer,
            RequestCVStatus.Onboarded,
            RequestCVStatus.RejectedTest,
            RequestCVStatus.RejectedApply
        };

        public static List<DayOfWeekDto> DayOfWeeks = new List<DayOfWeekDto>
        {
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Sunday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Sunday), DayOfWeekNameVN = "Chủ Nhật"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Saturday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Saturday), DayOfWeekNameVN = "Thứ 7"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Friday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Friday), DayOfWeekNameVN = "Thứ 6"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Thursday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Thursday), DayOfWeekNameVN = "Thứ 5"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Wednesday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Wednesday), DayOfWeekNameVN = "Thứ 4"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Tuesday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Tuesday), DayOfWeekNameVN = "Thứ 3"},
            new DayOfWeekDto {DayOfWeek = DayOfWeek.Monday, DayOfWeekNameEL = GetEnumName(DayOfWeek.Monday), DayOfWeekNameVN = "Thứ 2"},
        };
        public static string GetUserNameByEmail(string email)
        {
            return email.Split("@")[0];
        }
        public static string GetDiscordTagUser(string email)
        {
            return "${" + GetUserNameByEmail(email) + "}";
        }
    }
}