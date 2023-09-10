using Abp.UI;
using TalentV2.DomainServices.Categories.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;
using TalentV2.Entities.NccCVs;
using TalentV2.DomainServices.NccCVs.FakeSkills.Dtos;

namespace TalentV2.DomainServices.NccCVs.FakeSkills
{
    public class FakeSkillManager : BaseManager, IFakeSkillManager
    {
        public IQueryable<FakeSkillDto> IQGetAll()
        {
            var fakeSkills = from fs in WorkScope.GetAll<FakeSkill>()
                           select new FakeSkillDto
                           {
                               Id = fs.Id,
                               Name = fs.Name,
                               GroupSkillId = fs.GroupSkillId,
                               GroupSkillName = fs.GroupSkill.Name,
                               Description = fs.Description
                           };
            return fakeSkills;
        }
        public async Task<FakeSkillDto> Create(FakeSkillDto input)
        {
            await CheckDuplicateNameCategory<FakeSkill>(input.Name);
            var fakeSkill = ObjectMapper.Map<FakeSkill>(input);
            var id = await WorkScope.InsertAndGetIdAsync<FakeSkill>(fakeSkill);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<FakeSkillDto> Update(FakeSkillDto input)
        {
            await CheckDuplicateNameCategory<FakeSkill>(input.Name, input.Id);
            var fakeSkill = await WorkScope.GetAsync<FakeSkill>(input.Id);
            ObjectMapper.Map<FakeSkillDto, FakeSkill>(input, fakeSkill);
            await WorkScope.UpdateAsync(fakeSkill);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                    .Where(q => q.Id == fakeSkill.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task Delete(long Id)
        {
            await WorkScope.DeleteAsync<FakeSkill>(Id);
        }

        private async Task CheckDuplicateNameCategory<IEntity>(string name, long id = default)
           where IEntity : class, IEntity<long>
        {
            name = name.Trim();
            var query = WorkScope.GetAll<IEntity>();
            var param = Expression.Parameter(typeof(IEntity), "x");
            var value = Expression.Property(param, "Name");
            MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            var constant = Expression.Constant(name);
            var body = Expression.Call(value, equalsMethod, constant);
            var exp = Expression.Lambda<Func<IEntity, bool>>(body, param);
            query = query.Where(exp);
            if (id != default)
            {
                var idProperty = Expression.Property(param, "Id");
                var constantId = Expression.Constant(id);
                Expression notEqual = Expression.NotEqual(param, Expression.Constant(null));
                notEqual = Expression.AndAlso(notEqual, Expression.NotEqual(idProperty, constantId));
                var exp2 = Expression.Lambda<Func<IEntity, bool>>(notEqual, param);
                query = query.Where(exp2);
            }
            if (await query.AnyAsync())
            {
                throw new UserFriendlyException($"{name} existed");
            }
        }

    }
}
