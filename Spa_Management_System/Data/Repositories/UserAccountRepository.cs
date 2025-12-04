using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Models;

namespace Spa_Management_System.Data.Repositories;

public interface IUserAccountRepository : IRepository<UserAccount>
{
    Task<UserAccount?> GetByUsernameAsync(string username);
    Task<UserAccount?> GetWithEmployeeDetailsAsync(long userId);
    Task<IEnumerable<UserAccount>> GetActiveUsersAsync();
    Task<IEnumerable<UserAccount>> GetAllUsersWithInactiveAsync();
    Task<bool> UsernameExistsAsync(string username);
}

public class UserAccountRepository : Repository<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UserAccount?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Person)
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserAccount?> GetWithEmployeeDetailsAsync(long userId)
    {
        return await _dbSet
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Person)
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<IEnumerable<UserAccount>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Person)
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Role)
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAccount>> GetAllUsersWithInactiveAsync()
    {
        return await _dbSet
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Person)
            .Include(u => u.Employee)
                .ThenInclude(e => e!.Role)
            .ToListAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
}
