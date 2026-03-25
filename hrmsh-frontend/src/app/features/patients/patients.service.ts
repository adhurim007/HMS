import { Injectable } from '@angular/core';
import { ApiService } from '../../core/services/api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PagedApiResponse, PatientDto } from './patients.api';

export interface PatientsQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
}

@Injectable({ providedIn: 'root' })
export class PatientsService {
  constructor(private readonly api: ApiService) {}

  getPatients(query: PatientsQuery): Observable<PagedApiResponse<PatientDto>> {
    const params: Record<string, string> = {
      pageNumber: String(query.pageNumber),
      pageSize: String(query.pageSize),
    };
    if (query.sortBy) {
      params['sortBy'] = query.sortBy;
      params['sortDesc'] = String(!!query.sortDesc);
    }
    if (query.search) {
      params['search'] = query.search;
    }
    return this.api.get<PagedApiResponse<PatientDto>>('Patients', params);
  }

  getPatient(id: number): Observable<PatientDto> {
    return this.api
      .get<{ success: boolean; data: PatientDto }>(`Patients/${id}`)
      .pipe(map((x) => x.data));
  }

  getPatientByMrn(mrn: string): Observable<PatientDto> {
    return this.api
      .get<{ success: boolean; data: PatientDto }>(`Patients/by-mrn/${encodeURIComponent(mrn)}`)
      .pipe(map((x) => x.data));
  }

  createPatient(payload: {
    medicalRecordNumber: string;
    fullName: string;
    dateOfBirth?: string | null;
    gender: number;
    phone?: string | null;
    email?: string | null;
    address?: string | null;
    bloodGroup?: string | null;
    chronicConditions?: string | null;
    allergies?: string | null;
  }): Observable<PatientDto> {
    return this.api
      .post<{ success: boolean; data: PatientDto }>('Patients', payload)
      .pipe(map((x) => x.data));
  }

  updatePatient(
    id: number,
    payload: {
      fullName: string;
      dateOfBirth?: string | null;
      gender: number;
      phone?: string | null;
      email?: string | null;
      address?: string | null;
      bloodGroup?: string | null;
      chronicConditions?: string | null;
      allergies?: string | null;
    },
  ): Observable<PatientDto> {
    return this.api
      .put<{ success: boolean; data: PatientDto }>(`Patients/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deletePatient(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Patients/${id}`)
      .pipe(map(() => void 0));
  }
}

