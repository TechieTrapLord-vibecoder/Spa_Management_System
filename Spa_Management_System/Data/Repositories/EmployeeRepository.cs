using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Models;

namespace Spa_Management_System.Data.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetWithDetailsAsync(long employeeId);
    Task<IEnumerable<Employee>> GetAllWithDetailsAsync();
    Task<IEnumerable<Employee>> GetByRoleAsync(short roleId);
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
}

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetWithDetailsAsync(long employeeId)
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.Role)
            .Include(e => e.UserAccounts)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<IEnumerable<Employee>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.Role)
            .OrderBy(e => e.Person.LastName)
            .ThenBy(e => e.Person.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByRoleAsync(short roleId)
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.Role)
            .Where(e => e.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.Role)
            .Where(e => e.Status == "active")
            .OrderBy(e => e.Person.LastName)
            .ToListAsync();
    }
}
