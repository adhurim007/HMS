import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

interface VisitsPerDoctorRowDto {
  doctorId: number;
  doctorName?: string | null;
  visitCount: number;
}

@Component({
  selector: 'app-visits-per-doctor-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './visits-per-doctor.page.html',
  styleUrl: './visits-per-doctor.page.scss',
})
export class VisitsPerDoctorReportPage implements OnInit {
  from!: string;
  to!: string;

  loading = false;
  rows: VisitsPerDoctorRowDto[] = [];

  // SSRS / Report Server URL for the VisitsPerDoctor RDL
  // Host name aligned with your SQL Server machine.
  readonly rdlReportUrl =
    'http://ASULEJMANI/ReportServer?/HRMSH/VisitsPerDoctor';

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(today.getMonth() - 1);
    this.from = monthAgo.toISOString().substring(0, 10);
    this.to = today.toISOString().substring(0, 10);
    this.load();
  }

  load(): void {
    this.loading = true;
    const params: Record<string, string> = {};
    if (this.from) params['from'] = this.from;
    if (this.to) params['to'] = this.to;

    this.api
      .get<{ success: boolean; data: VisitsPerDoctorRowDto[] }>(
        'Reports/visits-per-doctor',
        params,
      )
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.rows = res.data ?? [];
        },
        error: () => {
          this.loading = false;
          this.rows = [];
        },
      });
  }

  get totalVisits(): number {
    return this.rows.reduce((sum, r) => sum + r.visitCount, 0);
  }

  get sortedRows(): VisitsPerDoctorRowDto[] {
    return [...this.rows].sort((a, b) => b.visitCount - a.visitCount);
  }

  get periodLabel(): string {
    if (!this.from || !this.to) return '';
    return `${this.from} – ${this.to}`;
  }

  print(): void {
    window.print();
  }

  openRdl(): void {
    if (!this.rdlReportUrl) {
      return;
    }

    const params: string[] = [];
    if (this.from) {
      params.push(`FromDateParam=${encodeURIComponent(this.from)}`);
    }
    if (this.to) {
      params.push(`ToDateParam=${encodeURIComponent(this.to)}`);
    }

    const url =
      this.rdlReportUrl + (params.length ? `&${params.join('&')}` : '');

    window.open(url, '_blank');
  }
}

