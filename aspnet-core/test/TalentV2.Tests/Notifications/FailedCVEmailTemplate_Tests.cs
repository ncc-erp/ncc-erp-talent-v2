using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NSubstitute;
using Shouldly;
using TalentV2.Constants.Enum;
using TalentV2.Entities;
using TalentV2.EntityFrameworkCore;
using TalentV2.EntityFrameworkCore.Seed.Emails;
using TalentV2.NccCore;
using TalentV2.Notifications.Mail;
using Xunit;

namespace TalentV2.Tests.Notifications
{
    public class FailedCVEmailTemplate_Tests
    {
        private const int TestTenantId = 99;

        [Fact]
        public void Seed_should_migrate_legacy_failed_cv_and_create_staff_and_intern_templates()
        {
            using (var context = CreateDbContext())
            {
                var legacyTemplate = new EmailTemplate
                {
                    TenantId = TestTenantId,
                    Type = MailFuncEnum.FailedCV,
                    Name = "[Failed CV] Thank-you mail",
                    Subject = "Legacy subject",
                    Description = "Legacy description",
                    BodyMessage = "Legacy body"
                };
                context.EmailTemplates.Add(legacyTemplate);
                context.SaveChanges();

                new DefaultEmailSettingsCreator(context, TestTenantId).Create();

                var templates = context.EmailTemplates
                    .IgnoreQueryFilters()
                    .Where(x => x.TenantId == TestTenantId && x.Type == MailFuncEnum.FailedCV)
                    .ToList();

                templates.Count.ShouldBe(2);
                templates.Select(x => x.Version).ShouldBe(new[] { "Staff", "Intern" }, ignoreOrder: true);
                templates.Single(x => x.Id == legacyTemplate.Id).Version.ShouldBe("Staff");
                templates.Single(x => x.Id == legacyTemplate.Id).Name
                    .ShouldBe("[Failed CV Staff] Thank-you mail");
                templates.Single(x => x.Id == legacyTemplate.Id).Subject.ShouldBe("Legacy subject");
            }
        }

        [Theory]
        [InlineData(UserType.Staff, "Staff", "Staff subject", 101)]
        [InlineData(UserType.Intern, "Intern", "Intern subject", 102)]
        public async Task GetContentMailCV_should_select_template_by_candidate_user_type(
            UserType userType,
            string expectedVersion,
            string expectedSubject,
            long expectedTemplateId)
        {
            var candidate = new CV
            {
                Id = 42,
                Name = "Candidate",
                Email = "candidate@example.com",
                Phone = "0123456789",
                UserType = userType,
                Branch = new Branch { Address = "Ha Noi" }
            };
            var templates = new[]
            {
                CreateFailedCVTemplate(101, "Staff", "Staff subject"),
                CreateFailedCVTemplate(102, "Intern", "Intern subject")
            };
            var workScope = Substitute.For<IWorkScope>();
            workScope.GetAll<CV>().Returns(candidate.ToAsyncQueryable());
            workScope.GetAll<EmailTemplate>()
                .Returns(((IEnumerable<EmailTemplate>)templates).ToAsyncQueryable());
            workScope.GetAll<TalentV2.Authorization.Users.User>()
                .Returns(Array.Empty<TalentV2.Authorization.Users.User>().AsQueryable());

            var mailService = new MailService(
                Substitute.For<IEmailSender>(),
                workScope,
                Substitute.For<IAbpSession>());

            var result = await mailService.GetContentMailCV(candidate.Id);

            templates.Single(x => x.Id == result.TemplateId).Version.ShouldBe(expectedVersion);
            result.TemplateId.ShouldBe(expectedTemplateId);
            result.Subject.ShouldBe(expectedSubject);
        }

        private static EmailTemplate CreateFailedCVTemplate(long id, string version, string subject)
        {
            return new EmailTemplate
            {
                Id = id,
                Type = MailFuncEnum.FailedCV,
                Version = version,
                Name = $"Failed CV {version}",
                Subject = subject,
                Description = $"Failed CV {version} template",
                BodyMessage = "Dear {{FullName}}"
            };
        }

        private static TalentV2DbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TalentV2DbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FailedCVTestDbContext(options)
            {
                SuppressAutoSetTenantId = true
            };
        }

        private sealed class FailedCVTestDbContext : TalentV2DbContext
        {
            public FailedCVTestDbContext(DbContextOptions<TalentV2DbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<MezonWebhook>().Ignore(x => x.Functions);
            }
        }
    }

    internal static class AsyncQueryableTestExtensions
    {
        public static IQueryable<T> ToAsyncQueryable<T>(this T item)
        {
            return new TestAsyncEnumerable<T>(new[] { item });
        }

        public static IQueryable<T> ToAsyncQueryable<T>(this IEnumerable<T> items)
        {
            return new TestAsyncEnumerable<T>(items);
        }
    }

    internal sealed class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments().Single();
            var result = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })
                .MakeGenericMethod(resultType)
                .Invoke(_inner, new object[] { expression });

            return (TResult)typeof(Task)
                .GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { result });
        }
    }

    internal sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    internal sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
    }
}
