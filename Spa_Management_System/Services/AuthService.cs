using Spa_Management_System.Data.Repositories;
using Spa_Management_System.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Spa_Management_System.Services;

public interface IAuthService
{
    Task<UserAccount?> AuthenticateAsync(string username, string password);
    (bool IsValid, string ErrorMessage) ValidatePassword(string password);
    Task<UserAccount> CreateUserAccountAsync(long employeeId, string username, string password);
    Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(long userId, string newPassword);
    Task<bool> DeactivateUserAsync(long userId);
    Task<bool> ActivateUserAsync(long userId);
    Task<IEnumerable<UserAccount>> GetAllUsersAsync();
    Task<IEnumerable<UserAccount>> GetAllUsersWithInactiveAsync();
    Task<bool> UsernameExistsAsync(string username);
}

public class AuthService : IAuthService
{
    private readonly IUserAccountRepository _userAccountRepository;

    public AuthService(IUserAccountRepository userAccountRepository)
    {
        _userAccountRepository = userAccountRepository;
    }

    public async Task<UserAccount?> AuthenticateAsync(string username, string password)
    {
        var user = await _userAccountRepository.GetByUsernameAsync(username);
        
        if (user == null || !user.IsActive)
            return null;

        var passwordHash = HashPassword(password);
        
        if (user.PasswordHash != passwordHash)
            return null;

        // Update last login
        user.LastLogin = DateTime.Now;
        await _userAccountRepository.UpdateAsync(user);

        return user;
    }

    public async Task<UserAccount> CreateUserAccountAsync(long employeeId, string username, string password)
    {
        // Validate password requirements
        var (isValid, errorMessage) = ValidatePassword(password);
        if (!isValid)
        {
            throw new Exception(errorMessage);
        }

        // Check if username already exists
        if (await _userAccountRepository.UsernameExistsAsync(username))
        {
            throw new Exception($"Username '{username}' already exists");
        }

        var userAccount = new UserAccount
        {
            EmployeeId = employeeId,
            Username = username,
            PasswordHash = HashPassword(password),
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        return await _userAccountRepository.AddAsync(userAccount);
    }

    public async Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
    {
        // Validate new password requirements
        var (isValid, errorMessage) = ValidatePassword(newPassword);
        if (!isValid)
        {
            throw new Exception(errorMessage);
        }

        var user = await _userAccountRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;

        var currentPasswordHash = HashPassword(currentPassword);
        
        if (user.PasswordHash != currentPasswordHash)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _userAccountRepository.UpdateAsync(user);
        
        return true;
    }

    public async Task<bool> ResetPasswordAsync(long userId, string newPassword)
    {
        // Validate new password requirements
        var (isValid, errorMessage) = ValidatePassword(newPassword);
        if (!isValid)
        {
            throw new Exception(errorMessage);
        }

        var user = await _userAccountRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _userAccountRepository.UpdateAsync(user);
        
        return true;
    }

    public async Task<bool> DeactivateUserAsync(long userId)
    {
        var user = await _userAccountRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;

        user.IsActive = false;
        await _userAccountRepository.UpdateAsync(user);
        
        return true;
    }

    public async Task<bool> ActivateUserAsync(long userId)
    {
        var user = await _userAccountRepository.GetByIdAsync(userId);
        
        if (user == null)
            return false;

        user.IsActive = true;
        await _userAccountRepository.UpdateAsync(user);
        
        return true;
    }

    public async Task<IEnumerable<UserAccount>> GetAllUsersAsync()
    {
        return await _userAccountRepository.GetActiveUsersAsync();
    }

    public async Task<IEnumerable<UserAccount>> GetAllUsersWithInactiveAsync()
    {
        return await _userAccountRepository.GetAllUsersWithInactiveAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userAccountRepository.UsernameExistsAsync(username);
    }

    // Simple password hashing - In production, use BCrypt or PBKDF2
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    // Password validation: 10+ chars, uppercase, lowercase, number, special character
    public (bool IsValid, string ErrorMessage) ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "Password is required.");

        var errors = new List<string>();

        if (password.Length < 10)
            errors.Add("at least 10 characters");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("one uppercase letter");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("one lowercase letter");

        if (!Regex.IsMatch(password, @"[0-9]"))
            errors.Add("one number");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            errors.Add("one special character (!@#$%^&*()_+-=[]{}|;':\",./<>?)");

        if (errors.Count > 0)
            return (false, $"Password must contain {string.Join(", ", errors)}.");

        return (true, string.Empty);
    }
}
