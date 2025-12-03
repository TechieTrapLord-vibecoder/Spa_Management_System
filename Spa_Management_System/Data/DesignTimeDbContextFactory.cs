using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Spa_Management_System.Data;

/// <summary>
/// Factory for creating DbContext at design time (for EF migrations).
/// This allows running migrations from command line even in a MAUI project.
/// 
/// Usage:
///   dotnet ef migrations add MigrationName --project Spa_Management_System.csproj --framework net8.0
///   dotnet ef database update --project Spa_Management_System.csproj --framework net8.0
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Try to load from appsettings.json, fallback to hardcoded connection string
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=NIKOLA\\SQLEXPRESS;Initial Catalog=spa_erp;Integrated Security=True;Trust Server Certificate=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
