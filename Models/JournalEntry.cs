using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("JournalEntry")]
public class JournalEntry
{
    [Key]
    [Column("journal_id")]
    public long JournalId { get; set; }

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
