using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spa_Sync_API.Models;

[Table("EmployeeServiceCommission")]
public class EmployeeServiceCommission
{
    [Key]
    [Column("commission_id")]
    public long CommissionId { get; set; }

    [Required]
    [Column("employee_id")]
    public long EmployeeId { get; set; }

    [Required]
    [Column("service_id")]
    public long ServiceId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("commission_type")]
    public string CommissionType { get; set; } = string.Empty; // 'percent' or 'fixed'

    [Column("commission_value", TypeName = "decimal(10,2)")]
    public decimal CommissionValue { get; set; }

    [Column("effective_from")]
    public DateTime? EffectiveFrom { get; set; }

    [Column("effective_to")]
    public DateTime? EffectiveTo { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; } = null!;
}

