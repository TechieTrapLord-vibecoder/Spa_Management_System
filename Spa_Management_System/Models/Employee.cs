using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

[Table("Employee")]
public class Employee : ISyncable
{
    [Key]
    [Column("employee_id")]
    public long EmployeeId { get; set; }

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
    [Column("person_id")]
    public long PersonId { get; set; }

    [Required]
    [Column("role_id")]
    public short RoleId { get; set; }

    [Column("hire_date")]
    public DateTime? HireDate { get; set; }

    [MaxLength(30)]
    [Column("status")]
    public string Status { get; set; } = "active";

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
    public virtual ICollection<EmployeeServiceCommission> EmployeeServiceCommissions { get; set; } = new List<EmployeeServiceCommission>();
    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public virtual ICollection<EmployeeAttendance> Attendances { get; set; } = new List<EmployeeAttendance>();
    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
}
