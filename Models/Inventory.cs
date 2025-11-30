using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Inventory")]
public class Inventory
{
    [Key]
    [Column("inventory_id")]
    public long InventoryId { get; set; }

    [Required]
    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("quantity_on_hand", TypeName = "decimal(12,2)")]
    public decimal QuantityOnHand { get; set; } = 0;

    [Column("reorder_level", TypeName = "decimal(12,2)")]
    public decimal ReorderLevel { get; set; } = 0;

    [Column("last_counted_at")]
    public DateTime? LastCountedAt { get; set; }

    // Navigation properties
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;
}
