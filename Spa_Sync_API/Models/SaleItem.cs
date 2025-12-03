using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("SaleItem")]
public class SaleItem
{
    [Key]
    [Column("sale_item_id")]
    public long SaleItemId { get; set; }

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

