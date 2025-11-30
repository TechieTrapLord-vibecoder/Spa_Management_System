using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("PurchaseOrder")]
public class PurchaseOrder
{
    [Key]
    [Column("po_id")]
    public long PoId { get; set; }

    [Required]
    [Column("supplier_id")]
    public long SupplierId { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [MaxLength(80)]
    [Column("po_number")]
    public string? PoNumber { get; set; }

    [MaxLength(40)]
    [Column("status")]
    public string? Status { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
