import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DepartmentsService } from '../departments/departments.service';
import { DepartmentDto } from '../departments/departments.api';
import { DoctorsService } from '../../doctors/doctors.service';
import { DoctorDto } from '../../doctors/doctors.api';
import { BillingService } from '../../billing/billing.service';
import { ServiceItemListDto } from '../../billing/billing.api';
import { ApiService } from '../../../core/services/api.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-services-config-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './services-config.page.html',
  styleUrl: './services-config.page.scss',
})
export class ServicesConfigPage implements OnInit {
  activeTab: 'department' | 'doctor' = 'department';

  services: ServiceItemListDto[] = [];

  departments: DepartmentDto[] = [];
  selectedDepartmentId: number | null = null;
  deptSelectedIds = new Set<number>();
  loadingDept = false;
  savingDept = false;

  doctors: DoctorDto[] = [];
  selectedDoctorId: number | null = null;
  doctorSelectedIds = new Set<number>();
  loadingDoctor = false;
  savingDoctor = false;

  constructor(
    private readonly departmentsService: DepartmentsService,
    private readonly doctorsService: DoctorsService,
    private readonly billingService: BillingService,
    private readonly api: ApiService,
  ) {}

  ngOnInit(): void {
    this.loadServices();
    this.loadDepartments();
    this.loadDoctors();
  }

  setTab(tab: 'department' | 'doctor'): void {
    this.activeTab = tab;
  }

  private loadServices(): void {
    this.billingService
      .getServiceItems({
        page: 1,
        pageSize: 500,
        search: null,
        isActive: null,
        sortBy: 'name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.services = res.items ?? [];
        },
      });
  }

  private loadDepartments(): void {
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
        },
      });
  }

  private loadDoctors(): void {
    this.doctorsService
      .getDoctors({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'fullName',
        sortDesc: false,
        search: null,
        departmentId: null,
        isActive: true,
      })
      .subscribe({
        next: (res) => {
          this.doctors = res.items ?? [];
        },
      });
  }

  onDepartmentChange(): void {
    if (!this.selectedDepartmentId) {
      this.deptSelectedIds.clear();
      return;
    }
    this.loadingDept = true;
    this.api
      .get<{ success?: boolean; data?: number[]; Data?: number[] }>(
        `Services/department/${this.selectedDepartmentId}`,
      )
      .subscribe({
        next: (res) => {
          this.loadingDept = false;
          const ids = res.data ?? (res as any).Data ?? [];
          this.deptSelectedIds = new Set<number>(ids ?? []);
        },
        error: () => {
          this.loadingDept = false;
          this.deptSelectedIds.clear();
        },
      });
  }

  toggleDeptService(id: number, checked: boolean): void {
    if (checked) this.deptSelectedIds.add(id);
    else this.deptSelectedIds.delete(id);
  }

  saveDept(): void {
    if (!this.selectedDepartmentId) return;
    this.savingDept = true;
    this.api
      .put<{ success?: boolean; message?: string }>(
        `Services/department/${this.selectedDepartmentId}`,
        { serviceItemIds: Array.from(this.deptSelectedIds) },
      )
      .subscribe({
        next: () => {
          this.savingDept = false;
        },
        error: () => {
          this.savingDept = false;
        },
      });
  }

  onDoctorChange(): void {
    if (!this.selectedDoctorId) {
      this.doctorSelectedIds.clear();
      return;
    }
    this.loadingDoctor = true;
    this.api
      .get<{ success?: boolean; data?: number[]; Data?: number[] }>(
        `Services/doctor/${this.selectedDoctorId}`,
      )
      .subscribe({
        next: (res) => {
          this.loadingDoctor = false;
          const ids = res.data ?? (res as any).Data ?? [];
          this.doctorSelectedIds = new Set<number>(ids ?? []);
        },
        error: () => {
          this.loadingDoctor = false;
          this.doctorSelectedIds.clear();
        },
      });
  }

  toggleDoctorService(id: number, checked: boolean): void {
    if (checked) this.doctorSelectedIds.add(id);
    else this.doctorSelectedIds.delete(id);
  }

  saveDoctor(): void {
    if (!this.selectedDoctorId) return;
    this.savingDoctor = true;
    this.api
      .put<{ success?: boolean; message?: string }>(
        `Services/doctor/${this.selectedDoctorId}`,
        { serviceItemIds: Array.from(this.doctorSelectedIds) },
      )
      .subscribe({
        next: () => {
          this.savingDoctor = false;
        },
        error: () => {
          this.savingDoctor = false;
        },
      });
  }
}


