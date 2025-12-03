using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("AuditLog")]
public class AuditLog
{
    [Key]
    [Column("audit_id")]
    public long AuditId { get; set; }

    [MaxLength(120)]
    [Column("entity_name")]
    public string? EntityName { get; set; }

    [MaxLength(80)]
    [Column("entity_id")]
    public string? EntityId { get; set; }

    [MaxLength(10)]
    [Column("action")]
    public string? Action { get; set; } // 'create','update','delete'

    [Column("changed_by_user_id")]
    public long? ChangedByUserId { get; set; }

    [Column("change_summary")]
    public string? ChangeSummary { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ChangedByUserId")]
    public virtual UserAccount? ChangedByUser { get; set; }
}

