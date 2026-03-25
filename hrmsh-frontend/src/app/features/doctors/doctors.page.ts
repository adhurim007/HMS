import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DoctorsService } from './doctors.service';
import { DoctorDto } from './doctors.api';
import { DepartmentsService } from '../admin/departments/departments.service';
import { DepartmentDto } from '../admin/departments/departments.api';

@Component({
  selector: 'app-doctors-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './doctors.page.html',
  styleUrl: './doctors.page.scss',
})
export class DoctorsPage implements OnInit {
  doctors: DoctorDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = 'name';
  sortDesc = false;
  loading = false;

  departmentFilter: number | null = null;
  isActiveFilter: boolean | null = null;

  departments: DepartmentDto[] = [];

  editingId: number | null = null;
  showForm = false;

  private visitSettingsId: number | null = null;

  readonly form = this.fb.group({
    fullName: ['', [Validators.required]],
    phone: [''],
    email: [''],
    password: [''],
    departmentId: [null as number | null],
    specialty: [''],
    licenseNumber: [''],
    isActive: [true],
    minVisitDurationMinutes: [
      30,
      [Validators.required, Validators.min(1), Validators.max(720)],
    ],

    monEnabled: [true],
    monStart: ['09:00'],
    monEnd: ['17:00'],
    tueEnabled: [true],
    tueStart: ['09:00'],
    tueEnd: ['17:00'],
    wedEnabled: [true],
    wedStart: ['09:00'],
    wedEnd: ['17:00'],
    thuEnabled: [true],
    thuStart: ['09:00'],
    thuEnd: ['17:00'],
    friEnabled: [true],
    friStart: ['09:00'],
    friEnd: ['17:00'],
    satEnabled: [false],
    satStart: ['09:00'],
    satEnd: ['17:00'],
    sunEnabled: [false],
    sunStart: ['09:00'],
    sunEnd: ['17:00'],
  });

  constructor(
    private readonly doctorsService: DoctorsService,
    private readonly departmentsService: DepartmentsService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.load();
  }

