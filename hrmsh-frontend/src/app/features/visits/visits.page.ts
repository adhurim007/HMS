import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  FormArray,
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { VisitsService } from './visits.service';
import { VisitListDto } from './visits.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { DoctorsService } from '../doctors/doctors.service';
import { DoctorDto, DoctorMeDto } from '../doctors/doctors.api';
import { BillingService } from '../billing/billing.service';
import { ServiceItemListDto } from '../billing/billing.api';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-visits-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './visits.page.html',
  styleUrl: './visits.page.scss',
})
export class VisitsPage implements OnInit {
  visits: VisitListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  sortBy: string | null = 'VisitDate';
  sortDescending = true;
  loading = false;

  patientFilter: number | null = null;
  doctorFilter: number | null = null;
  fromFilter: string | null = null;
  toFilter: string | null = null;

  patients: PatientDto[] = [];
  doctors: DoctorDto[] = [];
  /** Set when user is Doctor (not SuperAdmin): form shows only self, no doctor dropdown. */
  currentDoctor: DoctorMeDto | null = null;
  serviceItems: ServiceItemListDto[] = [];

  /** When creating a visit without pre-selected patient (direct from doctor),
   *  we allow searching by MRN and showing basic patient info instead of a dropdown.
   */
  patientMrnInput = '';
  patientLookupLoading = false;
  patientLookupError = '';
  selectedPatientName: string | null = null;
  selectedPatientId: number | null = null;

  /** Inline quick-create patient from visit screen */
  showNewPatientForm = false;
  newPatientFullName = '';
  newPatientGender: number = 1;
  newPatientPhone = '';
  newPatientEmail = '';
  newPatientSaving = false;
  newPatientError = '';

  editingId: number | null = null;
  editingHasPrescription = false;
  showForm = false;

  readonly form = this.fb.group({
    // patientId is still sent to backend, but for doctors the UI does not show a dropdown;
    // instead we resolve the patient via MRN or preselected appointment and set this hidden field.
    patientId: [null as number | null, [Validators.required]],
    doctorId: [null as number | null],
    visitDate: [''],
    chiefComplaint: [''],
    notes: [''],
    diagnosis: [''],
     services: this.fb.array([]),
  });

  get servicesArray(): FormArray {
    return this.form.get('services') as FormArray;
  }

  get isDoctorView(): boolean {
    return (
      this.auth.hasRole('Doctor') &&
      !this.auth.hasRole('SuperAdmin')
    );
  }

  get canCreateVisit(): boolean {
    // Doctors can create visits. (Even if the account has multiple roles.)
    return this.auth.hasRole('Doctor') && !this.auth.hasRole('SuperAdmin');
  }

  constructor(
    private readonly visitsService: VisitsService,
    private readonly patientsService: PatientsService,
    private readonly doctorsService: DoctorsService,
    private readonly billingService: BillingService,
    private readonly auth: AuthService,
    private readonly route: ActivatedRoute,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.load();

    // If navigated with query params (e.g. from appointment), pre-open create form.
    const qp = this.route.snapshot.queryParamMap;
    const patientIdParam = qp.get('patientId');
    if (patientIdParam) {
      const patientId = Number(patientIdParam);
      this.openCreate();
      this.form.patchValue({ patientId });
      this.selectedPatientId = patientId;
      const p = this.patients.find((x) => x.id === patientId);
      this.selectedPatientName = p ? p.fullName : null;
    }
  }

  loadLookups(): void {
    this.patientsService
      .getPatients({
        pageNumber: 1,
        pageSize: 200,
        sortBy: 'fullName',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => {
          const r = res as { items?: PatientDto[]; Items?: PatientDto[] };
          this.patients = r.items ?? r.Items ?? [];
        },
      });

    if (this.isDoctorView) {
      this.currentDoctor = null;
      this.doctors = [];
      this.doctorsService.getMe().subscribe({
        next: (me) => (this.currentDoctor = me),
        error: () => (this.currentDoctor = null),
      });
    } else {
      this.currentDoctor = null;
      this.doctorsService
        .getDoctors({
          pageNumber: 1,
          pageSize: 200,
          sortBy: 'fullName',
          sortDesc: false,
          search: null,
          departmentId: null,
          isActive: true,
        })
        .subscribe({
          next: (res) => {
            const r = res as { items?: DoctorDto[]; Items?: DoctorDto[] };
            this.doctors = r.items ?? r.Items ?? [];
          },
        });
    }

    if (this.isDoctorView) {
      this.billingService.getServicesForMe().subscribe({
        next: (items) => {
          this.serviceItems = items;
        },
      });
    } else {
      this.billingService
        .getServiceItems({
          page: 1,
          pageSize: 500,
          search: null,
          isActive: true,
          sortBy: 'name',
          sortDescending: false,
        })
        .subscribe({
          next: (res) => {
            this.serviceItems = res.items;
          },
        });
    }
  }

