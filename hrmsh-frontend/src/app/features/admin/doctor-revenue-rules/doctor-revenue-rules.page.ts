import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { DoctorsService } from '../../doctors/doctors.service';
import { DoctorDto } from '../../doctors/doctors.api';

interface DoctorRevenueRuleDto {
  id: number;
  doctorId: number | null;
  doctorName: string | null;
  departmentId: number | null;
  departmentName: string | null;
  serviceItemId: number | null;
  serviceItemName: string | null;
  minVisitsPerDay: number;
  maxVisitsPerDay: number | null;
  doctorSharePercent: number;
  hospitalSharePercent: number;
  isActive: boolean;
}

@Component({
  selector: 'app-doctor-revenue-rules',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-revenue-rules.page.html',
  styleUrl: './doctor-revenue-rules.page.scss',
})
export class DoctorRevenueRulesPage implements OnInit {
  loading = false;
  saving = false;
  error: string | null = null;

  rules: DoctorRevenueRuleDto[] = [];

  editing: DoctorRevenueRuleDto | null = null;

  doctors: DoctorDto[] = [];

  form: {
    id: number | null;
    doctorId: number | null;
    departmentId: number | null;
    serviceItemId: number | null;
    minVisitsPerDay: number;
    maxVisitsPerDay: number | null;
    doctorSharePercent: number;
    hospitalSharePercent: number;
    isActive: boolean;
  } = this.emptyForm();

  constructor(
    private readonly api: ApiService,
    private readonly doctorsService: DoctorsService,
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadDoctors();
  }

  private loadDoctors(): void {
    this.doctorsService
      .getDoctors({
        pageNumber: 1,
        pageSize: 200,
        isActive: true,
      })
      .subscribe({
        next: (res) => {
          this.doctors = res.items ?? [];
        },
        error: () => {
          this.doctors = [];
        },
      });
  }

  private emptyForm() {
    return {
      id: null,
      doctorId: null,
      departmentId: null,
      serviceItemId: null,
      minVisitsPerDay: 0,
      maxVisitsPerDay: null,
      doctorSharePercent: 50,
      hospitalSharePercent: 50,
      isActive: true,
    };
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.api
      .get<{ success: boolean; data: DoctorRevenueRuleDto[] }>(
        'DoctorRevenueRules',
      )
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.rules = res.data ?? [];
        },
        error: (err) => {
          this.loading = false;
          this.error =
            err?.error?.message || err?.message || 'Failed to load rules.';
        },
      });
  }

  startCreate(): void {
    this.editing = null;
    this.form = this.emptyForm();
  }

  startEdit(rule: DoctorRevenueRuleDto): void {
    this.editing = rule;
    this.form = {
      id: rule.id,
      doctorId: rule.doctorId,
      departmentId: rule.departmentId,
      serviceItemId: rule.serviceItemId,
      minVisitsPerDay: rule.minVisitsPerDay,
      maxVisitsPerDay: rule.maxVisitsPerDay,
      doctorSharePercent: rule.doctorSharePercent,
      hospitalSharePercent: rule.hospitalSharePercent,
      isActive: rule.isActive,
    };
  }

  syncHospitalShare(): void {
    if (
      this.form.doctorSharePercent >= 0 &&
      this.form.doctorSharePercent <= 100
    ) {
      this.form.hospitalSharePercent = 100 - this.form.doctorSharePercent;
    }
  }

  submit(): void {
    this.saving = true;
    this.error = null;

    const payload = {
      id: this.form.id,
      doctorId: this.form.doctorId,
      departmentId: this.form.departmentId,
      serviceItemId: this.form.serviceItemId,
      minVisitsPerDay: this.form.minVisitsPerDay,
      maxVisitsPerDay: this.form.maxVisitsPerDay,
      doctorSharePercent: this.form.doctorSharePercent,
      hospitalSharePercent: this.form.hospitalSharePercent,
      isActive: this.form.isActive,
    };

    this.api
      .post<{ success: boolean; data: DoctorRevenueRuleDto }>(
        'DoctorRevenueRules',
        payload,
      )
      .subscribe({
        next: (res) => {
          this.saving = false;
          const updated = res.data;
          if (!updated) {
            this.load();
            return;
          }
          const idx = this.rules.findIndex((r) => r.id === updated.id);
          if (idx >= 0) {
            this.rules[idx] = updated;
          } else {
            this.rules.push(updated);
          }
          this.rules = [...this.rules];
          this.startCreate();
        },
        error: (err) => {
          this.saving = false;
          this.error =
            err?.error?.message || err?.message || 'Failed to save rule.';
        },
      });
  }
}

