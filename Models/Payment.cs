using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Payment")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public long PaymentId { get; set; }

    [Required]
    [Column("sale_id")]
    public long SaleId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty; // 'cash','card','voucher'

    [Column("amount", TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; }

    [Column("paid_at")]
    public DateTime PaidAt { get; set; } = DateTime.Now;

    [Column("recorded_by_user_id")]
    public long? RecordedByUserId { get; set; }

    // Navigation properties
    [ForeignKey("SaleId")]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey("RecordedByUserId")]
    public virtual UserAccount? RecordedByUser { get; set; }
}
