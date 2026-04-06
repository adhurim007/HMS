import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AppointmentsService } from './appointments.service';
import { AppointmentDto } from './appointments.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { DoctorsService } from '../doctors/doctors.service';
import { DoctorCalendarSlotDto, DoctorDto, DoctorMeDto } from '../doctors/doctors.api';
import { DepartmentsService } from '../admin/departments/departments.service';
import { DepartmentDto } from '../admin/departments/departments.api';
import { FacilitiesService } from '../admin/facilities/facilities.service';
import { FacilityDto } from '../admin/facilities/facilities.api';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-appointments-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './appointments.page.html',
  styleUrl: './appointments.page.scss',
})
export class AppointmentsPage implements OnInit {
  appointments: AppointmentDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = null;
  sortDesc = false;
  loading = false;

  statusFilter: string | null = null;
  fromFilter: string | null = null;
  toFilter: string | null = null;
  patientFilter: number | null = null;
  doctorFilter: number | null = null;
  departmentFilter: number | null = null;
  facilityFilter: number | null = null;

  patients: PatientDto[] = [];
  // Patient picker (search + optional create)
  patientSearch = '';
  patientSearchLoading = false;
  patientSearchResults: PatientDto[] = [];
  showPatientCreate = false;

  patientNewMedicalRecordNumber = '';
  patientNewFullName = '';
  patientNewDateOfBirth = '';
  patientNewGender: number = 0; // 0=Unknown, 1=Male, 2=Female
  patientNewPhone = '';
  patientNewEmail = '';

  private patientSearchTimer: ReturnType<typeof setTimeout> | null = null;
  readonly patientGenders = [
    { value: 0, label: 'Unknown' },
    { value: 1, label: 'Male' },
    { value: 2, label: 'Female' },
  ];
  doctors: DoctorDto[] = [];
  departments: DepartmentDto[] = [];
  facilities: FacilityDto[] = [];
  filteredDepartments: DepartmentDto[] = [];
  facilityFormInput = '';
  facilityFilterInput = '';

  /** Set when user is Doctor (not SuperAdmin): used to fix form to self + own department. */
  currentDoctor: DoctorMeDto | null = null;

  readonly statuses = [
    { value: '', label: 'All statuses' },
    { value: 'Pending', label: 'Pending' },
    { value: 'Confirmed', label: 'Confirmed' },
    { value: 'Completed', label: 'Completed' },
    { value: 'Cancelled', label: 'Cancelled' },
    { value: 'NoShow', label: 'No show' },
  ];

  editingId: number | null = null;
  showForm = false;

  // Cancel confirmation modal (theme modal, not native confirm/alert)
  cancelModalOpen = false;
  cancelModalAppointment: AppointmentDto | null = null;

  readonly form = this.fb.group({
    patientId: [null as number | null, [Validators.required]],
    doctorId: [null as number | null],
    facilityId: [null as number | null],
    departmentId: [null as number | null],
    scheduledStart: ['', Validators.required],
    scheduledEnd: [''],
    reason: [''],
  });

  readonly math = Math;

  get isDoctorView(): boolean {
    // Treat users who ALSO have Reception (or SuperAdmin) as non-doctor for this screen.
    // Only pure doctors (no SuperAdmin, no Reception) get the locked doctor view.
    return (
      this.auth.hasRole('Doctor') &&
      !this.auth.hasRole('SuperAdmin') &&
      !this.auth.hasRole('Reception')
    );
  }

  // Calendar view state
  viewMode: 'list' | 'calendar' = 'list';
  calendarCurrent: Date = new Date();
  calendarWeeks: { date: Date; inMonth: boolean }[][] = [];
  calendarAppointments: AppointmentDto[] = [];
  calendarSlotsByDate: Record<string, DoctorCalendarSlotDto[]> = {};
  calendarLoading = false;
  calendarDoctorId: number | null = null;
  calendarCreateOpen = false;

