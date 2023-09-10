using Abp.Auditing;
using Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.WebServices.InternalServices.LMS.Dtos
{
    public class StudentDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
}
