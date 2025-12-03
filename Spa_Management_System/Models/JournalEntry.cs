using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("JournalEntry")]
public class JournalEntry : ISyncable
{
    [Key]
    [Column("journal_id")]
    public long JournalId { get; set; }

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

    [MaxLength(80)]
    [Column("journal_no")]
    public string? JournalNo { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }

    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
