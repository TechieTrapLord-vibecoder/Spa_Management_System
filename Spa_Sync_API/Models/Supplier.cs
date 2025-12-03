using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("Supplier")]
public class Supplier
{
    [Key]
    [Column("supplier_id")]
    public long SupplierId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("contact_person")]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    [Column("phone")]
    public string? Phone { get; set; }

    [MaxLength(150)]
    [Column("email")]
    public string? Email { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    // Navigation properties
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

