using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Domain.Appointments;
using HrmsH.Domain.Billing;
using HrmsH.Domain.Common;
using HrmsH.Domain.Menus;
using HrmsH.Domain.Localization;
using HrmsH.Domain.Organization;
using HrmsH.Domain.Patients;
using HrmsH.Domain.Pharmacy;
using HrmsH.Domain.Staff;
using HrmsH.Domain.Audit;
using HrmsH.Domain.Notifications;
using HrmsH.Domain.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Infrastructure.Persistence;

public class HrmsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IHrmsDbContext
{
    private readonly ICurrentUserService? _currentUser;
    private readonly IDateTime? _clock;

    public HrmsDbContext(DbContextOptions<HrmsDbContext> options) : base(options)
    {
    }

    public HrmsDbContext(
        DbContextOptions<HrmsDbContext> options,
        ICurrentUserService currentUser,
        IDateTime clock) : base(options)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<VisitService> VisitServices => Set<VisitService>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<DiagnosticTest> DiagnosticTests => Set<DiagnosticTest>();
    public DbSet<LaboratoryOrder> LaboratoryOrders => Set<LaboratoryOrder>();
    public DbSet<LaboratoryOrderItem> LaboratoryOrderItems => Set<LaboratoryOrderItem>();
    public DbSet<LaboratorySample> LaboratorySamples => Set<LaboratorySample>();
    public DbSet<LaboratoryResult> LaboratoryResults => Set<LaboratoryResult>();

    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<DepartmentService> DepartmentServices => Set<DepartmentService>();
    public DbSet<DoctorService> DoctorServices => Set<DoctorService>();
    public DbSet<DoctorRevenueRule> DoctorRevenueRules => Set<DoctorRevenueRule>();
    public DbSet<DoctorRevenueShare> DoctorRevenueShares => Set<DoctorRevenueShare>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<InstallmentPlan> InstallmentPlans => Set<InstallmentPlan>();
    public DbSet<InstallmentItem> InstallmentItems => Set<InstallmentItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockBatch> StockBatches => Set<StockBatch>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<PharmacyPurchaseInvoice> PharmacyPurchaseInvoices => Set<PharmacyPurchaseInvoice>();
    public DbSet<PharmacyPurchaseInvoiceItem> PharmacyPurchaseInvoiceItems => Set<PharmacyPurchaseInvoiceItem>();
    public DbSet<DoctorVisitSettings> DoctorVisitSettings => Set<DoctorVisitSettings>();
    public DbSet<DoctorWeeklyScheduleDay> DoctorWeeklyScheduleDays => Set<DoctorWeeklyScheduleDay>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserNotificationRead> UserNotificationReads => Set<UserNotificationRead>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(HrmsDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(HrmsDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { builder });
            }
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
        where TEntity : BaseEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        AddAuditLogs();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = _clock?.UtcNow ?? DateTime.UtcNow;
        var userId = _currentUser?.UserId?.ToString();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }

    private void AddAuditLogs()
    {
        var now = _clock?.UtcNow ?? DateTime.UtcNow;
        var userId = _currentUser?.UserId;
        var userName = _currentUser?.UserName;

        // Collect audit entries first; do not add to context while iterating (causes "Collection was modified").
        var toAdd = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.Entity is AuditLog)
            {
                continue;
            }

            string? action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => null,
            };

            if (action is null)
            {
                continue;
            }

            // Track only key domain entities for now
            int? patientId = null;
            var entityType = entry.Entity.GetType().Name;
            var entityId = entry.Entity.Id;

            switch (entry.Entity)
            {
                case Patient p:
                    patientId = p.Id;
                    break;
                case Visit v:
                    patientId = v.PatientId;
                    break;
                case Invoice i:
                    patientId = i.PatientId;
                    break;
                case StockMovement:
                    patientId = null;
                    break;
            }

            var description = action switch
            {
                "Created" => $"{entityType} created.",
                "Updated" => $"{entityType} updated.",
                "Deleted" => $"{entityType} deleted.",
                _ => null,
            };

            toAdd.Add(new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                CreatedAt = now,
                CreatedBy = userId?.ToString(),
                UserIdInt = userId,
                UserName = userName,
                PatientId = patientId,
                Description = description,
            });
        }

        foreach (var log in toAdd)
        {
            AuditLogs.Add(log);
        }
    }
}

