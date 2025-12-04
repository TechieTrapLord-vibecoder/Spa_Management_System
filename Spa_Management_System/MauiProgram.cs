using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Data;
using Spa_Management_System.Data.Repositories;
using Spa_Management_System.Services;
using Spa_Management_System.Models;

namespace Spa_Management_System
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add Blazor services
            builder.Services.AddMauiBlazorWebView();

            // Load configuration from appsettings.json
            var assembly = typeof(MauiProgram).Assembly;
            using var stream = assembly.GetManifestResourceStream("Spa_Management_System.appsettings.json");
            
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream!)
                .Build();
            
            builder.Configuration.AddConfiguration(config);

            // Configure Database from configuration
            var connectionString = config.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            // Also register AppDbContext directly for backward compatibility
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            // Register Generic Repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register Specific Repositories
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            // Register Services
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentManagementService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<IAccountingService, AccountingService>();
            
            // Register PDF Export Service
            builder.Services.AddSingleton<PdfExportService>();
            
            // Register Authentication State Service as Singleton (persists across app)
            builder.Services.AddSingleton<IAuthStateService, AuthStateService>();

            // Register Toast Notification Service as Singleton (shared across all pages)
            builder.Services.AddSingleton<IToastService, ToastService>();

            // Register Sync Service for offline-first cloud synchronization
            builder.Services.AddScoped<ISyncService, SyncService>();

            // Register Auto-Sync Service as Singleton (timer persists across app)
            builder.Services.AddSingleton<IAutoSyncService, AutoSyncService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
