using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Person")]
public class Person
{
    [Key]
    [Column("person_id")]
    public long PersonId { get; set; }

    [Required]
    [MaxLength(120)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(50)]
    [Column("phone")]
    public string? Phone { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("dob")]
    public DateTime? Dob { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
