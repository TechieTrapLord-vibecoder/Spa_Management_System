using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Spa_Management_System.Services;

/// <summary>
/// Background service that handles automatic synchronization based on configured interval
/// </summary>
public interface IAutoSyncService
{
    /// <summary>
    /// Start the auto-sync timer
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the auto-sync timer
    /// </summary>
    void Stop();

    /// <summary>
    /// Update the sync interval (in minutes)
    /// </summary>
    void UpdateInterval(int minutes);

    /// <summary>
    /// Check if auto-sync is currently running
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Get the last auto-sync time
    /// </summary>
    DateTime? LastAutoSyncTime { get; }
    
    /// <summary>
    /// Get the next scheduled sync time
    /// </summary>
    DateTime? NextSyncTime { get; }

    /// <summary>
    /// Event fired when auto-sync completes
    /// </summary>
    event EventHandler<AutoSyncEventArgs>? OnAutoSyncCompleted;
}

public class AutoSyncEventArgs : EventArgs
{
    public bool Success { get; set; }
    public int RecordsUploaded { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SyncTime { get; set; }
}

public class AutoSyncService : IAutoSyncService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer;
    private bool _isRunning;
    private DateTime? _lastAutoSyncTime;
    private DateTime? _nextSyncTime;
    private bool _isSyncing;

    public bool IsRunning => _isRunning;
    public DateTime? LastAutoSyncTime => _lastAutoSyncTime;
    public DateTime? NextSyncTime => _nextSyncTime;
    public event EventHandler<AutoSyncEventArgs>? OnAutoSyncCompleted;

    public AutoSyncService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeFromSettings();
    }

    private void InitializeFromSettings()
    {
        // Load settings and start if enabled
        using var scope = _serviceProvider.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
        var settings = syncService.GetSettings();

        if (settings.AutoSyncEnabled && settings.SyncIntervalMinutes > 0)
        {
            Start(settings.SyncIntervalMinutes);
        }
    }

    public void Start()
    {
        using var scope = _serviceProvider.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
        var settings = syncService.GetSettings();
        Start(settings.SyncIntervalMinutes);
    }

    private void Start(int intervalMinutes)
    {
        if (intervalMinutes <= 0)
            intervalMinutes = 5; // Default to 5 minutes

        Stop(); // Stop any existing timer

        _timer = new Timer(intervalMinutes * 60 * 1000); // Convert to milliseconds
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();
        _isRunning = true;
        
        // Set next sync time
        _nextSyncTime = DateTime.Now.AddMinutes(intervalMinutes);

        Console.WriteLine($"[AutoSync] Started with interval: {intervalMinutes} minutes");
    }

    public void Stop()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
            _timer = null;
        }
        _isRunning = false;
        _nextSyncTime = null;
        Console.WriteLine("[AutoSync] Stopped");
    }

    public void UpdateInterval(int minutes)
    {
        if (_isRunning)
        {
            Start(minutes);
        }
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_isSyncing)
        {
            Console.WriteLine("[AutoSync] Sync already in progress, skipping...");
            return;
        }

        _isSyncing = true;
        Console.WriteLine($"[AutoSync] Timer triggered at {DateTime.Now}");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

            // Check if auto-sync is still enabled
            var settings = syncService.GetSettings();
            if (!settings.AutoSyncEnabled)
            {
                Console.WriteLine("[AutoSync] Auto-sync is disabled, stopping timer...");
                Stop();
                return;
            }

            // Get pending count first
            var status = await syncService.GetSyncStatusAsync();
            if (status.TotalPending == 0)
            {
                Console.WriteLine("[AutoSync] No pending data to sync");
                _lastAutoSyncTime = DateTime.Now;
                _nextSyncTime = DateTime.Now.AddMinutes(settings.SyncIntervalMinutes);
                OnAutoSyncCompleted?.Invoke(this, new AutoSyncEventArgs
                {
                    Success = true,
                    RecordsUploaded = 0,
                    SyncTime = DateTime.Now,
                    ErrorMessage = null
                });
                return;
            }

            // Perform sync (same as SyncPending)
            Console.WriteLine($"[AutoSync] Syncing {status.TotalPending} pending records...");
            var result = await syncService.SyncPendingAsync();

            _lastAutoSyncTime = DateTime.Now;
            _nextSyncTime = DateTime.Now.AddMinutes(settings.SyncIntervalMinutes);

            OnAutoSyncCompleted?.Invoke(this, new AutoSyncEventArgs
            {
                Success = result.Success,
                RecordsUploaded = result.RecordsUploaded,
                SyncTime = DateTime.Now,
                ErrorMessage = result.ErrorMessage
            });

            if (result.Success)
            {
                Console.WriteLine($"[AutoSync] Completed successfully. Uploaded: {result.RecordsUploaded} records");
            }
            else
            {
                Console.WriteLine($"[AutoSync] Failed: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoSync] Error: {ex.Message}");
            OnAutoSyncCompleted?.Invoke(this, new AutoSyncEventArgs
            {
                Success = false,
                RecordsUploaded = 0,
                SyncTime = DateTime.Now,
                ErrorMessage = ex.Message
            });
        }
        finally
        {
            _isSyncing = false;
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
