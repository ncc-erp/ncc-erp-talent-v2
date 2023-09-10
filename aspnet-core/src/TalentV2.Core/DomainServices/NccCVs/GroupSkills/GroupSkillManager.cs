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
using TalentV2.DomainServices.NccCVs.GroupSkills.Dtos;

namespace TalentV2.DomainServices.NccCVs.GroupSkills
{
    public class GroupSkillManager : BaseManager, IGroupSkillManager
    {
        public IQueryable<GroupSkillDto> IQGetAll()
        {
            var groupSkills = from gs in WorkScope.GetAll<GroupSkill>()
                              orderby gs.LastModificationTime descending
                              select new GroupSkillDto
                              {
                                  Id = gs.Id,
                                  Name = gs.Name,
                                  Default = gs.Default
                              };
            return groupSkills;
        }
        public async Task<GroupSkillDto> Create(GroupSkillDto input)
        {
            await CheckDuplicateNameCategory<GroupSkill>(input.Name);

            var groupSkill = ObjectMapper.Map<GroupSkill>(input);
            var id = await WorkScope.InsertAndGetIdAsync<GroupSkill>(groupSkill);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<GroupSkillDto> Update(GroupSkillDto input)
        {
            await CheckDuplicateNameCategory<GroupSkill>(input.Name, input.Id);
            var groupSkill = await WorkScope.GetAsync<GroupSkill>(input.Id);
            ObjectMapper.Map<GroupSkillDto, GroupSkill>(input, groupSkill);
            await WorkScope.UpdateAsync(groupSkill);

            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                    .Where(q => q.Id == groupSkill.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task Delete(long Id)
        {
            var isExisted = await WorkScope.GetAll<GroupSkill>()
            .Where(q => q.Id == Id)
            .Where(q => q.Skills.Any(s => !s.IsDeleted))
            .AnyAsync();
            if (isExisted)
                throw new UserFriendlyException($"Not Deleted, Group Skill already exists somewhere");
            await WorkScope.DeleteAsync<GroupSkill>(Id);
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
