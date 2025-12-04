using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Service")]
public class Service : ISyncable
{
    [Key]
    [Column("service_id")]
    public long ServiceId { get; set; }

    // Sync tracking properties
    [Column("sync_id")]
    public Guid SyncId { get; set; } = Guid.NewGuid();

    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; }

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [MaxLength(20)]
    [Column("sync_status")]
    public string SyncStatus { get; set; } = "pending";

    [Column("sync_version")]
    public int SyncVersion { get; set; } = 1;

    [Column("service_category_id")]
    public int? ServiceCategoryId { get; set; }

    [MaxLength(60)]
    [Column("code")]
    public string? Code { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price", TypeName = "decimal(12,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("duration_minutes")]
    public int DurationMinutes { get; set; } = 0;

    [Column("active")]
    public bool Active { get; set; } = true;

    // Commission settings - fixed per service for all therapists
    [MaxLength(20)]
    [Column("commission_type")]
    public string CommissionType { get; set; } = "percentage"; // 'percentage' or 'fixed'

    [Column("commission_value", TypeName = "decimal(12,2)")]
    public decimal CommissionValue { get; set; } = 0.00m; // e.g., 30 for 30%, or 100 for ₱100 fixed

    // Navigation properties
    [ForeignKey("ServiceCategoryId")]
    public virtual ServiceCategory? ServiceCategory { get; set; }

    public virtual ICollection<EmployeeServiceCommission> EmployeeServiceCommissions { get; set; } = new List<EmployeeServiceCommission>();
    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
