using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Customer")]
public class Customer
{
    [Key]
    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Required]
    [Column("person_id")]
    public long PersonId { get; set; }

    [MaxLength(50)]
    [Column("customer_code")]
    public string? CustomerCode { get; set; }

    [Column("loyalty_points")]
    public int LoyaltyPoints { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    // Navigation properties
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<CrmNote> CrmNotes { get; set; } = new List<CrmNote>();
}
