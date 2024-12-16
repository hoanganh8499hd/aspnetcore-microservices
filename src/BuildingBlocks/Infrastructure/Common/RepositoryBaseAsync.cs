using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Infrastructure.Common
{

    public class RepositoryQueryBase<T, K, TContext> : IRepositoryQueryBase<T, K, TContext>
        where T : EntityBase<K>
        where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public RepositoryQueryBase(TContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<T> FindAll(bool trackChanges = false) =>
            !trackChanges ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindAll(trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            return !trackChanges ? _dbContext.Set<T>().Where(expression).AsNoTracking() : _dbContext.Set<T>().Where(expression);
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var items = FindByCondition(expression, trackChanges);
            items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
            return items;
        }

        public async Task<T?> GetByIdAsync(K id) =>
            await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties) =>
            await FindByCondition(x => x.Id.Equals(id), trackChanges: false, includeProperties).FirstOrDefaultAsync();
    }

    public class RepositoryBaseAsync<T, K, TContext> : RepositoryQueryBase<T, K, TContext>, IRepositoryBaseAsync<T, K, TContext>
         where T : EntityBase<K>
         where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly IUnitOfWork<TContext> _unitOfWork;

        public RepositoryBaseAsync(TContext dbContext, IUnitOfWork<TContext> unitOfWork) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
        }

        public Task<IDbContextTransaction> BeginTransactionAsync() =>
            _dbContext.Database.BeginTransactionAsync();

        public async Task EndTransactionAsync()
        {
            await SaveChangeAsync();
            await _dbContext.Database.CommitTransactionAsync();
        }

        public Task RollbackTransactionAsync() =>
            _dbContext.Database.RollbackTransactionAsync();

        public async Task<K> CreateAsync(T entity, bool isSaveChange = false)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            if (isSaveChange) await SaveChangeAsync();
            return entity.Id;
        }

        public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities, bool isSaveChange = false)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            if (isSaveChange) await SaveChangeAsync();
            return entities.Select(x => x.Id).ToList();
        }

        public async Task UpdateAsync(T entity, bool isSaveChange = false)
        {
            if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;

            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            if (isSaveChange) await SaveChangeAsync();
        }

        public async Task UpdateListAsync(IEnumerable<T> entities, bool isSaveChange = false)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            if (isSaveChange) await SaveChangeAsync();
        }


        public async Task DeleteAsync(T entity, bool isSaveChange = false)
        {
            _dbContext.Set<T>().Remove(entity);
            if (isSaveChange) await SaveChangeAsync();
        }

        public async Task DeleteListAsync(IEnumerable<T> entities, bool isSaveChange = false)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            if (isSaveChange) await SaveChangeAsync();
        }

        public Task<int> SaveChangeAsync() => _unitOfWork.CommitAsync();
    }
}
