import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { FacilityDto, PagedApiResponse } from './facilities.api';

export interface FacilitiesQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
}

@Injectable({ providedIn: 'root' })
export class FacilitiesService {
  constructor(private readonly api: ApiService) {}

  getFacilities(query: FacilitiesQuery): Observable<PagedApiResponse<FacilityDto>> {
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
    return this.api.get<PagedApiResponse<FacilityDto>>('Facilities', params);
  }

  getFacility(id: number): Observable<FacilityDto> {
    return this.api
      .get<{ success: boolean; data: FacilityDto }>(`Facilities/${id}`)
      .pipe(map((x) => x.data));
  }

  createFacility(payload: {
    name: string;
    code?: string | null;
    address?: string | null;
    parentId?: number | null;
  }): Observable<FacilityDto> {
    return this.api
      .post<{ success: boolean; data: FacilityDto }>('Facilities', payload)
      .pipe(map((x) => x.data));
  }

  updateFacility(
    id: number,
    payload: {
      name: string;
      code?: string | null;
      address?: string | null;
      parentId?: number | null;
    },
  ): Observable<FacilityDto> {
    return this.api
      .put<{ success: boolean; data: FacilityDto }>(`Facilities/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deleteFacility(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Facilities/${id}`)
      .pipe(map(() => void 0));
  }
}

