using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("ServiceCategory")]
public class ServiceCategory
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

    // Navigation properties
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
