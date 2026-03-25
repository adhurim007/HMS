import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { DoctorsService } from '../doctors/doctors.service';
import { DoctorDto } from '../doctors/doctors.api';
import { AuthService } from '../../core/services/auth.service';

interface DoctorDailyRevenueRow {
  date: string;
  totalVisits: number;
  totalAmount: number;
  doctorAmount: number;
  hospitalAmount: number;
}

@Component({
  selector: 'app-doctor-revenue-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-revenue.page.html',
  styleUrl: './doctor-revenue.page.scss',
})
export class DoctorRevenueReportPage implements OnInit {
  from!: string;
  to!: string;

  doctors: DoctorDto[] = [];
  doctorId: number | null = null;
  doctorName: string | null = null;
  private isDoctor = false;

  loading = false;
  error: string | null = null;
  rows: DoctorDailyRevenueRow[] = [];

  // Adjust to your actual SSRS URL and folder for the doctor daily revenue RDL
  readonly rdlReportUrl =
    'http://ASULEJMANI/ReportServer?/HRMSH/DoctorDailyRevenue';

  constructor(
    private readonly api: ApiService,
    private readonly doctorsService: DoctorsService,
    private readonly auth: AuthService,
  ) {}

  ngOnInit(): void {
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(today.getMonth() - 1);
    this.from = monthAgo.toISOString().substring(0, 10);
    this.to = today.toISOString().substring(0, 10);

    // First try to load current doctor profile.
    // If this user is not a doctor, we fall back to loading all doctors.
    this.loadMeDoctor();
  }

  get isDoctorView(): boolean {
    return this.isDoctor;
  }

  private loadMeDoctor(): void {
    this.doctorsService.getMe().subscribe({
      next: (me) => {
        // User is a doctor: lock dropdown to this doctor.
        this.isDoctor = true;
        this.doctorId = me.staffMemberId;
        this.doctorName = me.fullName;

        this.doctors = [
          {
            staffMemberId: me.staffMemberId,
            fullName: me.fullName,
            specialty: null,
            licenseNumber: null,
            departmentId: me.departmentId ?? null,
            phone: null,
            email: null,
            isActive: true,
          },
        ];

        this.load();
      },
      error: () => {
        // Not a doctor (or no profile): treat as non-doctor and enable dropdown.
        this.isDoctor = false;
        this.doctorId = null;
        this.doctorName = null;
        this.error = null;
        this.loadDoctors();
      },
    });
  }

  private loadDoctors(): void {
    this.doctorsService
      .getDoctors({ pageNumber: 1, pageSize: 200, isActive: true })
      .subscribe({
        next: (res) => {
          this.doctors = res.items ?? [];
        },
        error: () => {
          this.doctors = [];
        },
      });
  }

  load(): void {
    if (!this.doctorId || !this.from || !this.to) {
      this.error = 'Select doctor and period.';
      return;
    }

    this.loading = true;
    this.error = null;

    const params: Record<string, string> = {
      doctorId: String(this.doctorId),
      from: this.from,
      to: this.to,
    };

    this.api
      .get<{
        success: boolean;
        data: DoctorDailyRevenueRow[];
        Data?: DoctorDailyRevenueRow[];
      }>('DoctorRevenueShares/daily-list', params)
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.rows = res.data ?? res.Data ?? [];
        },
        error: (err) => {
          this.loading = false;
          this.error =
            err?.error?.message || err?.message || 'Failed to load revenue.';
        },
      });
  }

  openRdl(row: DoctorDailyRevenueRow): void {
    if (!this.rdlReportUrl || !this.doctorId) {
      return;
    }

    const date = row.date.substring(0, 10);
    const url =
      this.rdlReportUrl +
      `&DoctorIdParam=${encodeURIComponent(String(this.doctorId))}` +
      `&FromDateParam=${encodeURIComponent(date)}` +
      `&ToDateParam=${encodeURIComponent(date)}`;

    window.open(url, '_blank');
  }
}

