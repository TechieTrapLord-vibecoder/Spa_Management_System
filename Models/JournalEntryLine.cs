using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("JournalEntryLine")]
public class JournalEntryLine
{
    [Key]
    [Column("journal_line_id")]
    public long JournalLineId { get; set; }

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
