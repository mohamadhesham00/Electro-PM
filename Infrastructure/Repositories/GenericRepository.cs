using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T>(DbContext context) : IGenericRepository<T> where T : class
    {
        // We use generic dbContext beacause we may later want to use this repository with multiple contexts. If we used AppDbContext, we would be tightly coupled to it.
        protected readonly DbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        // EF tracks references automatically. We just alter the state flag to Modified.
        public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
