using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("LedgerAccount")]
public class LedgerAccount
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

    // Navigation properties
    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}

