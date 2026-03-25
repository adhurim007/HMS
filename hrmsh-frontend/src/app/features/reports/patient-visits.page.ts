import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { VisitsService } from '../visits/visits.service';
import { VisitListDto } from '../visits/visits.api';
import { DoctorsService } from '../doctors/doctors.service';
import { DoctorDto } from '../doctors/doctors.api';

@Component({
  selector: 'app-patient-visits-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-visits.page.html',
  styleUrl: './patient-visits.page.scss',
})
export class PatientVisitsReportPage implements OnInit {
  patientId!: number;
  patient: PatientDto | null = null;

  from!: string;
  to!: string;

  visits: VisitListDto[] = [];
  loading = false;

  doctors: DoctorDto[] = [];

  constructor(
    private readonly route: ActivatedRoute,
    private readonly patientsService: PatientsService,
    private readonly visitsService: VisitsService,
    private readonly doctorsService: DoctorsService,
  ) {}

  ngOnInit(): void {
    const pid = this.route.snapshot.queryParamMap.get('patientId');
    if (!pid) {
      return;
    }
    this.patientId = Number(pid);
    if (!this.patientId || Number.isNaN(this.patientId)) {
      return;
    }

    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(today.getMonth() - 1);
    this.from = monthAgo.toISOString().substring(0, 10);
    this.to = today.toISOString().substring(0, 10);

    this.loadPatient();
    this.loadDoctors();
    this.loadVisits();
  }

  private loadPatient(): void {
    this.patientsService.getPatient(this.patientId).subscribe({
      next: (p) => (this.patient = p),
    });
  }

  private loadDoctors(): void {
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
          this.doctors = res.items ?? [];
        },
      });
  }

  loadVisits(): void {
    this.loading = true;
    this.visitsService
      .getVisits({
        patientId: this.patientId,
        doctorId: null,
        from: this.from || null,
        to: this.to || null,
        page: 1,
        pageSize: 200,
        sortBy: 'VisitDate',
        sortDescending: true,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.visits = res.items ?? [];
        },
        error: () => {
          this.loading = false;
          this.visits = [];
        },
      });
  }

  getDoctorName(id: number | null | undefined): string {
    if (id == null) return '-';
    const d = this.doctors.find((x) => x.staffMemberId === id);
    return d ? d.fullName : String(id);
  }

  print(): void {
    window.print();
  }
}

