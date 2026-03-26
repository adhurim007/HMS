import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { PagedApiResponse, StaffMemberDto } from './staff.api';

export interface StaffQuery {
  pageNumber: number;
  pageSize: number;
  search?: string | null;
  staffType?: number | null;
  facilityId?: number | null;
  departmentId?: number | null;
  isActive?: boolean | null;
}

@Injectable({ providedIn: 'root' })
export class StaffService {
  constructor(private readonly api: ApiService) {}

  getStaff(query: StaffQuery): Observable<PagedApiResponse<StaffMemberDto>> {
    const params: Record<string, string> = {
      pageNumber: String(query.pageNumber),
      pageSize: String(query.pageSize),
    };
    if (query.search) params['search'] = query.search;
    if (query.staffType != null) params['staffType'] = String(query.staffType);
    if (query.facilityId != null) params['facilityId'] = String(query.facilityId);
    if (query.departmentId != null) params['departmentId'] = String(query.departmentId);
    if (query.isActive != null) params['isActive'] = String(query.isActive);
    return this.api.get<PagedApiResponse<StaffMemberDto>>('Staff', params);
  }

  createStaff(payload: {
    fullName: string;
    staffType: number;
    phone?: string | null;
    email?: string | null;
    departmentId?: number | null;
    userId?: number | null;
    facilityIds?: number[] | null;
  }): Observable<StaffMemberDto> {
    return this.api
      .post<{ success: boolean; data: StaffMemberDto }>('Staff', payload)
      .pipe(map((r) => r.data));
  }

  setActive(id: number, isActive: boolean): Observable<void> {
    return this.api
      .patch<{ success: boolean }>(`Staff/${id}/active?isActive=${isActive}`, {})
      .pipe(map(() => void 0));
  }
}
