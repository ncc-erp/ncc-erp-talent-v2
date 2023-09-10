using Abp.Authorization.Users;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{ 
    public class UserGeneralInfoDto
    {
        
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddressInCV { get; set; }
        public string ImgPath { get; set; }
        public string CurrentPosition { get; set; }
        public long UserId { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public long? BranchId { get; set; }
        public long? CurrentPositionId { get; set; }
    }
}
