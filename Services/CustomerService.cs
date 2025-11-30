using Spa_Management_System.Data.Repositories;
using Spa_Management_System.Models;

namespace Spa_Management_System.Services;

public interface ICustomerService
{
    Task<Customer?> GetCustomerByIdAsync(long customerId);
    Task<Customer?> GetCustomerByCodeAsync(string customerCode);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<IEnumerable<Customer>> GetAllWithArchivedAsync();
    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    Task<Customer> CreateCustomerAsync(string firstName, string lastName, string? email, string? phone, string? address);
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task<bool> DeleteCustomerAsync(long customerId);
    Task<bool> AddLoyaltyPointsAsync(long customerId, int points);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRepository<Person> _personRepository;

    public CustomerService(ICustomerRepository customerRepository, IRepository<Person> personRepository)
    {
        _customerRepository = customerRepository;
        _personRepository = personRepository;
    }

    public async Task<Customer?> GetCustomerByIdAsync(long customerId)
    {
        return await _customerRepository.GetWithPersonAsync(customerId);
    }

    public async Task<Customer?> GetCustomerByCodeAsync(string customerCode)
    {
        return await _customerRepository.GetByCustomerCodeAsync(customerCode);
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllWithArchivedAsync()
    {
        return await _customerRepository.GetAllWithArchivedAsync();
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _customerRepository.SearchCustomersAsync(searchTerm);
    }

    public async Task<Customer> CreateCustomerAsync(string firstName, string lastName, string? email, string? phone, string? address)
    {
        // Create Person first
        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Address = address,
            CreatedAt = DateTime.Now
        };

        person = await _personRepository.AddAsync(person);

        // Create Customer
        var customer = new Customer
        {
            PersonId = person.PersonId,
            CustomerCode = GenerateCustomerCode(),
            LoyaltyPoints = 0,
            CreatedAt = DateTime.Now
        };

        return await _customerRepository.AddAsync(customer);
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        return await _customerRepository.UpdateAsync(customer);
    }

    public async Task<bool> DeleteCustomerAsync(long customerId)
    {
        return await _customerRepository.DeleteAsync(customerId);
    }

    public async Task<bool> AddLoyaltyPointsAsync(long customerId, int points)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return false;

        customer.LoyaltyPoints += points;
        await _customerRepository.UpdateAsync(customer);
        return true;
    }

    private string GenerateCustomerCode()
    {
        // Generate a unique customer code (e.g., CUST-20250128-001)
        return $"CUST-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}
