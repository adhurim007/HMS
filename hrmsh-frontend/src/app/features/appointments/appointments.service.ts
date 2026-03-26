import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import { AppointmentDto, PagedApiResponse } from './appointments.api';

export interface AppointmentsQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
  facilityId?: number | null;
  patientId?: number | null;
  doctorId?: number | null;
  departmentId?: number | null;
  from?: string | null;
  to?: string | null;
  status?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AppointmentsService {
  constructor(private readonly api: ApiService) {}

  getAppointments(query: AppointmentsQuery): Observable<PagedApiResponse<AppointmentDto>> {
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
    if (query.facilityId != null) params['facilityId'] = String(query.facilityId);
    if (query.patientId != null) params['patientId'] = String(query.patientId);
    if (query.doctorId != null) params['doctorId'] = String(query.doctorId);
    if (query.departmentId != null)
      params['departmentId'] = String(query.departmentId);
    if (query.from) params['from'] = query.from;
    if (query.to) params['to'] = query.to;
    if (query.status) params['status'] = query.status as string;

    return this.api.get<PagedApiResponse<AppointmentDto>>('Appointments', params);
  }

  getAppointment(id: number): Observable<AppointmentDto> {
    return this.api
      .get<{ success: boolean; data: AppointmentDto }>(`Appointments/${id}`)
      .pipe(map((x) => x.data));
  }

  createAppointment(payload: {
    facilityId?: number | null;
    patientId: number;
    doctorId?: number | null;
    departmentId?: number | null;
    scheduledStart: string;
    scheduledEnd?: string | null;
    reason?: string | null;
  }): Observable<AppointmentDto> {
    return this.api
      .post<{ success: boolean; data: AppointmentDto }>('Appointments', payload)
      .pipe(map((x) => x.data));
  }

  updateAppointment(
    id: number,
    payload: {
      facilityId?: number | null;
      doctorId?: number | null;
      departmentId?: number | null;
      scheduledStart: string;
      scheduledEnd?: string | null;
      reason?: string | null;
    },
  ): Observable<AppointmentDto> {
    return this.api
      .put<{ success: boolean; data: AppointmentDto }>(`Appointments/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  changeStatus(id: number, status: string): Observable<void> {
    return this.api
      .patch<{ success: boolean }>(`Appointments/${id}/status?status=${status}`, {})
      .pipe(map(() => void 0));
  }

  deleteAppointment(id: number): Observable<void> {
    // No delete endpoint defined; appointments can be "cancelled" via status
    return this.changeStatus(id, 'Cancelled');
  }
}

