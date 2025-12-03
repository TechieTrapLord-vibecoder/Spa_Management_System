using Spa_Management_System.Data.Repositories;
using Spa_Management_System.Models;

namespace Spa_Management_System.Services;

public interface IAppointmentService
{
    Task<Appointment?> GetAppointmentByIdAsync(long appointmentId);
    Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(long customerId);
    Task<Appointment> CreateAppointmentAsync(long customerId, DateTime scheduledStart, DateTime? scheduledEnd, long? createdByUserId);
    Task<Appointment> UpdateAppointmentStatusAsync(long appointmentId, string status);
    Task<bool> CancelAppointmentAsync(long appointmentId);
    Task<Models.AppointmentService> AddServiceToAppointmentAsync(long appointmentId, long serviceId, long? therapistEmployeeId);
}

public class AppointmentManagementService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Models.AppointmentService> _appointmentServiceRepository;
    private readonly IRepository<Service> _serviceRepository;

    public AppointmentManagementService(
        IAppointmentRepository appointmentRepository,
        IRepository<Models.AppointmentService> appointmentServiceRepository,
        IRepository<Service> serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentServiceRepository = appointmentServiceRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(long appointmentId)
    {
        return await _appointmentRepository.GetWithDetailsAsync(appointmentId);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _appointmentRepository.GetAppointmentsByDateRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(long customerId)
    {
        return await _appointmentRepository.GetAppointmentsByCustomerAsync(customerId);
    }

    public async Task<Appointment> CreateAppointmentAsync(long customerId, DateTime scheduledStart, DateTime? scheduledEnd, long? createdByUserId)
    {
        var appointment = new Appointment
        {
            CustomerId = customerId,
            ScheduledStart = scheduledStart,
            ScheduledEnd = scheduledEnd,
            Status = "scheduled",
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.Now
        };

        return await _appointmentRepository.AddAsync(appointment);
    }

    public async Task<Appointment> UpdateAppointmentStatusAsync(long appointmentId, string status)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null)
            throw new Exception("Appointment not found");

        appointment.Status = status;
        return await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<bool> CancelAppointmentAsync(long appointmentId)
    {
        return await UpdateAppointmentStatusAsync(appointmentId, "cancelled") != null;
    }

    public async Task<Models.AppointmentService> AddServiceToAppointmentAsync(long appointmentId, long serviceId, long? therapistEmployeeId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null)
            throw new Exception("Service not found");

        var appointmentService = new Models.AppointmentService
        {
            AppointmentId = appointmentId,
            ServiceId = serviceId,
            TherapistEmployeeId = therapistEmployeeId,
            Price = service.Price,
            CommissionAmount = 0 // Calculate based on commission rules
        };

        return await _appointmentServiceRepository.AddAsync(appointmentService);
    }
}
