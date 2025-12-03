using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("Sale")]
public class Sale : ISyncable
{
    [Key]
    [Column("sale_id")]
    public long SaleId { get; set; }

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

    [Column("customer_id")]
    public long? CustomerId { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [MaxLength(80)]
    [Column("sale_number")]
    public string? SaleNumber { get; set; }

    [Column("total_amount", TypeName = "decimal(12,2)")]
    public decimal TotalAmount { get; set; } = 0.00m;

    [MaxLength(40)]
    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "unpaid";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }

    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

