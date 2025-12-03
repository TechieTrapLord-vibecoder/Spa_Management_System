using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("AppointmentService")]
public class AppointmentService : ISyncable
{
    [Key]
    [Column("appt_srv_id")]
    public long ApptSrvId { get; set; }

    // Sync tracking properties
    [Column("sync_id")]
    public Guid SyncId { get; set; } = Guid.NewGuid();

    [Column("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; }

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [MaxLength(20)]
    [Column("sync_status")]
    public string SyncStatus { get; set; } = "pending";

    [Column("sync_version")]
    public int SyncVersion { get; set; } = 1;

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
