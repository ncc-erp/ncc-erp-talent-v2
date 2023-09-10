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
using TalentV2.DomainServices.NccCVs.EmployeePositions.Dtos;

namespace TalentV2.DomainServices.NccCVs.EmployeePositions
{
    public class EmployeePositionManager : BaseManager, IEmployeePositionManager
    {
        public IQueryable<EmployeePositionDto> IQGetAll()
        {
            var ePositions = from ep in WorkScope.GetAll<EmployeePosition>()
                           select new EmployeePositionDto
                           {
                               Id = ep.Id,
                               Name = ep.Name,
                               Description = ep.Description
                           };
            return ePositions;
        }
        public async Task<EmployeePositionDto> Create(EmployeePositionDto input)
        {
            await CheckDuplicateNameCategory<EmployeePosition>(input.Name);
            var ePosition = ObjectMapper.Map<EmployeePosition>(input);
            var id = await WorkScope.InsertAndGetIdAsync<EmployeePosition>(ePosition);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<EmployeePositionDto> Update(EmployeePositionDto input)
        {
            await CheckDuplicateNameCategory<EmployeePosition>(input.Name, input.Id);
            var ePosition = await WorkScope.GetAsync<EmployeePosition>(input.Id);
            ObjectMapper.Map<EmployeePositionDto, EmployeePosition>(input, ePosition);
            await WorkScope.UpdateAsync(ePosition);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAll()
                    .Where(q => q.Id == ePosition.Id)
                    .FirstOrDefaultAsync();
        }
        public async Task Delete(long Id)
        {
            await WorkScope.DeleteAsync<EmployeePosition>(Id);
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
