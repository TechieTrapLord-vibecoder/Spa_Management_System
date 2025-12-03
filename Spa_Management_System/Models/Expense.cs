using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Expense")]
public class Expense : ISyncable
{
    [Key]
    [Column("expense_id")]
    public long ExpenseId { get; set; }

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
    [Column("expense_date")]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    [MaxLength(100)]
    [Column("category")]
    public string Category { get; set; } = string.Empty; // Rent, Utilities, Supplies, Salaries, Marketing, Maintenance, Insurance, Other

    [Required]
    [MaxLength(300)]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("amount", TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; } = 0;

    [MaxLength(100)]
    [Column("vendor")]
    public string? Vendor { get; set; }

    [MaxLength(100)]
    [Column("reference_number")]
    public string? ReferenceNumber { get; set; } // Invoice #, Receipt #, etc.

    [MaxLength(50)]
    [Column("payment_method")]
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Bank Transfer, Check

    [MaxLength(30)]
    [Column("status")]
    public string Status { get; set; } = "paid"; // paid, pending, cancelled

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("ledger_account_id")]
    public long? LedgerAccountId { get; set; } // Link to expense account in chart of accounts

    [Column("journal_id")]
    public long? JournalId { get; set; } // Link to journal entry if auto-posted

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("LedgerAccountId")]
    public virtual LedgerAccount? LedgerAccount { get; set; }

    [ForeignKey("JournalId")]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }
}
