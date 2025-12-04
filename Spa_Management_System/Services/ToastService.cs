namespace Spa_Management_System.Services;

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    event Action<Guid>? OnHide;
    
    void ShowSuccess(string message, string? title = null, int duration = 3500);
    void ShowError(string message, string? title = null, int duration = 5000);
    void ShowWarning(string message, string? title = null, int duration = 4000);
    void ShowInfo(string message, string? title = null, int duration = 3500);
    void Hide(Guid id);
}

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;
    public event Action<Guid>? OnHide;

    public void ShowSuccess(string message, string? title = null, int duration = 3500)
    {
        Show(ToastType.Success, message, title ?? "Success", duration);
    }

    public void ShowError(string message, string? title = null, int duration = 5000)
    {
        Show(ToastType.Error, message, title ?? "Error", duration);
    }

    public void ShowWarning(string message, string? title = null, int duration = 4000)
    {
        Show(ToastType.Warning, message, title ?? "Warning", duration);
    }

    public void ShowInfo(string message, string? title = null, int duration = 3500)
    {
        Show(ToastType.Info, message, title ?? "Info", duration);
    }

    public void Hide(Guid id)
    {
        OnHide?.Invoke(id);
    }

    private void Show(ToastType type, string message, string title, int duration)
    {
        var toast = new ToastMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Title = title,
            Message = message,
            Duration = duration
        };
        OnShow?.Invoke(toast);
    }
}

public class ToastMessage
{
    public Guid Id { get; set; }
    public ToastType Type { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public int Duration { get; set; } = 3500;
}

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}
