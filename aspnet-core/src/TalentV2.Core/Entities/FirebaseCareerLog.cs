﻿using Abp.Domain.Entities;

namespace TalentV2.Entities
{
    public class FirebaseCareerLog : Entity<int>
    {
        public string IdFirebase { get; set; }
        public string Status { get; set; }
    }
}
