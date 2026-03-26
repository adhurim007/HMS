import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { HospitalDto, PagedApiResponse } from './hospitals.api';

export interface HospitalsQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
}

@Injectable({ providedIn: 'root' })
export class HospitalsService {
  constructor(private readonly api: ApiService) {}

  getHospitals(query: HospitalsQuery): Observable<PagedApiResponse<HospitalDto>> {
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
    return this.api.get<PagedApiResponse<HospitalDto>>('Hospitals', params);
  }

  createHospital(payload: {
    name: string;
    code?: string | null;
    address?: string | null;
  }): Observable<HospitalDto> {
    return this.api
      .post<{ success: boolean; data: HospitalDto }>('Hospitals', payload)
      .pipe(map((x) => x.data));
  }

  updateHospital(
    id: number,
    payload: {
      name: string;
      code?: string | null;
      address?: string | null;
    },
  ): Observable<HospitalDto> {
    return this.api
      .put<{ success: boolean; data: HospitalDto }>(`Hospitals/${id}`, { id, ...payload })
      .pipe(map((x) => x.data));
  }

  deleteHospital(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Hospitals/${id}`)
      .pipe(map(() => void 0));
  }
}
