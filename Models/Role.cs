using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Role")]
public class Role
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

    // Navigation properties
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
