using HrmsH.Domain.Appointments;
using HrmsH.Domain.Billing;
using HrmsH.Domain.Menus;
using HrmsH.Domain.Localization;
using HrmsH.Domain.Organization;
using HrmsH.Domain.Patients;
using HrmsH.Domain.Pharmacy;
using HrmsH.Domain.Staff;
using HrmsH.Domain.Audit;
using HrmsH.Domain.Notifications;
using HrmsH.Domain.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Abstractions;

public interface IHrmsDbContext
{
    DbSet<Facility> Facilities { get; }
    DbSet<Department> Departments { get; }

    DbSet<StaffMember> StaffMembers { get; }
    DbSet<DoctorProfile> DoctorProfiles { get; }

    DbSet<Patient> Patients { get; }
    DbSet<Visit> Visits { get; }
    DbSet<VisitService> VisitServices { get; }
    DbSet<Prescription> Prescriptions { get; }
    DbSet<PrescriptionItem> PrescriptionItems { get; }

    DbSet<Appointment> Appointments { get; }
    DbSet<DiagnosticTest> DiagnosticTests { get; }
    DbSet<LaboratoryOrder> LaboratoryOrders { get; }
    DbSet<LaboratoryOrderItem> LaboratoryOrderItems { get; }
    DbSet<LaboratorySample> LaboratorySamples { get; }
    DbSet<LaboratoryResult> LaboratoryResults { get; }

    DbSet<ServiceItem> ServiceItems { get; }
    DbSet<DepartmentService> DepartmentServices { get; }
    DbSet<DoctorService> DoctorServices { get; }
    DbSet<DoctorRevenueRule> DoctorRevenueRules { get; }
    DbSet<DoctorRevenueShare> DoctorRevenueShares { get; }

    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<InstallmentPlan> InstallmentPlans { get; }
    DbSet<InstallmentItem> InstallmentItems { get; }

    DbSet<Product> Products { get; }
    DbSet<StockBatch> StockBatches { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<PharmacyPurchaseInvoice> PharmacyPurchaseInvoices { get; }
    DbSet<PharmacyPurchaseInvoiceItem> PharmacyPurchaseInvoiceItems { get; }

    DbSet<DoctorVisitSettings> DoctorVisitSettings { get; }
    DbSet<DoctorWeeklyScheduleDay> DoctorWeeklyScheduleDays { get; }

    DbSet<Menu> Menus { get; }
    DbSet<RoleMenu> RoleMenus { get; }

    DbSet<Language> Languages { get; }
    DbSet<Translation> Translations { get; }

    DbSet<AuditLog> AuditLogs { get; }
    DbSet<UserNotificationRead> UserNotificationReads { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