  loadLookups(): void {
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
          this.departments = res.items;
        },
      });
  }

  load(): void {
    this.loading = true;
    this.doctorsService
      .getDoctors({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
        departmentId: this.departmentFilter,
        isActive: this.isActiveFilter,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.doctors = res.items;
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

  applyFilters(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.departmentFilter = null;
    this.isActiveFilter = null;
    this.search = '';
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
    this.visitSettingsId = null;
    this.form.reset({
      fullName: '',
      phone: '',
      email: '',
      password: '',
      departmentId: null,
      specialty: '',
      licenseNumber: '',
      isActive: true,
      minVisitDurationMinutes: 30,

      monEnabled: true,
      monStart: '09:00',
      monEnd: '17:00',
      tueEnabled: true,
      tueStart: '09:00',
      tueEnd: '17:00',
      wedEnabled: true,
      wedStart: '09:00',
      wedEnd: '17:00',
      thuEnabled: true,
      thuStart: '09:00',
      thuEnd: '17:00',
      friEnabled: true,
      friStart: '09:00',
      friEnd: '17:00',
      satEnabled: false,
      satStart: '09:00',
      satEnd: '17:00',
      sunEnabled: false,
      sunStart: '09:00',
      sunEnd: '17:00',
    });
    this.form.get('email')?.setValidators([Validators.required, Validators.email]);
    this.form.get('password')?.setValidators([Validators.required]);
    this.showForm = true;
  }

  openEdit(d: DoctorDto): void {
    this.editingId = d.staffMemberId;
    this.visitSettingsId = null;
    this.form.reset({
      fullName: d.fullName,
      phone: d.phone ?? '',
      email: d.email ?? '',
      password: '',
      departmentId: d.departmentId ?? null,
      specialty: d.specialty ?? '',
      licenseNumber: d.licenseNumber ?? '',
      isActive: d.isActive,
      minVisitDurationMinutes: 30,

      monEnabled: true,
      monStart: '09:00',
      monEnd: '17:00',
      tueEnabled: true,
      tueStart: '09:00',
      tueEnd: '17:00',
      wedEnabled: true,
      wedStart: '09:00',
      wedEnd: '17:00',
      thuEnabled: true,
      thuStart: '09:00',
      thuEnd: '17:00',
      friEnabled: true,
      friStart: '09:00',
      friEnd: '17:00',
      satEnabled: false,
      satStart: '09:00',
      satEnd: '17:00',
      sunEnabled: false,
      sunStart: '09:00',
      sunEnd: '17:00',
    });
    this.form.get('email')?.clearValidators();
    this.form.get('password')?.clearValidators();
    this.showForm = true;

    this.doctorsService.getVisitSettings(d.staffMemberId).subscribe({
      next: (res) => {
        if (res) {
          this.visitSettingsId = res.id;
          this.form.patchValue({
            minVisitDurationMinutes: res.minVisitDurationMinutes,
          });
        } else {
          this.visitSettingsId = null;
          this.form.patchValue({ minVisitDurationMinutes: 30 });
        }
      },
      error: () => {
        // Keep defaults if settings couldn't be loaded.
      },
    });

    this.doctorsService.getWeeklySchedule(d.staffMemberId).subscribe({
      next: (res) => {
        const patch: any = {};
        for (const day of res.days) {
          const enabled = day.isWorking;
          const start = day.startTime ?? '09:00';
          const end = day.endTime ?? '17:00';
          switch (day.dayOfWeek) {
            case 1: patch.monEnabled = enabled; patch.monStart = start; patch.monEnd = end; break; // Mon
            case 2: patch.tueEnabled = enabled; patch.tueStart = start; patch.tueEnd = end; break; // Tue
            case 3: patch.wedEnabled = enabled; patch.wedStart = start; patch.wedEnd = end; break; // Wed
            case 4: patch.thuEnabled = enabled; patch.thuStart = start; patch.thuEnd = end; break; // Thu
            case 5: patch.friEnabled = enabled; patch.friStart = start; patch.friEnd = end; break; // Fri
            case 6: patch.satEnabled = enabled; patch.satStart = start; patch.satEnd = end; break; // Sat
            case 0: patch.sunEnabled = enabled; patch.sunStart = start; patch.sunEnd = end; break; // Sun
          }
        }
        this.form.patchValue(patch);
      },
      error: () => {
        // Keep defaults.
      },
    });
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

    if (this.editingId == null) {
      const email = (v.email ?? '').toString().trim();
      const password = (v.password ?? '').toString();
      if (!email || !password) {
        this.form.get('email')?.markAsTouched();
        this.form.get('password')?.markAsTouched();
        return;
      }
      this.doctorsService
        .createDoctorWithAccount({
          email,
          password,
          fullName: v.fullName!,
          phone: v.phone || null,
          departmentId: v.departmentId ?? null,
          specialty: v.specialty || null,
          licenseNumber: v.licenseNumber || null,
        })
        .subscribe({
          next: (created) => {
            const minDuration = Number(v.minVisitDurationMinutes);
            const days = [
              { dayOfWeek: 1, isWorking: !!v.monEnabled, startTime: v.monStart, endTime: v.monEnd },
              { dayOfWeek: 2, isWorking: !!v.tueEnabled, startTime: v.tueStart, endTime: v.tueEnd },
              { dayOfWeek: 3, isWorking: !!v.wedEnabled, startTime: v.wedStart, endTime: v.wedEnd },
              { dayOfWeek: 4, isWorking: !!v.thuEnabled, startTime: v.thuStart, endTime: v.thuEnd },
              { dayOfWeek: 5, isWorking: !!v.friEnabled, startTime: v.friStart, endTime: v.friEnd },
              { dayOfWeek: 6, isWorking: !!v.satEnabled, startTime: v.satStart, endTime: v.satEnd },
              { dayOfWeek: 0, isWorking: !!v.sunEnabled, startTime: v.sunStart, endTime: v.sunEnd },
            ];
            this.doctorsService
              .upsertVisitSettings({
                id: null,
                staffMemberId: created.staffMemberId,
                minVisitDurationMinutes: minDuration,
              })
              .subscribe({
                next: () => {
                  this.doctorsService
                    .upsertWeeklySchedule({
                      staffMemberId: created.staffMemberId,
                      days,
                    })
                    .subscribe({
                      next: () => {
                        this.showForm = false;
                        this.load();
                      },
                      error: (err) => {
                        const msg =
                          err?.error?.message ??
                          err?.message ??
                          'Failed to save weekly schedule.';
                        alert(msg);
                      },
                    });
                },
                error: (err) => {
                  const msg =
                    err?.error?.message ??
                    err?.message ??
                    'Failed to save visit settings.';
                  alert(msg);
                },
              });
          },
          error: (err) => {
            const msg = err?.error?.message ?? err?.message ?? 'Failed to create doctor.';
            alert(msg);
          },
        });
    } else {
      this.doctorsService
        .updateDoctor(this.editingId, {
          fullName: v.fullName!,
          phone: v.phone || null,
          email: v.email || null,
          departmentId: v.departmentId ?? null,
          isActive: !!v.isActive,
          specialty: v.specialty || null,
          licenseNumber: v.licenseNumber || null,
        })
        .subscribe({
          next: () => {
            const minDuration = Number(v.minVisitDurationMinutes);
            const days = [
              { dayOfWeek: 1, isWorking: !!v.monEnabled, startTime: v.monStart, endTime: v.monEnd },
              { dayOfWeek: 2, isWorking: !!v.tueEnabled, startTime: v.tueStart, endTime: v.tueEnd },
              { dayOfWeek: 3, isWorking: !!v.wedEnabled, startTime: v.wedStart, endTime: v.wedEnd },
              { dayOfWeek: 4, isWorking: !!v.thuEnabled, startTime: v.thuStart, endTime: v.thuEnd },
              { dayOfWeek: 5, isWorking: !!v.friEnabled, startTime: v.friStart, endTime: v.friEnd },
              { dayOfWeek: 6, isWorking: !!v.satEnabled, startTime: v.satStart, endTime: v.satEnd },
              { dayOfWeek: 0, isWorking: !!v.sunEnabled, startTime: v.sunStart, endTime: v.sunEnd },
            ];
            this.doctorsService
              .upsertVisitSettings({
                id: this.visitSettingsId,
                staffMemberId: this.editingId!,
                minVisitDurationMinutes: minDuration,
              })
              .subscribe({
                next: () => {
                  this.doctorsService
                    .upsertWeeklySchedule({
                      staffMemberId: this.editingId!,
                      days,
                    })
                    .subscribe({
                      next: () => {
                        this.showForm = false;
                        this.load();
                      },
                      error: (err) => {
                        const msg =
                          err?.error?.message ??
                          err?.message ??
                          'Failed to save weekly schedule.';
                        alert(msg);
                      },
                    });
                },
                error: (err) => {
                  const msg =
                    err?.error?.message ??
                    err?.message ??
                    'Failed to save visit settings.';
                  alert(msg);
                },
              });
          },
        });
    }
  }

  toggleActive(d: DoctorDto): void {
    this.doctorsService.setActive(d.staffMemberId, !d.isActive).subscribe({
      next: () => this.load(),
    });
  }

  getDepartmentName(id: number | null | undefined): string {
    if (id == null) return '-';
    const dep = this.departments.find((x) => x.id === id);
    return dep ? dep.name : String(id);
  }
}
