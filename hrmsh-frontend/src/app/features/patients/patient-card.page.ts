import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PatientsService } from './patients.service';
import { PatientDto } from './patients.api';
import { VisitsService } from '../visits/visits.service';
import { VisitListDto } from '../visits/visits.api';
import { AppointmentsService } from '../appointments/appointments.service';
import { AppointmentDto } from '../appointments/appointments.api';
import { BillingService } from '../billing/billing.service';
import { InvoiceListDto } from '../billing/billing.api';
import { VisitPrescriptionService } from '../visits/visit-prescription.service';
import { PrescriptionListItemDto } from '../visits/visit-prescription.api';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-patient-card-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './patient-card.page.html',
  styleUrl: './patient-card.page.scss',
})
export class PatientCardPage implements OnInit {
  patientId!: number;
  patient: PatientDto | null = null;

  visits: VisitListDto[] = [];
  visitsLoading = false;

  appointments: AppointmentDto[] = [];
  appointmentsLoading = false;

  invoices: InvoiceListDto[] = [];
  invoicesLoading = false;

  prescriptions: PrescriptionListItemDto[] = [];
  prescriptionsLoading = false;

  // UI state
  activeTab: 'visits' | 'appointments' | 'prescriptions' | 'invoices' =
    'visits';

  error = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly patientsService: PatientsService,
    private readonly visitsService: VisitsService,
    private readonly appointmentsService: AppointmentsService,
    private readonly billingService: BillingService,
    private readonly prescriptionsService: VisitPrescriptionService,
    private readonly auth: AuthService,
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.error = 'Patient id is missing.';
      return;
    }
    this.patientId = Number(idParam);
    if (!this.patientId || Number.isNaN(this.patientId)) {
      this.error = 'Invalid patient id.';
      return;
    }

    this.loadPatient();
    this.loadVisits();
    this.loadAppointments();
    this.loadInvoices();
    this.loadPrescriptions();
  }

  private loadPatient(): void {
    this.patientsService.getPatient(this.patientId).subscribe({
      next: (p) => (this.patient = p),
      error: () => {
        this.patient = null;
        this.error = 'Failed to load patient.';
      },
    });
  }

  private loadVisits(): void {
    this.visitsLoading = true;
    this.visitsService
      .getVisits({
        patientId: this.patientId,
        doctorId: null,
        from: null,
        to: null,
        page: 1,
        pageSize: 20,
        sortBy: 'VisitDate',
        sortDescending: true,
      })
      .subscribe({
        next: (res) => {
          this.visitsLoading = false;
          this.visits = res.items ?? [];
        },
        error: () => {
          this.visitsLoading = false;
          this.visits = [];
        },
      });
  }

  private loadAppointments(): void {
    this.appointmentsLoading = true;
    this.appointmentsService
      .getAppointments({
        pageNumber: 1,
        pageSize: 20,
        sortBy: 'scheduledStart',
        sortDesc: true,
        search: null,
        patientId: this.patientId,
        doctorId: null,
        departmentId: null,
        from: null,
        to: null,
        status: null,
      })
      .subscribe({
        next: (res) => {
          this.appointmentsLoading = false;
          this.appointments = res.items ?? [];
        },
        error: () => {
          this.appointmentsLoading = false;
          this.appointments = [];
        },
      });
  }

  private loadInvoices(): void {
    this.invoicesLoading = true;
    this.billingService
      .getInvoices({
        patientId: this.patientId,
        status: null,
        from: null,
        to: null,
        page: 1,
        pageSize: 20,
        sortBy: 'InvoiceDate',
        sortDescending: true,
      })
      .subscribe({
        next: (res) => {
          this.invoicesLoading = false;
          this.invoices = res.items ?? [];
        },
        error: () => {
          this.invoicesLoading = false;
          this.invoices = [];
        },
      });
  }

  private loadPrescriptions(): void {
    this.prescriptionsLoading = true;
    this.prescriptionsService
      .getList({
        page: 1,
        pageSize: 20,
        patientId: this.patientId,
        doctorId: null,
        status: null,
        from: null,
        to: null,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.prescriptionsLoading = false;
          this.prescriptions = res.items ?? [];
        },
        error: () => {
          this.prescriptionsLoading = false;
          this.prescriptions = [];
        },
      });
  }

  setTab(tab: 'visits' | 'appointments' | 'prescriptions' | 'invoices'): void {
    this.activeTab = tab;
  }

  get canCreateVisit(): boolean {
    // Only pure Doctor (without Reception role) can create visits.
    return this.auth.hasRole('Doctor') && !this.auth.hasRole('Reception');
  }

  get canCreateAppointment(): boolean {
    // Only pure Reception (without Doctor role) can create appointments.
    return this.auth.hasRole('Reception') && !this.auth.hasRole('Doctor');
  }
}

