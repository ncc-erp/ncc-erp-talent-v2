﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.DomainServices.Candidates.Dtos
{
    public class Candidate
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Branch { get; set; }
        public string Education { get; set; }
        public string ApplyLevel { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public string CV { get; set; }
    }
    public class InterView
    {
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime? Time { get; set; }
        public string Positon { get; set; }
        public string Branch { get; set; }
        public RequestCVStatus Status { get; set; }
        public string ApplyLevel { get; set; }
        public string FinalLevel { get; set; }
        public string Reason { get; set; }
    }
    public class OnBoard
    {
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime? Time { get; set; }
        public string Positon { get; set; }
        public string Branch { get; set; }
        public RequestCVStatus Status { get; set; }
        public string ApplyLevel { get; set; }
        public string FinalLevel { get; set; }
        public string Reason { get; set; }
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
}
