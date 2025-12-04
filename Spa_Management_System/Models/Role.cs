using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Role")]
public class Role : ISyncable
{
    [Key]
    [Column("role_id")]
    public short RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

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
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
