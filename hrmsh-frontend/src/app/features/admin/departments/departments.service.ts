import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { DepartmentDto, PagedApiResponse } from './departments.api';

export interface DepartmentsQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
  facilityId?: number | null;
}

@Injectable({ providedIn: 'root' })
export class DepartmentsService {
  constructor(private readonly api: ApiService) {}

  getDepartments(query: DepartmentsQuery): Observable<PagedApiResponse<DepartmentDto>> {
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
    if (query.facilityId != null) {
      params['facilityId'] = String(query.facilityId);
    }
    return this.api.get<PagedApiResponse<DepartmentDto>>('Departments', params);
  }

  getDepartment(id: number): Observable<DepartmentDto> {
    return this.api
      .get<{ success: boolean; data: DepartmentDto }>(`Departments/${id}`)
      .pipe(map((x) => x.data));
  }

  createDepartment(payload: {
    name: string;
    code?: string | null;
    facilityId: number;
  }): Observable<DepartmentDto> {
    return this.api
      .post<{ success: boolean; data: DepartmentDto }>('Departments', payload)
      .pipe(map((x) => x.data));
  }

  updateDepartment(
    id: number,
    payload: {
      name: string;
      code?: string | null;
      facilityId: number;
    },
  ): Observable<DepartmentDto> {
    return this.api
      .put<{ success: boolean; data: DepartmentDto }>(`Departments/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deleteDepartment(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Departments/${id}`)
      .pipe(map(() => void 0));
  }
}

