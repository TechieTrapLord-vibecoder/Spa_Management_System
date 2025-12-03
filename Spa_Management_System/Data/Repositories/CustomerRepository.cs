using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Models;

namespace Spa_Management_System.Data.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByCustomerCodeAsync(string customerCode);
    Task<Customer?> GetWithPersonAsync(long customerId);
    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<IEnumerable<Customer>> GetAllWithArchivedAsync();
}

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Person)
            .Where(c => !c.IsArchived)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet
            .Include(c => c.Person)
            .Where(c => !c.IsArchived)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllWithArchivedAsync()
    {
        return await _dbSet
            .Include(c => c.Person)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Customer?> GetByCustomerCodeAsync(string customerCode)
    {
        return await _dbSet
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);
    }

    public async Task<Customer?> GetWithPersonAsync(long customerId)
    {
        return await _dbSet
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _dbSet
            .Include(c => c.Person)
            .Where(c => c.Person.FirstName.Contains(searchTerm) ||
                       c.Person.LastName.Contains(searchTerm) ||
                       c.Person.Email != null && c.Person.Email.Contains(searchTerm) ||
                       c.CustomerCode != null && c.CustomerCode.Contains(searchTerm))
            .ToListAsync();
    }
}
