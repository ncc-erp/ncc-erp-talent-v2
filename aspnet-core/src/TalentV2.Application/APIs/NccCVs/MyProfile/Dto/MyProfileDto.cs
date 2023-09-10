using System;
using System.Collections.Generic;
using System.Text;
using TalentV2.Constants.Enum.NccCVs;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{
    public class MyProfileDto
    {
        public bool isHiddenYear { get; set; }
        public AttachmentTypeEnum typeOffile { get; set; }
        public UserGeneralInfoDto EmployeeInfo { get; set; }
        public IEnumerable<EmployeeEducationDto> EducationBackGround { get; set; }
        public TechnicalExpertiseDto TechnicalExpertises { get; set; }
        public PersonalAttributeDto PersonalAttributes { get; set; }
        public IEnumerable<WorkingExperienceDto> WorkingExperiences { get; set; }
    }
}
