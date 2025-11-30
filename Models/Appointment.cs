using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Appointment")]
public class Appointment
{
    [Key]
    [Column("appointment_id")]
    public long AppointmentId { get; set; }

    [Required]
    [Column("customer_id")]
    public long CustomerId { get; set; }

    [Required]
    [Column("scheduled_start")]
    public DateTime ScheduledStart { get; set; }

    [Column("scheduled_end")]
    public DateTime? ScheduledEnd { get; set; }

    [MaxLength(40)]
    [Column("status")]
    public string Status { get; set; } = "scheduled";

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("notes")]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
}
