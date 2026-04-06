import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PatientsService } from './patients.service';
import { PatientDto } from './patients.api';

@Component({
  selector: 'app-patients-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './patients.page.html',
  styleUrl: './patients.page.scss',
})
export class PatientsPage implements OnInit {
  patients: PatientDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = null;
  sortDesc = false;
  loading = false;

  genders = [
    { value: 0, label: 'Unknown' },
    { value: 1, label: 'Male' },
    { value: 2, label: 'Female' },
  ];

  editingId: number | null = null;
  showForm = false;

  readonly form = this.fb.group({
    medicalRecordNumber: ['', Validators.required],
    fullName: ['', Validators.required],
    dateOfBirth: [''],
    gender: [1, Validators.required],
    phone: [''],
    email: ['', Validators.email],
    address: [''],
    bloodGroup: [''],
    chronicConditions: [''],
    allergies: [''],
    parentGuardianName: [''],
    pediatricMtl: ['' as string | number],
    pediatricGjtl: ['' as string | number],
    pediatricPkl: ['' as string | number],
    priorLiveBirth: [null as boolean | null],
    priorAbortion: [null as boolean | null],
  });

  constructor(
    private readonly patientsService: PatientsService,
    private readonly fb: FormBuilder,
  ) {}

  readonly math = Math;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.patientsService
      .getPatients({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.patients = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  onSearchChange(value: string): void {
    this.search = value;
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
    this.form.reset({
      medicalRecordNumber: '',
      fullName: '',
      dateOfBirth: '',
      gender: 1,
      phone: '',
      email: '',
      address: '',
      bloodGroup: '',
      chronicConditions: '',
      allergies: '',
      parentGuardianName: '',
      pediatricMtl: '',
      pediatricGjtl: '',
      pediatricPkl: '',
      priorLiveBirth: null,
      priorAbortion: null,
    });
    this.showForm = true;
  }

  openEdit(patient: PatientDto): void {
    this.editingId = patient.id;
    this.form.reset({
      medicalRecordNumber: patient.medicalRecordNumber,
      fullName: patient.fullName,
      dateOfBirth: patient.dateOfBirth
        ? patient.dateOfBirth.substring(0, 10)
        : '',
      gender: typeof patient.gender === 'number' ? patient.gender : 1,
      phone: patient.phone ?? '',
      email: patient.email ?? '',
      address: patient.address ?? '',
      bloodGroup: patient.bloodGroup ?? '',
      chronicConditions: patient.chronicConditions ?? '',
      allergies: patient.allergies ?? '',
      parentGuardianName: patient.parentGuardianName ?? '',
      pediatricMtl: patient.pediatricMtl ?? '',
      pediatricGjtl: patient.pediatricGjtl ?? '',
      pediatricPkl: patient.pediatricPkl ?? '',
      priorLiveBirth: patient.priorLiveBirth ?? null,
      priorAbortion: patient.priorAbortion ?? null,
    });
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value;
    const numOrNull = (v: string | number | null | undefined) => {
      if (v === '' || v === null || v === undefined) return null;
      const n = Number(v);
      return Number.isFinite(n) ? n : null;
    };

    const payload = {
      medicalRecordNumber: value.medicalRecordNumber!,
      fullName: value.fullName!,
      dateOfBirth: value.dateOfBirth || null,
      gender: Number(value.gender),
      phone: value.phone || null,
      email: value.email || null,
      address: value.address || null,
      bloodGroup: value.bloodGroup || null,
      chronicConditions: value.chronicConditions || null,
      allergies: value.allergies || null,
      parentGuardianName: value.parentGuardianName?.trim() || null,
      pediatricMtl: numOrNull(value.pediatricMtl),
      pediatricGjtl: numOrNull(value.pediatricGjtl),
      pediatricPkl: numOrNull(value.pediatricPkl),
      priorLiveBirth: value.priorLiveBirth ?? null,
      priorAbortion: value.priorAbortion ?? null,
    };

    if (this.editingId == null) {
      this.patientsService.createPatient(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.patientsService
        .updatePatient(this.editingId, {
          fullName: payload.fullName,
          dateOfBirth: payload.dateOfBirth,
          gender: payload.gender,
          phone: payload.phone,
          email: payload.email,
          address: payload.address,
          bloodGroup: payload.bloodGroup,
          chronicConditions: payload.chronicConditions,
          allergies: payload.allergies,
          parentGuardianName: payload.parentGuardianName,
          pediatricMtl: payload.pediatricMtl,
          pediatricGjtl: payload.pediatricGjtl,
          pediatricPkl: payload.pediatricPkl,
          priorLiveBirth: payload.priorLiveBirth,
          priorAbortion: payload.priorAbortion,
        })
        .subscribe({
          next: () => {
            this.showForm = false;
            this.load();
          },
        });
    }
  }

  delete(patient: PatientDto): void {
    if (!confirm(`Delete patient ${patient.fullName}?`)) return;
    this.patientsService.deletePatient(patient.id).subscribe({
      next: () => this.load(),
    });
  }
}


