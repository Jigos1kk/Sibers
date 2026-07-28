using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    /// <summary>
    /// Generic base repository providing common CRUD operations for all entities.
    /// Eliminates code duplication across entity-specific repositories.
    /// </summary>
    /// <typeparam name="T">The entity type, must inherit from <see cref="BaseEntity"/>.</typeparam>
    public class BaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id, CancellationToken ct, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id, ct)
                ?? throw new InvalidOperationException($"Not found {typeof(T).Name} with ID {id}");
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken ct, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync(ct);
        }

        public virtual async Task AddAsync(T entity, CancellationToken ct)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public virtual Task UpdateAsync(T entity, CancellationToken ct)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken ct)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}