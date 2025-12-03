using Spa_Management_System.Models;

namespace Spa_Management_System.Services;

public interface IAuthStateService
{
    UserAccount? CurrentUser { get; }
    bool IsAuthenticated { get; }
    event Action? OnAuthStateChanged;
    
    void SetUser(UserAccount user);
    void Logout();
    string GetUserDisplayName();
    string GetUserRole();
}

public class AuthStateService : IAuthStateService
{
    private UserAccount? _currentUser;

    public UserAccount? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public event Action? OnAuthStateChanged;

    public void SetUser(UserAccount user)
    {
        _currentUser = user;
        OnAuthStateChanged?.Invoke();
    }

    public void Logout()
    {
        _currentUser = null;
        OnAuthStateChanged?.Invoke();
    }

    public string GetUserDisplayName()
    {
        if (_currentUser?.Employee?.Person != null)
        {
            return $"{_currentUser.Employee.Person.FirstName} {_currentUser.Employee.Person.LastName}";
        }
        return _currentUser?.Username ?? "Guest";
    }

    public string GetUserRole()
    {
        return _currentUser?.Employee?.Role?.Name ?? "Unknown";
    }
}
