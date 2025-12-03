using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("SaleItem")]
public class SaleItem : ISyncable
{
    [Key]
    [Column("sale_item_id")]
    public long SaleItemId { get; set; }

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

    [Required]
    [Column("sale_id")]
    public long SaleId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("item_type")]
    public string ItemType { get; set; } = string.Empty; // 'product' or 'service'

    [Column("product_id")]
    public long? ProductId { get; set; }

    [Column("service_id")]
    public long? ServiceId { get; set; }

    [Column("qty", TypeName = "decimal(12,2)")]
    public decimal Qty { get; set; } = 1;

    [Column("unit_price", TypeName = "decimal(12,2)")]
    public decimal UnitPrice { get; set; }

    [Column("line_total", TypeName = "decimal(12,2)")]
    public decimal LineTotal { get; set; }

    [Column("therapist_employee_id")]
    public long? TherapistEmployeeId { get; set; }

    // Navigation properties
    [ForeignKey("SaleId")]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }

    [ForeignKey("ServiceId")]
    public virtual Service? Service { get; set; }

    [ForeignKey("TherapistEmployeeId")]
    public virtual Employee? TherapistEmployee { get; set; }
}
