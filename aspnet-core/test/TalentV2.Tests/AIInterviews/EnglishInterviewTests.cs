using Abp.Dependency;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TalentV2.Authorization.Users;
using TalentV2.Authorization;
using TalentV2.APIs;
using TalentV2.Constants.Enum;
using TalentV2.DomainServices.AIInterviews;
using TalentV2.DomainServices.AIInterviews.Dtos;
using TalentV2.Entities;
using TalentV2.EntityFrameworkCore;
using TalentV2.MultiTenancy;
using TalentV2.NccCore;
using TalentV2.WebServices.ExternalServices.Komu;
using Xunit;

namespace TalentV2.Tests.AIInterviews
{
    [CollectionDefinition("EnglishInterview", DisableParallelization = true)]
    public class EnglishInterviewCollection { }

    [Collection("EnglishInterview")]
    public class EnglishInterviewTests : IDisposable
    {
        private readonly InterviewTestContext _context;
        private readonly AIInterviewManager _manager;
        private readonly RecordingHandler _handler = new RecordingHandler();
        private readonly HttpClient _client;
        private readonly TalentV2DbContext _applicationContext;
        private readonly IocManager _previousIocManager;
        private readonly IocManager _ioc = new IocManager();

        public EnglishInterviewTests()
        {
            _previousIocManager = IocManager.Instance;
            SetGlobalIoc(_ioc);
            _context = new InterviewTestContext(new DbContextOptionsBuilder<InterviewTestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var scope = Substitute.For<IWorkScope>();
            scope.GetAll<AIInterviewResult>().Returns(_context.Set<AIInterviewResult>());
            scope.GetAll<CV>().Returns(_context.Set<CV>());
            scope.GetAll<RequestCV>().Returns(_context.Set<RequestCV>());
            scope.InsertAndGetIdAsync(Arg.Any<AIInterviewResult>()).Returns(async call =>
            {
                var result = call.Arg<AIInterviewResult>();
                _context.Add(result);
                await _context.SaveChangesAsync();
                return result.Id;
            });

            _applicationContext = new TalentV2DbContext(new DbContextOptionsBuilder<TalentV2DbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _ioc.IocContainer.Register(
                Component.For<IWorkScope>().Instance(scope),
                Component.For<IAbpSession>().Instance(NullAbpSession.Instance),
                Component.For<TalentV2DbContext>().Instance(_applicationContext),
                Component.For<TenantManager>().Instance(new TenantManager(null, null, null, null)));

            _client = new HttpClient(_handler) { BaseAddress = new Uri("http://komu.test/") };
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["App:ClientRootAddress"] = "http://talent.test/"
            }).Build();
            var komu = new KomuService(_client, config, NullLogger<KomuService>.Instance, NullAbpSession.Instance);
            _manager = new AIInterviewManager(komu, config, NullLogger<AIInterviewManager>.Instance);
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(Substitute.For<IActiveUnitOfWork>());
            _manager.UnitOfWorkManager = unitOfWorkManager;
        }

        [Theory]
        [InlineData(UserType.Staff, "staff-list")]
        [InlineData(UserType.Intern, "intern-list")]
        public async Task Saves_result_and_sends_only_short_DMs_with_EnglishInterview_link(UserType userType, string route)
        {
            await SeedCandidate(userType);
            var result = await _manager.ReceiveResult(Payload());

            result.Message.ShouldBe("Saved");
            var saved = await _context.Set<AIInterviewResult>().SingleAsync();
            saved.CVId.ShouldBe(10);
            saved.ProcessStatus.ShouldBe(AIInterviewProcessStatus.Matched);
            JObject.Parse(saved.Report).ContainsKey("words_breakdown").ShouldBeFalse();
            var request = await _context.Set<RequestCV>().SingleAsync();
            request.Interviewed.ShouldBe(true);
            request.Status.ShouldBe(RequestCVStatus.ScheduledInterview);
            request.Percentage.ShouldBe("85");
            _handler.Messages.Count.ShouldBe(2);
            _handler.Messages[0]["userId"].Value<string>().ShouldBe("1942867974385831936");
            _handler.Messages[0]["username"].ShouldBeNull();
            _handler.Messages[1]["username"].Value<string>().ShouldBe("hr");
            var hrMessage = _handler.Messages[1]["message"].Value<string>();
            hrMessage.ShouldContain($"{route}/10?userType={(int)userType}&tab=5");
            hrMessage.ShouldNotContain("Detailed assessment");

            var duplicate = await _manager.ReceiveResult(Payload());
            duplicate.ResultId.ShouldBe(result.ResultId);
            _handler.Messages.Count.ShouldBe(2);
            (await _context.Set<AIInterviewResult>().CountAsync()).ShouldBe(1);
        }

