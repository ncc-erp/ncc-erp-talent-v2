using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Constants.Enum.NccCVs
{
    public static class ErrorCodes
    {
        public static class NotFound
        {
            public const string UserLogin = "4040001";
            public const string UserNotExist = "4040002";
            public const string Candidate = "4040003";
            public const string WorkingExperience = "4040004";
            public const string FileUpload = "4040005";
            public const string VersionOfCV = "4040006";
            public const string Presenter = "4040007";
            public const string NoBranch = "4040008";
            public const string NoPosition = "4040009";
            public const string NoSourceType = "4040010";
            public const string NoSkill = "4040011";
            public const string EmployeeFutureNotExist = "4040012";
        }
        public static class Forbidden
        {
            public const string AccessOtherProfile = "4030001";
            public const string EditOtherProfile = "4030002";
            public const string AddOrEditCandidate = "4030003";
            public const string DeleteAnotherFiles = "4030004";
            public const string ViewOtherProfile = "4030005";

        }
        public static class Unauthorized
        {
            public const string UserNotLogin = "4010001";
        }
        public static class Conflict
        {
            public const string DuplicatePosition = "4090001";

            public const string VersionName = "4090002";

            public const string SourceName = "4090003";
            public const string EmployeeFuture = "4090004";
        }
        public static class NoContent
        {
            public const string Candidate = "2040001";
            public const string Candidate_Status = "2040002";
        }

        public static class NotAcceptable
        {
            public const string FileNameExtensions = "4060001";
            public const string YearIsNotValid = "4060002";
            public const string YearOutOfRange = "4060003";
        }

        public static class BadRequest
        {
            public const string LackStartTime = "4000001";

            public const string PresenterIdMissing = "4000002";

            public const string SourceConnectMissing = "4000003";

            public const string BranchIsUsed = "4000004";

            public const string PositionIsUsed = "4000005";

            public const string SourceTypeIsUsed = "4000006";

            public const string SkillIsUsed = "4000007";

            public const string SourceIsUsed = "4000008";

            public const string CVCandidateIsInRecruitment = "4000009";

            public const string RequestIsClose = "4000010";

            public const string CVCandidateIsSentMail = "4000011";

            public const string IsHasInterviewerCandidate = "4000012";

        }
    }
}
