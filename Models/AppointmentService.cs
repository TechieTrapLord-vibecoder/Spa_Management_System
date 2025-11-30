using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("AppointmentService")]
public class AppointmentService
{
    [Key]
    [Column("appt_srv_id")]
    public long ApptSrvId { get; set; }

    [Required]
    [Column("appointment_id")]
    public long AppointmentId { get; set; }

    [Required]
    [Column("service_id")]
    public long ServiceId { get; set; }

    [Column("therapist_employee_id")]
    public long? TherapistEmployeeId { get; set; }

    [Column("price", TypeName = "decimal(12,2)")]
    public decimal Price { get; set; }

    [Column("commission_amount", TypeName = "decimal(12,2)")]
    public decimal CommissionAmount { get; set; } = 0;

    // Navigation properties
    [ForeignKey("AppointmentId")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; } = null!;

    [ForeignKey("TherapistEmployeeId")]
    public virtual Employee? TherapistEmployee { get; set; }
}
