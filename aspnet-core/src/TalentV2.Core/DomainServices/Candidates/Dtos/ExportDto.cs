using System;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class Candidate
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public CVStatus CvStatus { get; set; }
        public string Email { get; set; }
        public string Branch { get; set; }
        public string Education { get; set; }
        public string Positon { get; set; }
        public string ApplyLevel { get; set; }
        public string FinalLevel { get; set; }
        public string InterviewLevel { get; set; }
        public RequestCVStatus Status { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public string CV { get; set; }
    }
    public class InterView
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? Time { get; set; }
        public string Positon { get; set; }
        public string Branch { get; set; }
        public RequestCVStatus Status { get; set; }
        public string ApplyLevel { get; set; }
        public string FinalLevel { get; set; }
        public string InterviewLevel { get; set; }
        public string Score { get; set; }
        public string TalentLink { get; set; }
    }
    public class CadidateReport
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public CVStatus CvStatus { get; set; }
        public string Education { get; set; }
        public DateTime? Time { get; set; }
        public string Positon { get; set; }
        public string Branch { get; set; }
        public RequestCVStatus Status { get; set; }
        public string ApplyLevel { get; set; }
        public string FinalLevel { get; set; }
        public string InterviewLevel { get; set; }
        public string Score { get; set; }
        public string Note { get; set; }
        public string TalentLink { get; set; }
    }
    public class DateInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    public class ExportInput : DateInput
    {
        public UserType? userType { get; set; }
    }
    public class ExportReport: DateInput
    {
        public UserType? userType { get; set; }
        public RequestCVStatus? reqCvStatus { get; set; }
        public RequestCVStatus? FromStatus { get; set; }
        public RequestCVStatus? ToStatus { get; set; }
    }
}
