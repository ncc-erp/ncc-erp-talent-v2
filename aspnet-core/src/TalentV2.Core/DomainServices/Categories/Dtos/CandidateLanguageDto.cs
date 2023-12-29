using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.Entities;

namespace TalentV2.DomainServices.Categories.Dtos
{
    public class CandidateLanguageDto : CategoryDto
    {
        [ApplySearchAttribute]
        public string ColorCode { get; set; }
        public string Alias { get; set; }

        [AutoMapTo(typeof(CandidateLanguage))]
        public class CreateLanguageDto : CandidateLanguageDto
        {
        }
        [AutoMapTo(typeof(CandidateLanguage))]
        public class UpdateLanguageDto : CandidateLanguageDto
        {
        }
    }
}
