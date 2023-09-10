using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using TalentV2.Entities;
using TalentV2.Entities.NccCVs;

namespace TalentV2.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "PasswordUserabc";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(500)]
        public string AvatarPath { get; set; }
        [MaxLength(500)]
        public string Certificates { get; set; }
        [MaxLength(500)]
        public string PersonalAttribute { get; set; }
        public long? BranchId { get; set; }
        public long? PositionId { get; set; }
        public string SignatureContact { get; set; }
        [DefaultValue(false)]
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }
        [ForeignKey(nameof(PositionId))]
        public EmployeePosition Position { get; set; }

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        public ICollection<Versions> Versions { get; set; }
        public ICollection<RequestCVInterview> RequestCVInterviews { get; set; }
    }
}
