using Abp.Authorization.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TalentV2.APIs.NccCVs.MyProfile.Dto
{
    public class UpdateUserGeneralInfoDto
    {
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string EmailAddressInCV { get; set; }

        [Required]
        public long CurrentPositionId { get; set; }
        public long UserId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public long BranchId { get; set; }

        public IFormFile File { get; set; }
        public string Path { get; set; }

    }
}
