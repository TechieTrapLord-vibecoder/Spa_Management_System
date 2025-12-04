using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("ServiceCategory")]
public class ServiceCategory : ISyncable
{
    [Key]
    [Column("service_category_id")]
    public int ServiceCategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

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
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
