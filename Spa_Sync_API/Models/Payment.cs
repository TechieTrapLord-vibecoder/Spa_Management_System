using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("Payment")]
public class Payment : ISyncable
{
    [Key]
    [Column("payment_id")]
    public long PaymentId { get; set; }

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

