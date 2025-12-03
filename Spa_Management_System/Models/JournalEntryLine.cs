using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("JournalEntryLine")]
public class JournalEntryLine : ISyncable
{
    [Key]
    [Column("journal_line_id")]
    public long JournalLineId { get; set; }

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
    [Column("journal_id")]
    public long JournalId { get; set; }

    [Required]
    [Column("ledger_account_id")]
    public long LedgerAccountId { get; set; }

    [Column("debit", TypeName = "decimal(14,2)")]
    public decimal Debit { get; set; } = 0;

    [Column("credit", TypeName = "decimal(14,2)")]
    public decimal Credit { get; set; } = 0;

    [Column("line_memo")]
    public string? LineMemo { get; set; }

    // Navigation properties
    [ForeignKey("JournalId")]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey("LedgerAccountId")]
    public virtual LedgerAccount LedgerAccount { get; set; } = null!;
}
