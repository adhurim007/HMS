using HrmsH.Application.Abstractions;
using HrmsH.Application.Notifications;
using HrmsH.Domain.Appointments;
using HrmsH.Domain.Billing;
using HrmsH.Domain.Patients;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Notifications.Queries;

public sealed class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    private readonly IHrmsDbContext _db;

    public GetNotificationsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var roles = request.RoleNames.Select(r => r.ToLowerInvariant()).ToHashSet();
        var readKeys = await _db.UserNotificationReads
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .Select(x => new { x.NotificationType, x.NotificationKey })
            .ToListAsync(cancellationToken);
        var readSet = readKeys.Select(x => (x.NotificationType, x.NotificationKey)).ToHashSet();

        var list = new List<NotificationDto>();
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);

        // Appointments today – global summary for all authenticated users
        var appointmentsCount = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.ScheduledStart >= todayStart && a.ScheduledStart < todayEnd && a.Status != AppointmentStatus.Cancelled)
            .CountAsync(cancellationToken);
        var appointmentsKey = ("AppointmentsToday", "appointments-today");
        list.Add(new NotificationDto
        {
            Type = appointmentsKey.Item1,
            Key = appointmentsKey.Item2,
            Title = "Appointments today",
            Message = appointmentsCount == 0 ? "No appointments scheduled for today." : $"{appointmentsCount} appointment(s) scheduled for today.",
            Link = "/appointments",
            CreatedAt = now,
            IsRead = readSet.Contains(appointmentsKey),
        });

        // Doctor-specific: today's appointments for this doctor only (summary) + per-appointment items
        if (roles.Contains("doctor") && !roles.Contains("superadmin"))
        {
            var doctorStaffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == request.UserId && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (doctorStaffId != 0)
            {
                var myTodayCount = await _db.Appointments
                    .AsNoTracking()
                    .Where(a => a.DoctorId == doctorStaffId &&
                                a.ScheduledStart >= todayStart &&
                                a.ScheduledStart < todayEnd &&
                                a.Status != AppointmentStatus.Cancelled)
                    .CountAsync(cancellationToken);

                var myAppointmentsKey = ("DoctorAppointmentsToday", "doctor-appointments-today");
                list.Add(new NotificationDto
                {
                    Type = myAppointmentsKey.Item1,
                    Key = myAppointmentsKey.Item2,
                    Title = "Your appointments today",
                    Message = myTodayCount == 0
                        ? "You have no appointments scheduled for today."
                        : $"You have {myTodayCount} appointment(s) scheduled for today.",
                    Link = "/appointments",
                    CreatedAt = now,
                    IsRead = readSet.Contains(myAppointmentsKey),
                });

                // Per-appointment notifications for the next 7 days
                var upcomingEnd = todayStart.AddDays(7);
                var doctorAppointments = await _db.Appointments
                    .AsNoTracking()
                    .Where(a => a.DoctorId == doctorStaffId &&
                                a.ScheduledStart >= todayStart &&
                                a.ScheduledStart < upcomingEnd &&
                                a.Status != AppointmentStatus.Cancelled)
                    .Join(_db.Patients.AsNoTracking(),
                        a => a.PatientId,
                        p => p.Id,
                        (a, p) => new { Appointment = a, Patient = p })
                    .OrderBy(x => x.Appointment.ScheduledStart)
                    .ToListAsync(cancellationToken);

                foreach (var item in doctorAppointments)
                {
                    var type = "DoctorAppointment";
                    var key = $"doctor-{doctorStaffId}-appointment-{item.Appointment.Id}";
                    list.Add(new NotificationDto
                    {
                        Type = type,
                        Key = key,
                        Title = "Upcoming appointment",
                        Message = $"{item.Patient.FullName} on {item.Appointment.ScheduledStart:g}",
                        Link = "/appointments",
                        CreatedAt = item.Appointment.ScheduledStart,
                        IsRead = readSet.Contains((type, key)),
                    });
                }
            }
        }

        // Stock expiring in 30 days – show to pharmacy/admin/manager roles
        if (roles.Overlaps(new[] { "superadmin", "hospitaladmin", "manager", "pharmacist" }))
        {
            var thresholdDate = now.Date.AddDays(30);
            var stockCount = await _db.StockBatches
                .AsNoTracking()
                .Where(x => x.ExpiryDate != null && x.ExpiryDate <= thresholdDate && x.QuantityOnHand > 0)
                .CountAsync(cancellationToken);
            var stockKey = ("StockExpiry", "stock-expiry-30");
            list.Add(new NotificationDto
            {
                Type = stockKey.Item1,
                Key = stockKey.Item2,
                Title = "Stock expiry",
                Message = stockCount == 0 ? "No batches expiring in the next 30 days." : $"{stockCount} batch(es) expiring within 30 days.",
                Link = "/pharmacy/stock",
                CreatedAt = now,
                IsRead = readSet.Contains(stockKey),
            });
        }

        // Pending (unpaid) invoices – show to finance/admin/reception/manager
        if (roles.Overlaps(new[] { "superadmin", "hospitaladmin", "manager", "finance", "reception" }))
        {
            var pendingCount = await _db.Invoices
                .AsNoTracking()
                .Where(i => i.Status == InvoiceStatus.Unpaid || i.Status == InvoiceStatus.PartiallyPaid)
                .CountAsync(cancellationToken);
            var invoicesKey = ("PendingInvoices", "pending-invoices");
            list.Add(new NotificationDto
            {
                Type = invoicesKey.Item1,
                Key = invoicesKey.Item2,
                Title = "Pending invoices",
                Message = pendingCount == 0 ? "No unpaid invoices." : $"{pendingCount} unpaid or partially paid invoice(s).",
                Link = "/billing",
                CreatedAt = now,
                IsRead = readSet.Contains(invoicesKey),
            });
        }

        // Visits with unbilled services – per-visit notifications for reception/finance/admin to prepare invoices
        if (roles.Overlaps(new[] { "superadmin", "hospitaladmin", "manager", "finance", "reception" }))
        {
            var unbilledVisits = await _db.VisitServices
                .AsNoTracking()
                .Where(vs => !vs.IsBilled)
                .Join(_db.Visits.AsNoTracking(),
                    vs => vs.VisitId,
                    v => v.Id,
                    (vs, v) => new { vs, v })
                .Join(_db.Patients.AsNoTracking(),
                    x => x.v.PatientId,
                    p => p.Id,
                    (x, p) => new { x.v, Patient = p })
                .GroupBy(x => new { VisitId = x.v.Id, x.v.VisitDate, x.Patient.Id, x.Patient.FullName })
                .Select(g => new
                {
                    g.Key.VisitId,
                    g.Key.VisitDate,
                    PatientId = g.Key.Id,
                    PatientName = g.Key.FullName
                })
                .OrderBy(x => x.VisitDate)
                .ToListAsync(cancellationToken);

            foreach (var v in unbilledVisits)
            {
                var type = "VisitUnbilled";
                var key = $"visit-unbilled-{v.VisitId}";
                list.Add(new NotificationDto
                {
                    Type = type,
                    Key = key,
                    Title = "Visit without invoice",
                    Message = $"{v.PatientName} visit on {v.VisitDate:g} has unbilled services.",
                    Link = $"/patients/{v.PatientId}/billing",
                    CreatedAt = v.VisitDate,
                    IsRead = readSet.Contains((type, key)),
                });
            }
        }

        return list.OrderByDescending(x => x.CreatedAt).ToList();
    }
}
