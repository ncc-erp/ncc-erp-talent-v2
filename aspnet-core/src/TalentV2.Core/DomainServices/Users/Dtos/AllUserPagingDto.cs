using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.DomainServices.Users.Dtos
{
    public class AllUserPagingDto
    {
        public long Id { get; set; }
        [ApplySearchAttribute]
        public string UserName { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string Surname { get; set; }
        public string FullName
        {
            get => Surname + " " + Name;
        }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public long? BranchId { get; set; }
        public string BranchName { get; set; }
        public string[] RoleNames { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
}
