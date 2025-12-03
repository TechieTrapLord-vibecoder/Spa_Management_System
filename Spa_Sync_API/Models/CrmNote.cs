using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("CRM_Note")]
public class CrmNote
{
    [Key]
    [Column("note_id")]
    public long NoteId { get; set; }

    [Required]
    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Required]
    [Column("created_by_user_id")]
    public long CreatedByUserId { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount CreatedByUser { get; set; } = null!;
}

