using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using TalentV2.Authorization.Roles;
using TalentV2.Authorization.Users;
using TalentV2.MultiTenancy;
using TalentV2.Entities;
using TalentV2.Entities.NccCVs;

namespace TalentV2.EntityFrameworkCore
{
    public class TalentV2DbContext : AbpZeroDbContext<Tenant, Role, User, TalentV2DbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Capability> Capabilities { get; set; }
        public DbSet<CapabilitySetting> CapabilitySettings { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<ExternalCV> ExternalCVs { get; set; }
        public DbSet<CVEducation> CVEducations { get; set; }
        public DbSet<CVSkill> CVSkills { get; set; }
        public DbSet<CVSource> CVSources { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<EducationType> EducationsTypes { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestCV> RequestCVs { get; set; }
        public DbSet<RequestCVCapabilityResult> RequestCVCapabilityResults { get; set; }
        public DbSet<RequestCVInterview> RequestCVInterviews { get; set; }
        public DbSet<RequestCVStatusHistory> RequestCVStatusHistories { get; set; }
        public DbSet<RequestSkill> RequestSkills { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailStatusHistory> EmailStatusHistories { get; set; }
        public DbSet<SubPosition> SubPositions { get; set; }
        public DbSet<PositionSetting> PositionSettings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<RequestCVStatusChangeHistory> RequestCVStatusChangeHistories { get; set; }
        public DbSet<ApplyCV> ApplyCVs { get; set; }
        public DbSet<ScoreSetting> ScoreSettings { get; set; }
        public DbSet<ScoreRange> ScoreRanges { get; set; }

        #region Ncc CVs
        public DbSet<EmployeeEducation> EmployeeEducations { get; set; }
        public DbSet<EmployeeLanguage> EmployeeLanguages { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<EmployeeWorkingExpAndTechnologies> EmployeeWorkingExpAndTechnologies { get; set; }
        public DbSet<EmployeeWorkingExperience> EmployeeWorkingExperiences { get; set; }
        public DbSet<GroupSkill> GroupSkills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTechnology> ProjectTechnologies { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<Versions> Versions { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }
        public DbSet<FakeSkill> FakeSkills { get; set; }
        public DbSet<CandidateLanguage> CandidateLanguages { get; set; }
        #endregion
        public TalentV2DbContext(DbContextOptions<TalentV2DbContext> options)
            : base(options)
        {
        }
    }
}
