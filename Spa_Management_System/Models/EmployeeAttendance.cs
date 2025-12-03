using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Management_System.Models;

/// <summary>
/// Simple attendance - just records days worked per pay period
/// </summary>
[Table("EmployeeAttendance")]
public class EmployeeAttendance
{
    [Key]
    [Column("attendance_id")]
    public long AttendanceId { get; set; }

    [Required]
    [Column("employee_id")]
    public long EmployeeId { get; set; }

    [Required]
    [Column("work_date")]
    public DateTime WorkDate { get; set; } = DateTime.Today; // Week ending date

    [Column("days_worked", TypeName = "decimal(4,1)")]
    public decimal HoursWorked { get; set; } = 5; // Days worked (0-7), kept as HoursWorked for compatibility

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_by_user_id")]
    public long? CreatedByUserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    public virtual UserAccount? CreatedByUser { get; set; }
}
