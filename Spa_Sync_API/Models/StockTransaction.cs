using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("StockTransaction")]
public class StockTransaction
{
    [Key]
    [Column("stock_tx_id")]
    public long StockTxId { get; set; }

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

