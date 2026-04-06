import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import { VisitDto, VisitListDto, PagedApiResponse } from './visits.api';

export interface VisitsQuery {
  patientId?: number | null;
  doctorId?: number | null;
  from?: string | null;
  to?: string | null;
  page: number;
  pageSize: number;
  sortBy?: string | null;
  sortDescending?: boolean;
}

@Injectable({ providedIn: 'root' })
export class VisitsService {
  constructor(private readonly api: ApiService) {}

  getVisits(query: VisitsQuery): Observable<PagedApiResponse<VisitListDto>> {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
    };
    if (query.patientId != null) params['patientId'] = String(query.patientId);
    if (query.doctorId != null) params['doctorId'] = String(query.doctorId);
    if (query.from) params['from'] = query.from;
    if (query.to) params['to'] = query.to;
    if (query.sortBy) params['sortBy'] = query.sortBy;
    if (query.sortDescending !== undefined)
      params['sortDescending'] = String(query.sortDescending);

    return this.api.get<PagedApiResponse<VisitListDto>>('Visits', params);
  }

  getVisit(id: number): Observable<VisitDto> {
    return this.api
      .get<{ success: boolean; data: VisitDto }>(`Visits/${id}`)
      .pipe(map((x) => x.data));
  }

  createVisit(payload: {
    patientId: number;
    doctorId?: number | null;
    visitDate?: string | null;
    chiefComplaint?: string | null;
    notes?: string | null;
    diagnosis?: string | null;
    clinicalDataJson?: string | null;
    services?: {
      serviceItemId: number;
      quantity: number;
      unitPrice: number | null;
      notes: string | null;
    }[];
  }): Observable<VisitDto> {
    return this.api
      .post<{ success: boolean; data: VisitDto }>('Visits', payload)
      .pipe(map((x) => x.data));
  }

  updateVisit(
    id: number,
    payload: {
      doctorId?: number | null;
      visitDate?: string | null;
      chiefComplaint?: string | null;
      notes?: string | null;
      diagnosis?: string | null;
      clinicalDataJson?: string | null;
      services?: {
        serviceItemId: number;
        quantity: number;
        unitPrice: number | null;
        notes: string | null;
      }[];
    },
  ): Observable<VisitDto> {
    return this.api
      .put<{ success: boolean; data: VisitDto }>(`Visits/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deleteVisit(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Visits/${id}`)
      .pipe(map(() => void 0));
  }
}
