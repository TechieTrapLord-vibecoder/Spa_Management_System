using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

/// <summary>
/// Many-to-many relationship between Suppliers and Products.
/// Tracks which products each supplier can provide and at what price.
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

    // Foreign Keys
    [Column("supplier_id")]
    public long SupplierId { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// The price this specific supplier charges for this product.
    /// May differ from the product's default cost price.
    /// </summary>
    [Column("supplier_price", TypeName = "decimal(12,2)")]
    public decimal SupplierPrice { get; set; }

    /// <summary>
    /// Supplier's SKU or part number for this product (optional).
    /// </summary>
    [MaxLength(80)]
    [Column("supplier_sku")]
    public string? SupplierSku { get; set; }

    /// <summary>
    /// Minimum order quantity from this supplier (optional).
    /// </summary>
    [Column("min_order_qty")]
    public int? MinOrderQty { get; set; }

    /// <summary>
    /// Lead time in days for delivery from this supplier (optional).
    /// </summary>
    [Column("lead_time_days")]
    public int? LeadTimeDays { get; set; }

    /// <summary>
    /// Whether this supplier is the preferred/primary supplier for this product.
    /// </summary>
    [Column("is_preferred")]
    public bool IsPreferred { get; set; } = false;

    /// <summary>
    /// Whether this supplier-product relationship is active.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
