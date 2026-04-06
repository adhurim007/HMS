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
import { DepartmentsService } from '../admin/departments/departments.service';
import { DepartmentDto } from '../admin/departments/departments.api';
import {
  VisitFormTemplate,
  VisitClinicalV1,
  VisitFormTemplateId,
  defaultClinicalDraft,
  parseClinicalJson,
  resolveTemplateFromDepartmentCode,
  computeBmiKgM,
} from './visit-clinical.models';

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
  departments: DepartmentDto[] = [];
  currentDoctor: DoctorMeDto | null = null;
  serviceItems: ServiceItemListDto[] = [];

  patientMrnInput = '';
  patientLookupLoading = false;
  patientLookupError = '';
  selectedPatientName: string | null = null;
  selectedPatientId: number | null = null;
  selectedPatientDetail: PatientDto | null = null;

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

  /** Form layout: fixed when editing (server snapshot); derived from doctor when creating. */
  activeVisitTemplate: VisitFormTemplateId = VisitFormTemplate.General;
  gynTab: 'report' | 'colposcopy' | 'spermiogram' = 'report';
  clinicalDraft: VisitClinicalV1 = defaultClinicalDraft();

  readonly VisitFormTemplate = VisitFormTemplate;

  readonly form = this.fb.group({
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
    return this.auth.hasRole('Doctor') && !this.auth.hasRole('SuperAdmin');
  }

  get canCreateVisit(): boolean {
    return this.auth.hasRole('Doctor') && !this.auth.hasRole('SuperAdmin');
  }

  get pediatricBmi(): number | null {
    const w = this.clinicalDraft.vitals?.weightValue;
    const hCm = this.clinicalDraft.vitals?.heightCm;
    const unit = (this.clinicalDraft.vitals?.weightUnit || 'kg').toLowerCase();
    if (w == null || hCm == null || hCm <= 0) return null;
    let kg = Number(w);
    if (unit === 'g') kg = kg / 1000;
    if (!Number.isFinite(kg) || kg <= 0) return null;
    return computeBmiKgM(kg, hCm / 100);
  }

  constructor(
    private readonly visitsService: VisitsService,
    private readonly patientsService: PatientsService,
    private readonly doctorsService: DoctorsService,
    private readonly departmentsService: DepartmentsService,
    private readonly billingService: BillingService,
    private readonly auth: AuthService,
    private readonly route: ActivatedRoute,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.load();

    const qp = this.route.snapshot.queryParamMap;
    const patientIdParam = qp.get('patientId');
    if (patientIdParam) {
      const patientId = Number(patientIdParam);
      this.openCreate();
      this.form.patchValue({ patientId });
      this.selectedPatientId = patientId;
      const p = this.patients.find((x) => x.id === patientId);
      this.selectedPatientName = p ? p.fullName : null;
      this.refreshPatientDetail(patientId);
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

    this.departmentsService
      .getDepartments({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'name',
        sortDesc: false,
        search: null,
        facilityId: null,
      })
      .subscribe({
        next: (res) => {
          this.departments = res.items ?? [];
          this.trySyncTemplateAfterLookups();
        },
      });

    if (this.isDoctorView) {
      this.currentDoctor = null;
      this.doctors = [];
      this.doctorsService.getMe().subscribe({
        next: (me) => {
          this.currentDoctor = me;
          this.trySyncTemplateAfterLookups();
        },
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

  private trySyncTemplateAfterLookups(): void {
    if (!this.showForm || this.editingId != null) return;
    this.syncActiveTemplateFromDoctor();
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

  computeTemplateForSelectedDoctor(): VisitFormTemplateId {
    let deptId: number | null | undefined;
    if (this.isDoctorView && this.currentDoctor?.departmentId != null) {
      deptId = this.currentDoctor.departmentId;
    } else {
      const docId = this.form.value.doctorId;
      if (docId == null) return VisitFormTemplate.General;
      const d = this.doctors.find((x) => x.staffMemberId === docId);
      deptId = d?.departmentId ?? null;
    }
    if (deptId == null) return VisitFormTemplate.General;
    const dep = this.departments.find((x) => x.id === deptId);
    return resolveTemplateFromDepartmentCode(dep?.code);
  }

  syncActiveTemplateFromDoctor(): void {
    if (this.editingId != null) return;
    this.activeVisitTemplate = this.computeTemplateForSelectedDoctor();
    this.clinicalDraft = defaultClinicalDraft();
    this.gynTab = 'report';
  }

  onDoctorSelectionChanged(): void {
    if (this.editingId != null) return;
    this.syncActiveTemplateFromDoctor();
  }

  openCreate(): void {
    this.editingId = null;
    this.editingHasPrescription = false;
    const doctorId =
      this.isDoctorView && this.currentDoctor
        ? this.currentDoctor.staffMemberId
        : null;
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
    this.selectedPatientDetail = null;
    this.syncActiveTemplateFromDoctor();
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
        this.activeVisitTemplate =
          (dto.visitFormTemplate as VisitFormTemplateId) ||
          VisitFormTemplate.General;
        this.clinicalDraft = parseClinicalJson(dto.clinicalDataJson);
        this.gynTab = 'report';
        this.form.reset({
          patientId: dto.patientId,
          doctorId,
          visitDate: dto.visitDate ? dto.visitDate.substring(0, 10) : '',
          chiefComplaint: dto.chiefComplaint ?? '',
          notes: dto.notes ?? '',
          diagnosis: dto.diagnosis ?? '',
        });
        this.servicesArray.clear();
        const services = dto.services;
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
        this.refreshPatientDetail(dto.patientId);
        this.showForm = true;
      },
    });
  }

  closeForm(): void {
    this.showForm = false;
    this.editingHasPrescription = false;
    this.selectedPatientDetail = null;
  }

  refreshPatientDetail(patientId: number | null): void {
    if (patientId == null) {
      this.selectedPatientDetail = null;
      return;
    }
    this.patientsService.getPatient(patientId).subscribe({
      next: (p) => (this.selectedPatientDetail = p),
      error: () => (this.selectedPatientDetail = null),
    });
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
        this.refreshPatientDetail(p.id);
      },
      error: (err) => {
        this.patientLookupLoading = false;
        const msg =
          err?.error?.message ?? err?.message ?? 'Patient not found.';
        this.patientLookupError = msg;
        this.selectedPatientId = null;
        this.selectedPatientName = null;
        this.selectedPatientDetail = null;
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
        bloodGroup: null,
        chronicConditions: null,
        allergies: null,
        parentGuardianName: null,
        pediatricMtl: null,
        pediatricGjtl: null,
        pediatricPkl: null,
        priorLiveBirth: null,
        priorAbortion: null,
      })
      .subscribe({
        next: (p) => {
          this.newPatientSaving = false;
          this.showNewPatientForm = false;
          this.selectedPatientId = p.id;
          this.selectedPatientName = p.fullName;
          this.form.patchValue({ patientId: p.id });
          this.patients.unshift(p);
          this.refreshPatientDetail(p.id);
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

    const servicesPayload = this.servicesArray.controls
      .map((ctrl) => {
        const sv = ctrl.value as {
          serviceItemId: number | null;
          quantity: number;
          unitPrice: number;
          notes: string | null;
        };
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

    const clinicalDataJson =
      this.activeVisitTemplate === VisitFormTemplate.General
        ? null
        : JSON.stringify(this.clinicalDraft);

    const payload = {
      patientId: Number(v.patientId),
      doctorId: v.doctorId ? Number(v.doctorId) : null,
      visitDate: v.visitDate || null,
      chiefComplaint: v.chiefComplaint || null,
      notes: v.notes || null,
      diagnosis: v.diagnosis || null,
      clinicalDataJson,
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
          clinicalDataJson: payload.clinicalDataJson,
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
    if (
      this.isDoctorView &&
      this.currentDoctor &&
      id === this.currentDoctor.staffMemberId
    )
      return this.currentDoctor.fullName;
    const d = this.doctors.find((x) => x.staffMemberId === id);
    return d ? d.fullName : String(id);
  }

  formatTemplateLabel(t: string): string {
    switch (t) {
      case VisitFormTemplate.Pediatrics:
        return 'Pediatrics';
      case VisitFormTemplate.Gynecology:
        return 'Gynecology';
      case VisitFormTemplate.Dentistry:
        return 'Dentistry';
      default:
        return 'General';
    }
  }

  private buildServiceGroup(data?: {
    serviceItemId: number | null;
    quantity: number;
    unitPrice: number;
    notes: string | null;
  }) {
    return this.fb.group({
      serviceItemId: [data?.serviceItemId ?? null, Validators.required],
      quantity: [
        data?.quantity ?? 1,
        [Validators.required, Validators.min(1)],
      ],
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
