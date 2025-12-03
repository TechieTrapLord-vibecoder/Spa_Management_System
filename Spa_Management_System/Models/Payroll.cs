using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

/// <summary>
/// Simple payroll - Days Worked × Daily Rate = Gross Pay, minus Deductions = Net Pay
/// </summary>
[Table("Payroll")]
public class Payroll : ISyncable
{
    [Key]
    [Column("payroll_id")]
    public long PayrollId { get; set; }

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
    [Column("employee_id")]
    public long EmployeeId { get; set; }

    [Required]
    [Column("period_start")]
    public DateTime PeriodStart { get; set; }

    [Required]
    [Column("period_end")]
    public DateTime PeriodEnd { get; set; }

    [Column("days_worked")]
    public int DaysWorked { get; set; } = 0;

    [Column("daily_rate", TypeName = "decimal(10,2)")]
    public decimal DailyRate { get; set; } = 500;

    [Column("gross_pay", TypeName = "decimal(12,2)")]
    public decimal GrossPay { get; set; } = 0; // DaysWorked × DailyRate

    [Column("deductions", TypeName = "decimal(12,2)")]
    public decimal Deductions { get; set; } = 0; // SSS, PhilHealth, Pag-IBIG, Tax, etc.

    [Column("net_pay", TypeName = "decimal(12,2)")]
    public decimal NetPay { get; set; } = 0; // GrossPay - Deductions

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft"; // draft, paid

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Column("journal_id")]
    public long? JournalId { get; set; } // Link to journal entry when paid

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }

    [ForeignKey("JournalId")]
    public virtual JournalEntry? JournalEntry { get; set; }
}
