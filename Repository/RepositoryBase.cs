using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using PA.Contracts;
using PA.TOYOTA.DB;

namespace PA.Repository
{
    //MH: 05.04.2022
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
       
        protected ToyotaContext RepositoryContext;

        public RepositoryBase(ToyotaContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
              RepositoryContext.Set<T>()
                .AsNoTracking() :
              RepositoryContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges) =>
            !trackChanges ?
              RepositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking() :
              RepositoryContext.Set<T>()
                .Where(expression);

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);

        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);

        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
    }
}
