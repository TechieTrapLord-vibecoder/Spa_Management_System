using System.Linq.Expressions;

namespace Spa_Management_System.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<int> CountAsync();
    Task<int> SaveChangesAsync();
}
