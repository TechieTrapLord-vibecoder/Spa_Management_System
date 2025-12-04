using Microsoft.EntityFrameworkCore;
using Spa_Sync_API.Models;

namespace Spa_Sync_API.Data;

public class CloudDbContext : DbContext
{
    public CloudDbContext(DbContextOptions<CloudDbContext> options) : base(options) { }

    // All 28 tables - exact mirror of local database
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<UserAccount> UserAccounts { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<EmployeeServiceCommission> EmployeeServiceCommissions { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<SupplierProduct> SupplierProducts { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AppointmentService> AppointmentServices { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<LedgerAccount> LedgerAccounts { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; } = null!;
    public DbSet<CrmNote> CrmNotes { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Payroll> Payrolls { get; set; } = null!;
    public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure unique indexes
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<UserAccount>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(c => c.CustomerCode).IsUnique();
        modelBuilder.Entity<Service>().HasIndex(s => s.Code).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();
        modelBuilder.Entity<Inventory>().HasIndex(i => i.ProductId).IsUnique();
        modelBuilder.Entity<PurchaseOrder>().HasIndex(po => po.PoNumber).IsUnique();
        modelBuilder.Entity<Sale>().HasIndex(s => s.SaleNumber).IsUnique();
        modelBuilder.Entity<LedgerAccount>().HasIndex(la => la.Code).IsUnique();
        modelBuilder.Entity<JournalEntry>().HasIndex(je => je.JournalNo).IsUnique();

        // Disable cascade delete for all relationships to avoid issues
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