        [Fact]
        public async Task Failed_candidate_DM_keeps_result_readable_and_still_notifies_HR()
        {
            await SeedCandidate(UserType.Staff);
            _handler.CandidateStatus = HttpStatusCode.BadRequest;

            var response = await _manager.ReceiveResult(Payload());

            response.Success.ShouldBeTrue();
            _handler.Messages.Count.ShouldBe(2);
            var saved = await _context.Set<AIInterviewResult>().SingleAsync();
            saved.ProcessStatus.ShouldBe(AIInterviewProcessStatus.NotifyFailed);
            saved.Note.ShouldContain("Candidate Komu DM");
            var results = await _manager.GetResultsByCVId(10);
            results.Single().OverallScore.ShouldBe(0);
            results.Single().Report["key_strengths"][0].Value<string>().ShouldBe("Detailed assessment");
        }

        [Fact]
        public async Task Read_returns_only_requested_candidate_sessions_newest_first()
        {
            _context.AddRange(
                new AIInterviewResult { CVId = 10, SessionId = "old", InterviewedAt = new DateTime(2026, 11, 1) },
                new AIInterviewResult { CVId = 10, SessionId = "new", InterviewedAt = new DateTime(2026, 11, 10) },
                new AIInterviewResult { CVId = 20, SessionId = "other", InterviewedAt = new DateTime(2026, 11, 11) });
            await _context.SaveChangesAsync();

            var results = await _manager.GetResultsByCVId(10);

            results.Select(x => x.SessionId).ShouldBe(new[] { "new", "old" });
            (await _manager.GetResultsByCVId(99)).ShouldBeEmpty();
        }

        [Theory]
        [InlineData(UserType.Staff, PermissionNames.Pages_CandidateStaff_ViewDetail_PersonInfo_View)]
        [InlineData(UserType.Intern, PermissionNames.Pages_CandidateIntern_ViewDetail_PersonInfo_View)]
        public async Task Read_API_requires_permission_for_actual_candidate_type(UserType userType, string permission)
        {
            await SeedCandidate(userType);
            var checker = Substitute.For<IPermissionChecker>();
            var service = new EnglishInterviewAppService(_manager) { PermissionChecker = checker };

            await Should.ThrowAsync<AbpAuthorizationException>(() => service.GetByCVId(10));
            await checker.Received().IsGrantedAsync(permission);

            checker.IsGrantedAsync(permission).Returns(true);
            (await service.GetByCVId(10)).ShouldBeEmpty();
        }

        private async Task SeedCandidate(UserType userType)
        {
            _context.Add(new User
            {
                Id = 1, Name = "HR", EmailAddress = "hr@example.test", UserName = "hr",
                NormalizedEmailAddress = "HR@EXAMPLE.TEST", NormalizedUserName = "HR",
                Surname = "Test", Password = "test"
            });
            _context.Add(new CV { Id = 10, Name = "Candidate", Email = "Candidate@example.test", UserType = userType, CreatorUserId = 1 });
            _context.Add(new RequestCV { Id = 100, CVId = 10, Status = RequestCVStatus.ScheduledInterview, Percentage = "85" });
            await _context.SaveChangesAsync();
        }

        private static AIInterviewWebhookDto Payload() => new AIInterviewWebhookDto
        {
            CandidateEmail = "candidate@example.test",
            UserId = "1942867974385831936",
            SessionId = "session-1",
            ScenarioTitle = "English interview",
            Report = JObject.Parse("{\"overall_score\":0,\"cefr_level\":\"B1\",\"key_strengths\":[\"Detailed assessment\"],\"words_breakdown\":[{}]}")
        };

        public void Dispose()
        {
            SetGlobalIoc(_previousIocManager);
            _ioc.Dispose();
            _client.Dispose();
            _context.Dispose();
            _applicationContext.Dispose();
        }

        // BaseManager/BaseWebService resolve through this singleton. Isolate it
        // for these non-parallel tests without changing the application's IOC.
        private static void SetGlobalIoc(IocManager ioc) =>
            typeof(IocManager).GetProperty(nameof(IocManager.Instance)).SetValue(null, ioc);

        private class RecordingHandler : HttpMessageHandler
        {
            public List<JObject> Messages { get; } = new List<JObject>();
            public HttpStatusCode CandidateStatus { get; set; } = HttpStatusCode.OK;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.RequestUri.AbsolutePath.ShouldBe("/sendMessageToUser");
                var payload = JObject.Parse(await request.Content.ReadAsStringAsync());
                Messages.Add(payload);
                return new HttpResponseMessage(payload["userId"] != null ? CandidateStatus : HttpStatusCode.OK);
            }
        }

        // Limit the test model to this flow; the application's existing test model
        // includes PostgreSQL-specific properties unsupported by InMemory.
        private class InterviewTestContext : DbContext
        {
            public InterviewTestContext(DbContextOptions<InterviewTestContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                foreach (var type in new[] { typeof(CV), typeof(RequestCV), typeof(AIInterviewResult), typeof(User) })
                {
                    var entity = builder.Entity(type);
                    foreach (var property in type.GetProperties().Where(p => !p.PropertyType.IsValueType && p.PropertyType != typeof(string)))
                        entity.Ignore(property.Name);
                }
                builder.Entity<CV>().HasOne(x => x.CreatorUser).WithMany().HasForeignKey(x => x.CreatorUserId);
                builder.Entity<CV>().HasOne(x => x.LastModifierUser).WithMany().HasForeignKey(x => x.LastModifierUserId);
            }
        }
    }
}
