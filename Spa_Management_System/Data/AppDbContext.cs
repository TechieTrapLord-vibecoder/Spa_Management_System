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

    /// <summary>
    /// Override SaveChanges to automatically update sync tracking properties
    /// </summary>
    public override int SaveChanges()
    {
        UpdateSyncProperties();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update sync tracking properties
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateSyncProperties();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates LastModifiedAt and SyncStatus for all modified ISyncable entities
    /// </summary>
    private void UpdateSyncProperties()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is ISyncable && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var syncable = (ISyncable)entry.Entity;
            
            // For new entities, ensure SyncId is set
            if (entry.State == EntityState.Added)
            {
                // Always update LastModifiedAt for new entities
                syncable.LastModifiedAt = DateTime.Now;
                
                if (syncable.SyncId == Guid.Empty)
                {
                    syncable.SyncId = Guid.NewGuid();
                }
                syncable.SyncStatus = "pending";
                syncable.SyncVersion = 1;
            }
            // For modified entities, only mark as pending if we're not explicitly syncing
            else if (entry.State == EntityState.Modified)
            {
                // Check if SyncStatus property was explicitly modified to "synced"
                var syncStatusProperty = entry.Property(nameof(ISyncable.SyncStatus));
                var lastSyncedProperty = entry.Property(nameof(ISyncable.LastSyncedAt));
                
                // If we're setting to "synced", this is a sync operation - don't reset to pending
                if (syncStatusProperty.IsModified && syncable.SyncStatus == "synced")
                {
                    // This is a sync marking operation, leave it alone
                    continue;
                }
                
                // Regular modification - update timestamp and mark as pending
                syncable.LastModifiedAt = DateTime.Now;
                
                // Only reset to pending if it was previously synced
                if (syncable.SyncStatus == "synced")
                {
                    syncable.SyncStatus = "pending";
                    syncable.SyncVersion++;
                }
            }
        }
    }
}
