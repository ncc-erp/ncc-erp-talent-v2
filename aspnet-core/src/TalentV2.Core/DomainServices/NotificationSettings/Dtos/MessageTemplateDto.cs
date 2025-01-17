using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;
using TalentV2.Constants.Enum.NccCVs;
using TalentV2.Entities;

namespace TalentV2.Notifications.Message.Dtos
{
    public class MessageTemplateDto
    {
        public NotificationType Type { get; set; }
        public string BodyMessage { get; set; }
        public string[] PropertiesSupport { get; set; }
    }

    public class ResultMessageTemplate<T> where T : class
    {
        public T Result { get; set; }
        public string[] PropertiesSupport { get => typeof(T).GetProperties().Select(s => s.Name).ToArray(); }
    }
}
