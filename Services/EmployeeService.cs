using Spa_Management_System.Data.Repositories;
using Spa_Management_System.Models;
using System.Security.Cryptography;
using System.Text;

namespace Spa_Management_System.Services;

public interface IEmployeeService
{
    Task<Employee?> GetEmployeeByIdAsync(long employeeId);
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
    Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(short roleId);
    Task<Employee> CreateEmployeeAsync(string firstName, string lastName, string? email, string? phone, short roleId, DateTime? hireDate, string? note);
    Task<Employee> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeactivateEmployeeAsync(long employeeId);
    Task<bool> DeleteEmployeeAsync(long employeeId);
}

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRepository<Person> _personRepository;

    public EmployeeService(IEmployeeRepository employeeRepository, IRepository<Person> personRepository)
    {
        _employeeRepository = employeeRepository;
        _personRepository = personRepository;
    }

    public async Task<Employee?> GetEmployeeByIdAsync(long employeeId)
    {
        return await _employeeRepository.GetWithDetailsAsync(employeeId);
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _employeeRepository.GetAllWithDetailsAsync();
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _employeeRepository.GetActiveEmployeesAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(short roleId)
    {
        return await _employeeRepository.GetByRoleAsync(roleId);
    }

    public async Task<Employee> CreateEmployeeAsync(string firstName, string lastName, string? email, string? phone, short roleId, DateTime? hireDate, string? note)
    {
        // Create Person first
        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            CreatedAt = DateTime.Now
        };

        person = await _personRepository.AddAsync(person);

        // Create Employee
        var employee = new Employee
        {
            PersonId = person.PersonId,
            RoleId = roleId,
            HireDate = hireDate ?? DateTime.Now,
            Status = "active",
            Note = note,
            CreatedAt = DateTime.Now
        };

        return await _employeeRepository.AddAsync(employee);
    }

    public async Task<Employee> UpdateEmployeeAsync(Employee employee)
    {
        return await _employeeRepository.UpdateAsync(employee);
    }

    public async Task<bool> DeactivateEmployeeAsync(long employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return false;

        employee.Status = "inactive";
        await _employeeRepository.UpdateAsync(employee);
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(long employeeId)
    {
        return await _employeeRepository.DeleteAsync(employeeId);
    }
}