  load(): void {
    this.loading = true;
    this.visitsService
      .getVisits({
        patientId: this.patientFilter,
        doctorId: this.doctorFilter,
        from: this.fromFilter,
        to: this.toFilter,
        page: this.page,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDescending: this.sortDescending,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.visits = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  applyFilters(): void {
    this.page = 1;
    this.load();
  }

  clearFilters(): void {
    this.patientFilter = null;
    this.doctorFilter = null;
    this.fromFilter = null;
    this.toFilter = null;
    this.page = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.page + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.page = next;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.page = 1;
    this.load();
  }

  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDescending = !this.sortDescending;
    } else {
      this.sortBy = column;
      this.sortDescending = true;
    }
    this.load();
  }

  openCreate(): void {
    this.editingId = null;
    this.editingHasPrescription = false;
    const doctorId =
      this.isDoctorView && this.currentDoctor ? this.currentDoctor.staffMemberId : null;
    this.form.reset({
      patientId: null,
      doctorId,
      visitDate: '',
      chiefComplaint: '',
      notes: '',
      diagnosis: '',
    });
    this.servicesArray.clear();
    this.addServiceRow();
    this.patientMrnInput = '';
    this.patientLookupError = '';
    this.patientLookupLoading = false;
    this.selectedPatientName = null;
    this.selectedPatientId = null;
    this.showForm = true;
  }

  openEdit(v: VisitListDto): void {
    this.visitsService.getVisit(v.id).subscribe({
      next: (dto) => {
        this.editingId = dto.id;
        this.editingHasPrescription = !!dto.hasPrescription;
        const doctorId =
          this.isDoctorView && this.currentDoctor
            ? this.currentDoctor.staffMemberId
            : (dto.doctorId ?? null);
        this.form.reset({
          patientId: dto.patientId,
          doctorId,
          visitDate: dto.visitDate ? dto.visitDate.substring(0, 10) : '',
          chiefComplaint: dto.chiefComplaint ?? '',
          notes: dto.notes ?? '',
          diagnosis: dto.diagnosis ?? '',
        });
        this.servicesArray.clear();
        const services = (dto as any).services as
          | {
              serviceItemId: number;
              quantity: number;
              unitPrice: number;
              notes?: string | null;
            }[]
          | undefined;
        if (services && services.length) {
          services.forEach((s) =>
            this.servicesArray.push(
              this.buildServiceGroup({
                serviceItemId: s.serviceItemId,
                quantity: s.quantity,
                unitPrice: s.unitPrice,
                notes: s.notes ?? null,
              }),
            ),
          );
        } else {
          this.addServiceRow();
        }
        this.patientMrnInput = '';
        this.patientLookupError = '';
        this.patientLookupLoading = false;
        this.selectedPatientId = dto.patientId;
        const p = this.patients.find((x) => x.id === dto.patientId);
        this.selectedPatientName = p ? p.fullName : null;
        this.showForm = true;
      },
    });
  }

  closeForm(): void {
    this.showForm = false;
    this.editingHasPrescription = false;
  }

  searchPatientByMrn(): void {
    const mrn = (this.patientMrnInput || '').trim();
    if (!mrn) {
      this.patientLookupError = 'Enter personal number / MRN.';
      return;
    }
    this.patientLookupLoading = true;
    this.patientLookupError = '';
    this.patientsService.getPatientByMrn(mrn).subscribe({
      next: (p) => {
        this.patientLookupLoading = false;
        this.selectedPatientId = p.id;
        this.selectedPatientName = p.fullName;
        this.form.patchValue({ patientId: p.id });
      },
      error: (err) => {
        this.patientLookupLoading = false;
        const msg =
          err?.error?.message ?? err?.message ?? 'Patient not found.';
        this.patientLookupError = msg;
        this.selectedPatientId = null;
        this.selectedPatientName = null;
        this.form.patchValue({ patientId: null });
      },
    });
  }

  startNewPatient(): void {
    this.showNewPatientForm = true;
    this.newPatientFullName = '';
    this.newPatientGender = 1;
    this.newPatientPhone = '';
    this.newPatientEmail = '';
    this.newPatientError = '';
  }

  cancelNewPatient(): void {
    this.showNewPatientForm = false;
    this.newPatientSaving = false;
    this.newPatientError = '';
  }

  createNewPatientFromVisit(): void {
    const mrn = (this.patientMrnInput || '').trim();
    if (!mrn) {
      this.newPatientError = 'Enter personal number / MRN first.';
      return;
    }
    if (!this.newPatientFullName.trim()) {
      this.newPatientError = 'Full name is required.';
      return;
    }

    this.newPatientSaving = true;
    this.newPatientError = '';

    this.patientsService
      .createPatient({
        medicalRecordNumber: mrn,
        fullName: this.newPatientFullName.trim(),
        dateOfBirth: null,
        gender: this.newPatientGender,
        phone: this.newPatientPhone.trim() || null,
        email: this.newPatientEmail.trim() || null,
        address: null,
      })
      .subscribe({
        next: (p) => {
          this.newPatientSaving = false;
          this.showNewPatientForm = false;
          this.selectedPatientId = p.id;
          this.selectedPatientName = p.fullName;
          this.form.patchValue({ patientId: p.id });
          // also refresh local patients list so getPatientName works for this new patient
          this.patients.unshift(p);
        },
        error: (err) => {
          this.newPatientSaving = false;
          const msg =
            err?.error?.message ??
            err?.message ??
            'Failed to create patient.';
          this.newPatientError = msg;
        },
      });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;

    const servicesPayload =
      this.servicesArray.controls
        .map((ctrl) => {
          const sv = ctrl.value as any;
          if (!sv.serviceItemId) return null;
          return {
            serviceItemId: Number(sv.serviceItemId),
            quantity: sv.quantity ? Number(sv.quantity) : 1,
            unitPrice:
              sv.unitPrice !== null && sv.unitPrice !== undefined
                ? Number(sv.unitPrice)
                : null,
            notes: sv.notes || null,
          };
        })
        .filter(Boolean) as {
        serviceItemId: number;
        quantity: number;
        unitPrice: number | null;
        notes: string | null;
      }[];

    const payload = {
      patientId: Number(v.patientId),
      doctorId: v.doctorId ? Number(v.doctorId) : null,
      visitDate: v.visitDate || null,
      chiefComplaint: v.chiefComplaint || null,
      notes: v.notes || null,
      diagnosis: v.diagnosis || null,
      services: servicesPayload,
    };

    if (this.editingId == null) {
      this.visitsService.createVisit(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.visitsService
        .updateVisit(this.editingId, {
          doctorId: payload.doctorId,
          visitDate: payload.visitDate,
          chiefComplaint: payload.chiefComplaint,
          notes: payload.notes,
          diagnosis: payload.diagnosis,
      services: servicesPayload,
      })
        .subscribe({
          next: () => {
            this.showForm = false;
            this.load();
          },
        });
    }
  }

  deleteVisit(v: VisitListDto): void {
    if (!confirm('Delete this visit?')) return;
    this.visitsService.deleteVisit(v.id).subscribe({
      next: () => this.load(),
    });
  }

  getPatientName(id: number): string {
    const p = this.patients.find((x) => x.id === id);
    return p ? p.fullName : String(id);
  }

  getDoctorName(id: number | null | undefined): string {
    if (id == null) return '-';
    if (this.isDoctorView && this.currentDoctor && id === this.currentDoctor.staffMemberId)
      return this.currentDoctor.fullName;
    const d = this.doctors.find((x) => x.staffMemberId === id);
    return d ? d.fullName : String(id);
  }

  private buildServiceGroup(data?: {
    serviceItemId: number | null;
    quantity: number;
    unitPrice: number;
    notes: string | null;
  }) {
    return this.fb.group({
      serviceItemId: [data?.serviceItemId ?? null, Validators.required],
      quantity: [data?.quantity ?? 1, [Validators.required, Validators.min(1)]],
      unitPrice: [data?.unitPrice ?? 0, Validators.required],
      notes: [data?.notes ?? null],
    });
  }

  addServiceRow(): void {
    this.servicesArray.push(this.buildServiceGroup());
  }

  removeServiceRow(index: number): void {
    this.servicesArray.removeAt(index);
  }
}