  constructor(
    private readonly appointmentsService: AppointmentsService,
    private readonly patientsService: PatientsService,
    private readonly doctorsService: DoctorsService,
    private readonly departmentsService: DepartmentsService,
    private readonly facilitiesService: FacilitiesService,
    private readonly auth: AuthService,
    private readonly router: Router,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.load();
  }

  loadLookups(): void {
    this.facilitiesService
      .getFacilities({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'name',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.facilities = res.items;
        },
      });

    if (this.isDoctorView) {
      this.doctorsService.getMe().subscribe({
        next: (me) => {
          this.currentDoctor = me;
          this.calendarDoctorId = me.staffMemberId;
          if (this.viewMode === 'calendar') {
            this.loadCalendar();
          }
        },
        error: () => {
          this.currentDoctor = null;
          this.calendarDoctorId = null;
        },
      });
    } else {
      this.currentDoctor = null;
      this.calendarDoctorId = null;
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
            const anyRes = res as unknown as {
              items?: DoctorDto[];
              Items?: DoctorDto[];
            };
            this.doctors = anyRes.items ?? anyRes.Items ?? [];
          },
        });
      this.departmentsService
        .getDepartments({
          pageNumber: 1,
          pageSize: 200,
          sortBy: 'name',
          sortDesc: false,
          search: null,
          facilityId: null,
        })
        .subscribe({
          next: (res) => {
            const anyRes = res as unknown as {
              items?: DepartmentDto[];
              Items?: DepartmentDto[];
            };
            this.departments = anyRes.items ?? anyRes.Items ?? [];
            this.syncFacilityAndDepartment();
          },
        });
    }

    this.patientsService
      .getPatients({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'fullName',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.patients = res.items ?? [];
        },
      });
  }

  resetPatientPicker(): void {
    this.patientSearch = '';
    this.patientSearchLoading = false;
    this.patientSearchResults = [];
    this.showPatientCreate = false;

    this.patientNewMedicalRecordNumber = '';
    this.patientNewFullName = '';
    this.patientNewDateOfBirth = '';
    this.patientNewGender = 0;
    this.patientNewPhone = '';
    this.patientNewEmail = '';

    this.form.patchValue({ patientId: null });
  }

  onPatientSearchChange(value: string): void {
    const term = (value ?? '').trim();
    this.patientSearch = value;

    // If user changes the search term, clear previously selected patient.
    this.form.patchValue({ patientId: null });
    this.form.controls.patientId.markAsTouched();

    this.showPatientCreate = false;
    this.patientSearchResults = [];

    if (this.patientSearchTimer) {
      clearTimeout(this.patientSearchTimer);
      this.patientSearchTimer = null;
    }

    if (term.length < 2) return;

    this.patientSearchLoading = true;
    this.patientSearchTimer = setTimeout(() => {
      const searchForApi = this.normalizePatientSearchForApi(term);
      this.patientsService
        .getPatients({
          pageNumber: 1,
          pageSize: 10,
          sortBy: 'mrn',
          sortDesc: false,
          search: searchForApi,
        })
        .subscribe({
          next: (res) => {
            this.patientSearchLoading = false;
            this.patientSearchResults = res.items ?? [];

            if (this.patientSearchResults.length === 0) {
              this.showPatientCreate = true;
              this.patientNewMedicalRecordNumber = term;
              this.patientNewFullName = '';
              this.patientNewDateOfBirth = '';
              this.patientNewGender = 0;
              this.patientNewPhone = '';
              this.patientNewEmail = '';
            }
          },
          error: () => {
            this.patientSearchLoading = false;
            this.patientSearchResults = [];
            this.showPatientCreate = true;
            this.patientNewMedicalRecordNumber = term;
            this.patientNewFullName = '';
            this.patientNewDateOfBirth = '';
            this.patientNewGender = 0;
            this.patientNewPhone = '';
            this.patientNewEmail = '';
          },
        });
    }, 300);
  }

  private normalizePatientSearchForApi(term: string): string {
    // If the user is searching by "personal number" (digits), normalize common formatting characters.
    // If the term contains letters, keep it as-is (so name searches work).
    const hasLetters = /[a-zA-Z]/.test(term);
    if (hasLetters) return term;

    return term
      .replace(/\s/g, '')
      .replace(/-/g, '')
      .replace(/\(/g, '')
      .replace(/\)/g, '')
      .replace(/\./g, '')
      .replace(/\//g, '');
  }

  selectPatient(p: PatientDto): void {
    this.form.patchValue({ patientId: p.id });
    this.showPatientCreate = false;
    this.patientSearchResults = [];

    // Keep local list for display in appointment list.
    const exists = this.patients.some((x) => x.id === p.id);
    if (!exists) this.patients = [...this.patients, p];
  }

  createPatientFromPicker(): void {
    const mrn = this.patientNewMedicalRecordNumber.trim();
    const fullName = this.patientNewFullName.trim();

    if (!mrn || !fullName) {
      alert('Medical record number and full name are required to create a patient.');
      return;
    }

    this.patientsService
      .createPatient({
        medicalRecordNumber: mrn,
        fullName,
        dateOfBirth: this.patientNewDateOfBirth || null,
        gender: Number(this.patientNewGender),
        phone: this.patientNewPhone || null,
        email: this.patientNewEmail || null,
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
          this.selectPatient(p);
          this.patientSearch = `${p.fullName} (${p.medicalRecordNumber})`;
          this.showPatientCreate = false;
        },
        error: (err) => {
          const msg = err?.error?.message ?? err?.message ?? 'Failed to create patient.';
          alert(msg);
        },
      });
  }

  load(): void {
    this.loading = true;
    this.appointmentsService
      .getAppointments({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
        facilityId: this.facilityFilter,
        patientId: this.patientFilter,
        doctorId: this.doctorFilter,
        departmentId: this.departmentFilter,
        from: this.fromFilter,
        to: this.toFilter,
        status: this.statusFilter,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.appointments = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  onSearchChange(): void {
    this.pageNumber = 1;
    this.load();
  }

  getStatusLabel(status: unknown): string {
    if (status === 1 || status === 'Pending') return 'Pending';
    if (status === 2 || status === 'Confirmed') return 'Confirmed';
    if (status === 3 || status === 'Completed') return 'Completed';
    if (status === 4 || status === 'Cancelled') return 'Cancelled';
    if (status === 5 || status === 'NoShow') return 'No show';
    return String(status ?? '');
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.statusFilter = null;
    this.fromFilter = null;
    this.toFilter = null;
    this.patientFilter = null;
    this.doctorFilter = null;
    this.departmentFilter = null;
    this.facilityFilter = null;
    this.pageNumber = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.pageNumber + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.pageNumber = next;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDesc = !this.sortDesc;
    } else {
      this.sortBy = column;
      this.sortDesc = false;
    }
    this.load();
  }

  openCreate(): void {
    this.editingId = null;
    const doctorId = this.isDoctorView && this.currentDoctor ? this.currentDoctor.staffMemberId : null;
    const departmentId = this.isDoctorView && this.currentDoctor ? this.currentDoctor.departmentId ?? null : null;
    const facilityId = departmentId != null ? (this.departments.find((x) => x.id === departmentId)?.facilityId ?? null) : null;
    this.form.reset({
      patientId: null,
      doctorId,
      facilityId,
      departmentId,
      scheduledStart: '',
      scheduledEnd: '',
      reason: '',
    });
    const selectedFacility = this.facilities.find((f) => f.id === facilityId);
    this.facilityFormInput = selectedFacility ? this.getFacilityOptionLabel(selectedFacility) : '';
    this.onFacilityChange();
    this.resetPatientPicker();
    this.showForm = true;
  }

  openEdit(a: AppointmentDto): void {
    this.editingId = a.id;
    const doctorId = this.isDoctorView && this.currentDoctor ? this.currentDoctor.staffMemberId : (a.doctorId ?? null);
    const departmentId = this.isDoctorView && this.currentDoctor ? (this.currentDoctor.departmentId ?? null) : (a.departmentId ?? null);
    const facilityId = departmentId != null ? (this.departments.find((x) => x.id === departmentId)?.facilityId ?? null) : null;
    this.form.reset({
      patientId: a.patientId,
      doctorId,
      facilityId,
      departmentId,
      scheduledStart: a.scheduledStart.substring(0, 16),
      scheduledEnd: a.scheduledEnd ? a.scheduledEnd.substring(0, 16) : '',
      reason: a.reason ?? '',
    });
    const selectedFacility = this.facilities.find((f) => f.id === facilityId);
    this.facilityFormInput = selectedFacility ? this.getFacilityOptionLabel(selectedFacility) : '';
    this.onFacilityChange();

    // Show selected patient in picker input (no search).
    const p = this.patients.find((x) => x.id === a.patientId);
    this.patientSearch = p ? `${p.fullName} (${p.medicalRecordNumber})` : String(a.patientId);
    this.patientSearchResults = [];
    this.showPatientCreate = false;
    this.patientSearchLoading = false;
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;
    const payload = {
      facilityId: v.facilityId ? Number(v.facilityId) : null,
      patientId: Number(v.patientId),
      doctorId: v.doctorId ? Number(v.doctorId) : null,
      departmentId: v.departmentId ? Number(v.departmentId) : null,
      scheduledStart: v.scheduledStart!,
      scheduledEnd: v.scheduledEnd || null,
      reason: v.reason || null,
    };

    if (this.editingId == null) {
      this.appointmentsService.createAppointment(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.appointmentsService
        .updateAppointment(this.editingId, {
          facilityId: payload.facilityId,
          doctorId: payload.doctorId,
          departmentId: payload.departmentId,
          scheduledStart: payload.scheduledStart,
          scheduledEnd: payload.scheduledEnd,
          reason: payload.reason,
        })
        .subscribe({
          next: () => {
            this.showForm = false;
            this.load();
          },
        });
    }
  }

  changeStatus(a: AppointmentDto, status: string): void {
    this.appointmentsService.changeStatus(a.id, status).subscribe({
      next: () => {
        this.load();
        if (this.viewMode === 'calendar') {
          this.loadCalendar();
        }
      },
    });
  }

  cancelAppointment(a: AppointmentDto): void {
    if (a.status === 'Cancelled' || a.status === 4) return;
    this.cancelModalAppointment = a;
    this.cancelModalOpen = true;
  }

  closeCancelModal(): void {
    this.cancelModalOpen = false;
    this.cancelModalAppointment = null;
  }

  confirmCancelAppointment(): void {
    if (!this.cancelModalAppointment) return;
    const a = this.cancelModalAppointment;
    this.closeCancelModal();
    this.changeStatus(a, 'Cancelled');
  }

  createVisitFromAppointment(a: AppointmentDto): void {
    // Only doctors (without Reception role) should be able to start visits from appointments.
    if (!this.isDoctorView) {
      return;
    }
    this.router.navigate(['/visits'], {
      queryParams: {
        patientId: a.patientId,
        fromAppointmentId: a.id,
      },
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

  getDepartmentName(id: number | null | undefined): string {
    if (id == null) return '-';
    if (this.isDoctorView && this.currentDoctor && id === this.currentDoctor.departmentId)
      return this.currentDoctor.departmentName ?? String(id);
    const d = this.departments.find((x) => x.id === id);
    return d ? d.name : String(id);
  }

  // Calendar helpers
  setViewMode(mode: 'list' | 'calendar'): void {
    if (this.viewMode === mode) return;
    this.viewMode = mode;
    if (mode === 'calendar') {
      this.buildCalendar(this.calendarCurrent);
      this.loadCalendar();
    }
  }

  onCalendarDoctorChange(): void {
    if (this.viewMode === 'calendar') {
      this.loadCalendar();
    }
  }

  prevMonth(): void {
    this.calendarCurrent = new Date(
      this.calendarCurrent.getFullYear(),
      this.calendarCurrent.getMonth() - 1,
      1,
    );
    this.buildCalendar(this.calendarCurrent);
    this.loadCalendar();
  }

  nextMonth(): void {
    this.calendarCurrent = new Date(
      this.calendarCurrent.getFullYear(),
      this.calendarCurrent.getMonth() + 1,
      1,
    );
    this.buildCalendar(this.calendarCurrent);
    this.loadCalendar();
  }

  todayMonth(): void {
    this.calendarCurrent = new Date();
    this.buildCalendar(this.calendarCurrent);
    this.loadCalendar();
  }

  private buildCalendar(center: Date): void {
    const year = center.getFullYear();
    const month = center.getMonth();
    const firstOfMonth = new Date(year, month, 1);
    const lastOfMonth = new Date(year, month + 1, 0);

    // Start on Monday
    const startDay = (firstOfMonth.getDay() + 6) % 7;
    const startDate = new Date(firstOfMonth);
    startDate.setDate(firstOfMonth.getDate() - startDay);

    const endDay = (lastOfMonth.getDay() + 6) % 7;
    const endDate = new Date(lastOfMonth);
    endDate.setDate(lastOfMonth.getDate() + (6 - endDay));

    const weeks: { date: Date; inMonth: boolean }[][] = [];
    let cursor = new Date(startDate);
    while (cursor <= endDate) {
      const week: { date: Date; inMonth: boolean }[] = [];
      for (let i = 0; i < 7; i++) {
        week.push({
          date: new Date(cursor),
          inMonth: cursor.getMonth() === month,
        });
        cursor.setDate(cursor.getDate() + 1);
      }
      weeks.push(week);
    }
    this.calendarWeeks = weeks;
  }

  private loadCalendar(): void {
    this.calendarLoading = true;
    const year = this.calendarCurrent.getFullYear();
    const month = this.calendarCurrent.getMonth();
    const from = new Date(year, month, 1).toISOString().substring(0, 10);
    const to = new Date(year, month + 1, 0).toISOString().substring(0, 10);

    if (!this.isDoctorView && !this.calendarDoctorId) {
      this.calendarAppointments = [];
      this.calendarSlotsByDate = {};
      this.calendarLoading = false;
      return;
    }

    if (!this.isDoctorView) {
      this.doctorsService
        .getAvailableSlots({
          staffMemberId: this.calendarDoctorId!,
          from,
          to,
        })
        .subscribe({
          next: (res) => {
            this.calendarLoading = false;
            this.calendarSlotsByDate = {};
            for (const day of res.days) {
              const key =
                typeof (day.date as any) === 'string'
                  ? (day.date as any).substring(0, 10)
                  : new Date(day.date as any).toISOString().substring(0, 10);
              this.calendarSlotsByDate[key] = day.slots;
            }
          },
          error: () => {
            this.calendarLoading = false;
            this.calendarSlotsByDate = {};
          },
        });
      return;
    }

    this.appointmentsService
      .getAppointments({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'date',
        sortDesc: false,
        search: null,
        facilityId: this.facilityFilter,
        patientId: this.patientFilter,
        doctorId: this.calendarDoctorId,
        departmentId: this.departmentFilter,
        from,
        to,
        status: this.statusFilter,
      })
      .subscribe({
        next: (res) => {
          this.calendarLoading = false;
          this.calendarAppointments = res.items;
        },
        error: () => {
          this.calendarLoading = false;
          this.calendarAppointments = [];
        },
      });
  }

  getAppointmentsForDate(date: Date): AppointmentDto[] {
    const target = date.toISOString().substring(0, 10);
    return this.calendarAppointments.filter(
      (a) =>
        a.scheduledStart &&
        a.scheduledStart.substring(0, 10) === target,
    );
  }

  openCalendarCreate(date: Date, slot?: DoctorCalendarSlotDto): void {
    if (!this.isDoctorView && !this.calendarDoctorId) {
      return;
    }
    const doctorId = this.isDoctorView
      ? this.currentDoctor?.staffMemberId ?? null
      : this.calendarDoctorId;
    const departmentId = this.isDoctorView
      ? this.currentDoctor?.departmentId ?? null
      : (this.doctors.find((d) => d.staffMemberId === doctorId)?.departmentId ?? null);

    // For reception slot calendar: default to first available slot for the day.
    if (!this.isDoctorView) {
      const daySlots = this.getSlotsForDate(date);
      const chosen = slot ?? daySlots.find((s) => s.isAvailable);
      if (!chosen) return;

      {
        const startDate = new Date(chosen.slotStart);
        const endDate = new Date(chosen.slotEnd);
        this.editingId = null;
        this.form.reset({
          patientId: null,
          doctorId,
          facilityId: departmentId != null ? (this.departments.find((x) => x.id === departmentId)?.facilityId ?? null) : null,
          departmentId,
          scheduledStart: this.toLocalDateTimeInput(startDate),
          scheduledEnd: this.toLocalDateTimeInput(endDate),
          reason: '',
        });
        this.resetPatientPicker();
        this.calendarCreateOpen = true;
        return;
      }
    }

    const startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 9, 0, 0);
    const endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 9, 30, 0);
    this.editingId = null;
    this.form.reset({
      patientId: null,
      doctorId,
      facilityId: departmentId != null ? (this.departments.find((x) => x.id === departmentId)?.facilityId ?? null) : null,
      departmentId,
      scheduledStart: this.toLocalDateTimeInput(startDate),
      scheduledEnd: this.toLocalDateTimeInput(endDate),
      reason: '',
    });
    this.resetPatientPicker();
    this.calendarCreateOpen = true;
  }

  getSlotsForDate(date: Date): DoctorCalendarSlotDto[] {
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
    return this.calendarSlotsByDate[key] ?? [];
  }

  closeCalendarCreate(): void {
    this.calendarCreateOpen = false;
  }

  submitCalendarCreate(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.value;
    this.appointmentsService
      .createAppointment({
        patientId: Number(v.patientId),
        doctorId: v.doctorId ? Number(v.doctorId) : null,
        departmentId: v.departmentId ? Number(v.departmentId) : null,
        scheduledStart: v.scheduledStart!,
        scheduledEnd: v.scheduledEnd || null,
        reason: v.reason || null,
      })
      .subscribe({
        next: () => {
          this.calendarCreateOpen = false;
          this.load();
          this.loadCalendar();
        },
      });
  }

  getCalendarDoctorName(): string {
    if (this.isDoctorView && this.currentDoctor) return this.currentDoctor.fullName;
    if (this.calendarDoctorId == null) return 'Select doctor';
    const d = this.doctors.find((x) => x.staffMemberId === this.calendarDoctorId);
    return d ? d.fullName : String(this.calendarDoctorId);
  }

  private toLocalDateTimeInput(d: Date): string {
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }

  onFacilityChange(): void {
    const facilityId = this.form.value.facilityId ?? null;
    this.filteredDepartments = this.departments.filter((x) => x.facilityId === facilityId);
    const currentDepartmentId = this.form.value.departmentId ?? null;
    if (currentDepartmentId != null && !this.filteredDepartments.some((x) => x.id === currentDepartmentId)) {
      this.form.patchValue({ departmentId: null });
    }
  }

  onFacilityFormInputChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.form.patchValue({ facilityId: null });
      this.onFacilityChange();
      return;
    }
    const selected = this.facilities.find((f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase());
    this.form.patchValue({ facilityId: selected?.id ?? null });
    this.onFacilityChange();
  }

  onFacilityFilterInputChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.facilityFilter = null;
      this.applyFilters();
      return;
    }
    const selected = this.facilities.find((f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase());
    this.facilityFilter = selected?.id ?? null;
    this.applyFilters();
  }

  getFacilityOptionLabel(facility: FacilityDto): string {
    return facility.code ? `${facility.name} (${facility.code})` : facility.name;
  }

  private syncFacilityAndDepartment(): void {
    const departmentId = this.form.value.departmentId ?? null;
    if (departmentId != null) {
      const dep = this.departments.find((x) => x.id === departmentId);
      if (dep) {
        this.form.patchValue({ facilityId: dep.facilityId }, { emitEvent: false });
      }
    }
    this.onFacilityChange();
  }
}

