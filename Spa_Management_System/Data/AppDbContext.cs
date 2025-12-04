using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Models;

namespace Spa_Management_System.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet properties for all entities
    public DbSet<Person> Persons { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<EmployeeServiceCommission> EmployeeServiceCommissions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierProduct> SupplierProducts { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentService> AppointmentServices { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<LedgerAccount> LedgerAccounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
    public DbSet<CrmNote> CrmNotes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure unique constraints
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<UserAccount>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.CustomerCode)
            .IsUnique();

        modelBuilder.Entity<Service>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();

        modelBuilder.Entity<Inventory>()
            .HasIndex(i => i.ProductId)
            .IsUnique();

        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex(po => po.PoNumber)
            .IsUnique();

        modelBuilder.Entity<Sale>()
            .HasIndex(s => s.SaleNumber)
            .IsUnique();

        modelBuilder.Entity<LedgerAccount>()
            .HasIndex(la => la.Code)
            .IsUnique();

        modelBuilder.Entity<JournalEntry>()
            .HasIndex(je => je.JournalNo)
            .IsUnique();

        // Unique constraint for attendance - one record per employee per day
        modelBuilder.Entity<EmployeeAttendance>()
            .HasIndex(ea => new { ea.EmployeeId, ea.WorkDate })
            .IsUnique();

        // Configure relationships and cascade behaviors
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Person)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Person)
            .WithMany(p => p.Customers)
            .HasForeignKey(c => c.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // SupplierProduct: unique constraint on supplier + product combination
        modelBuilder.Entity<SupplierProduct>()
            .HasIndex(sp => new { sp.SupplierId, sp.ProductId })
            .IsUnique();

        modelBuilder.Entity<SupplierProduct>()
            .HasOne(sp => sp.Supplier)
            .WithMany(s => s.SupplierProducts)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SupplierProduct>()
            .HasOne(sp => sp.Product)
            .WithMany(p => p.SupplierProducts)
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
