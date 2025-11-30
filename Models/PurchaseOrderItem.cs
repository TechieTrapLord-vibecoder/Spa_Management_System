using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("PurchaseOrderItem")]
public class PurchaseOrderItem
{
    [Key]
    [Column("po_item_id")]
    public long PoItemId { get; set; }

    [Required]
    [Column("po_id")]
    public long PoId { get; set; }

    [Required]
    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("qty_ordered", TypeName = "decimal(12,2)")]
    public decimal QtyOrdered { get; set; }

    [Column("unit_cost", TypeName = "decimal(12,2)")]
    public decimal UnitCost { get; set; }

    // Navigation properties
    [ForeignKey("PoId")]
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;
}
