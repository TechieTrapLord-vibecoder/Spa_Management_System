using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

/// <summary>
/// Many-to-many relationship between Suppliers and Products.
/// </summary>
[Table("SupplierProduct")]
public class SupplierProduct : ISyncable
{
    [Key]
    [Column("supplier_product_id")]
    public long SupplierProductId { get; set; }

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

    [Column("supplier_id")]
    public long SupplierId { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("supplier_price", TypeName = "decimal(12,2)")]
    public decimal SupplierPrice { get; set; }

    [MaxLength(80)]
    [Column("supplier_sku")]
    public string? SupplierSku { get; set; }

    [Column("min_order_qty")]
    public int? MinOrderQty { get; set; }

    [Column("lead_time_days")]
    public int? LeadTimeDays { get; set; }

    [Column("is_preferred")]
    public bool IsPreferred { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
