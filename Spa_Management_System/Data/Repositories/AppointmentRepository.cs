using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Models;

namespace Spa_Management_System.Data.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(long customerId);
    Task<Appointment?> GetWithDetailsAsync(long appointmentId);
    Task<bool> HasTherapistConflictAsync(long therapistId, DateTime startTime, DateTime endTime, long? excludeAppointmentId = null);
}

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(a => a.Customer)
                .ThenInclude(c => c.Person)
            .Include(a => a.AppointmentServices)
                .ThenInclude(aps => aps.Service)
            .Where(a => a.ScheduledStart >= startDate && a.ScheduledStart <= endDate)
            .OrderBy(a => a.ScheduledStart)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(long customerId)
    {
        return await _dbSet
            .Include(a => a.AppointmentServices)
                .ThenInclude(aps => aps.Service)
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.ScheduledStart)
            .ToListAsync();
    }

    public async Task<Appointment?> GetWithDetailsAsync(long appointmentId)
    {
        return await _dbSet
            .Include(a => a.Customer)
                .ThenInclude(c => c.Person)
            .Include(a => a.AppointmentServices)
                .ThenInclude(aps => aps.Service)
            .Include(a => a.AppointmentServices)
                .ThenInclude(aps => aps.TherapistEmployee)
                    .ThenInclude(e => e!.Person)
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
    }

    public async Task<bool> HasTherapistConflictAsync(long therapistId, DateTime startTime, DateTime endTime, long? excludeAppointmentId = null)
    {
        // Check if therapist has any overlapping appointments (not cancelled)
        var conflictingAppointments = await _context.AppointmentServices
            .Include(appts => appts.Appointment)
            .Where(appts => appts.TherapistEmployeeId == therapistId
                && appts.Appointment.Status != "cancelled"
                && appts.Appointment.Status != "paid"
                && (excludeAppointmentId == null || appts.AppointmentId != excludeAppointmentId)
                && appts.Appointment.ScheduledStart < endTime
                && (appts.Appointment.ScheduledEnd ?? appts.Appointment.ScheduledStart.AddHours(1)) > startTime)
            .ToListAsync();

        return conflictingAppointments.Any();
    }
}
