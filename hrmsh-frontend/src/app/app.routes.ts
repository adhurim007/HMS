import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    canActivateChild: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.page').then(
            (m) => m.DashboardPage,
          ),
      },
      {
        path: 'patients',
        loadComponent: () =>
          import('./features/patients/patients.page').then(
            (m) => m.PatientsPage,
          ),
      },
      {
        path: 'patients/:id/card',
        loadComponent: () =>
          import('./features/patients/patient-card.page').then(
            (m) => m.PatientCardPage,
          ),
      },
      {
        path: 'patients/:id/billing',
        loadComponent: () =>
          import('./features/patients/patient-billing.page').then(
            (m) => m.PatientBillingPage,
          ),
      },
      {
        path: 'appointments',
        loadComponent: () =>
          import('./features/appointments/appointments.page').then(
            (m) => m.AppointmentsPage,
          ),
      },
      {
        path: 'visits',
        loadComponent: () =>
          import('./features/visits/visits.page').then(
            (m) => m.VisitsPage,
          ),
      },
      {
        path: 'visits/:id/prescription',
        loadComponent: () =>
          import('./features/visits/visit-prescription.page').then(
            (m) => m.VisitPrescriptionPage,
          ),
      },
      {
        path: 'prescriptions',
        loadComponent: () =>
          import('./features/prescriptions/prescriptions.page').then(
            (m) => m.PrescriptionsPage,
          ),
      },
      {
        path: 'doctors',
        loadComponent: () =>
          import('./features/doctors/doctors.page').then(
            (m) => m.DoctorsPage,
          ),
      },
      {
        path: 'pharmacy/products',
        loadComponent: () =>
          import('./features/pharmacy/products.page').then(
            (m) => m.PharmacyProductsPage,
          ),
      },
      {
        path: 'pharmacy/stock',
        loadComponent: () =>
          import('./features/pharmacy/stock.page').then(
            (m) => m.PharmacyStockPage,
          ),
      },
      {
        path: 'pharmacy/purchases',
        loadComponent: () =>
          import('./features/pharmacy/purchases.page').then(
            (m) => m.PharmacyPurchasesPage,
          ),
      },
      {
        path: 'pharmacy/sales',
        loadComponent: () =>
          import('./features/pharmacy/sales.page').then(
            (m) => m.PharmacySalesPage,
          ),
      },
      {
        path: 'billing',
        loadComponent: () =>
          import('./features/billing/billing.page').then(
            (m) => m.BillingPage,
          ),
        data: { view: 'invoices' },
      },
      {
        path: 'admin/service-catalog',
        loadComponent: () =>
          import('./features/billing/billing.page').then(
            (m) => m.BillingPage,
          ),
        data: { view: 'services' },
      },
      {
        path: 'diagnostics',
        pathMatch: 'full',
        redirectTo: 'diagnostics/laboratory-workflow',
      },
      {
        path: 'diagnostics/test-catalog',
        loadComponent: () =>
          import('./features/diagnostics/diagnostics.page').then(
            (m) => m.DiagnosticsPage,
          ),
        data: { view: 'tests' },
      },
      {
        path: 'diagnostics/laboratory-workflow',
        loadComponent: () =>
          import('./features/diagnostics/diagnostics.page').then(
            (m) => m.DiagnosticsPage,
          ),
        data: { view: 'laboratory' },
      },
      {
        path: 'diagnostics/laboratory-orders',
        loadComponent: () =>
          import('./features/diagnostics/diagnostics.page').then(
            (m) => m.DiagnosticsPage,
          ),
        data: { view: 'laboratory-orders' },
      },
      {
        path: 'installments',
        loadComponent: () =>
          import('./features/billing/billing.page').then(
            (m) => m.BillingPage,
          ),
      },
      {
        path: 'installments/:id',
        loadComponent: () =>
          import('./features/patients/patient-billing.page').then(
            (m) => m.PatientBillingPage,
          ),
      },
      {
        path: 'patients/:id/installments',
        loadComponent: () =>
          import('./features/patients/patient-billing.page').then(
            (m) => m.PatientBillingPage,
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile.page').then(
            (m) => m.ProfilePage,
          ),
      },
      {
        path: 'admin/facilities',
        loadComponent: () =>
          import('./features/admin/facilities/facilities.page').then(
            (m) => m.FacilitiesPage,
          ),
      },
      {
        path: 'admin/hospitals',
        loadComponent: () =>
          import('./features/admin/hospitals/hospitals.page').then(
            (m) => m.HospitalsPage,
          ),
      },
      {
        path: 'admin/departments',
        loadComponent: () =>
          import('./features/admin/departments/departments.page').then(
            (m) => m.DepartmentsPage,
          ),
      },
      {
        path: 'admin/menus',
        loadComponent: () =>
          import('./features/admin/menus/menus.page').then(
            (m) => m.MenusPage,
          ),
      },
      {
        path: 'admin/users',
        loadComponent: () =>
          import('./features/admin/users/users.page').then(
            (m) => m.UsersPage,
          ),
      },
      {
        path: 'admin/staff',
        loadComponent: () =>
          import('./features/admin/staff/staff.page').then(
            (m) => m.StaffPage,
          ),
      },
      {
        path: 'admin/localization',
        loadComponent: () =>
          import('./features/admin/localization/localization.page').then(
            (m) => m.LocalizationPage,
          ),
      },
      {
        path: 'admin/audit',
        loadComponent: () =>
          import('./features/admin/audit/audit.page').then(
            (m) => m.AuditPage,
          ),
      },
      {
        path: 'admin/services-config',
        loadComponent: () =>
          import('./features/admin/services-config/services-config.page').then(
            (m) => m.ServicesConfigPage,
          ),
      },
      {
        path: 'admin/doctor-revenue-rules',
        loadComponent: () =>
          import(
            './features/admin/doctor-revenue-rules/doctor-revenue-rules.page'
          ).then((m) => m.DoctorRevenueRulesPage),
      },
      {
        path: 'reports/visits-per-doctor',
        loadComponent: () =>
          import('./features/reports/visits-per-doctor.page').then(
            (m) => m.VisitsPerDoctorReportPage,
          ),
      },
      {
        path: 'reports/doctor-revenue',
        loadComponent: () =>
          import('./features/reports/doctor-revenue.page').then(
            (m) => m.DoctorRevenueReportPage,
          ),
      },
      {
        path: 'reports/patient-visits',
        loadComponent: () =>
          import('./features/reports/patient-visits.page').then(
            (m) => m.PatientVisitsReportPage,
          ),
      },
    ],
  },
  {
    path: 'auth/login',
    loadComponent: () =>
      import('./features/auth/login.page').then((m) => m.LoginPage),
  },
  {
    path: 'auth/signup',
    loadComponent: () =>
      import('./features/auth/signup.page').then((m) => m.SignupPage),
  },
  { path: '**', redirectTo: '' },
];

