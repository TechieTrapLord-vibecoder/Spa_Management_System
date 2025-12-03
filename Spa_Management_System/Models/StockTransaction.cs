using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("StockTransaction")]
public class StockTransaction : ISyncable
{
    [Key]
    [Column("stock_tx_id")]
    public long StockTxId { get; set; }

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
    [Column("product_id")]
    public long ProductId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("tx_type")]
    public string TxType { get; set; } = string.Empty; // 'purchase','sale','adjust','return'

    [Column("qty", TypeName = "decimal(12,2)")]
    public decimal Qty { get; set; }

    [Column("unit_cost", TypeName = "decimal(12,2)")]
    public decimal? UnitCost { get; set; }

    [MaxLength(120)]
    [Column("reference")]
    public string? Reference { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }
}
