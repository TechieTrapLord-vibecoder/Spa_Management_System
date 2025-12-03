using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Product")]
public class Product : ISyncable
{
    [Key]
    [Column("product_id")]
    public long ProductId { get; set; }

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

    [MaxLength(80)]
    [Column("sku")]
    public string? Sku { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("unit_price", TypeName = "decimal(12,2)")]
    public decimal UnitPrice { get; set; } = 0.00m;

    [Column("cost_price", TypeName = "decimal(12,2)")]
    public decimal CostPrice { get; set; } = 0.00m;

    [MaxLength(20)]
    [Column("unit")]
    public string? Unit { get; set; }

    [Column("active")]
    public bool Active { get; set; } = true;

    // Navigation properties
    public virtual Inventory? Inventory { get; set; }
    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
