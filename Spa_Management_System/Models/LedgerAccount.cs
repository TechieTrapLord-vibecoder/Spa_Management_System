using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("LedgerAccount")]
public class LedgerAccount : ISyncable
{
    [Key]
    [Column("ledger_account_id")]
    public long LedgerAccountId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("account_type")]
    public string AccountType { get; set; } = string.Empty; // 'asset','liability','equity','revenue','expense'

    [Required]
    [MaxLength(10)]
    [Column("normal_balance")]
    public string NormalBalance { get; set; } = string.Empty; // 'debit' or 'credit'

    // ISyncable properties
    [Column("sync_id")]
    public Guid SyncId { get; set; } = Guid.NewGuid();

    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; } = DateTime.Now;

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [Column("sync_status")]
    [MaxLength(20)]
    public string SyncStatus { get; set; } = "pending";

    [Column("sync_version")]
    public int SyncVersion { get; set; } = 1;

    // Navigation properties
    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
