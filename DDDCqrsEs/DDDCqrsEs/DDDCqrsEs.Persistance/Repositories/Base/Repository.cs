using DDDCqrsEs.Application.Repositories.Base;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Persistance.Repositories.Base
{
    [MapServiceDependency(nameof(Repository<T>))]
    public class Repository<T> : IRepository<T> where T:class, IEntity
    {
        protected ToDoDbContext ToDoDbContext;

        public Repository(ToDoDbContext toDoContext)
        {
            ToDoDbContext = toDoContext;
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> whereFilter = null) 
        {
            var query = ToDoDbContext.Set<T>();
            if (whereFilter == null) return query;

            return query.Where(whereFilter);
        }

        public int SaveChanges()
        {
            return ToDoDbContext.SaveChanges();
        }

        public void Add(T entity, bool saveImmediately = false) 
        {
            ToDoDbContext.Add(entity);

            if (saveImmediately)
            {
                SaveChanges();
            }
        }

        public async Task AddAsync(T entity, bool saveImmediately = false, CancellationToken cancellationToken = default) 
        {
            ToDoDbContext.Add(entity);

            if (saveImmediately)
            {
                await ToDoDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public void Remove(T entity) 
        {
            ToDoDbContext.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities) 
        {
            ToDoDbContext.RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await ToDoDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
