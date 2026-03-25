import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  DoctorDto,
  DoctorMeDto,
  DoctorVisitSettingsDto,
  DoctorWeeklyScheduleDto,
  GetDoctorCalendarSlotsDto,
  PagedApiResponse,
} from './doctors.api';

export interface DoctorsQuery {
  pageNumber: number;
  pageSize: number;
  sortBy?: string | null;
  sortDesc?: boolean;
  search?: string | null;
  departmentId?: number | null;
  isActive?: boolean | null;
}

@Injectable({ providedIn: 'root' })
export class DoctorsService {
  constructor(private readonly api: ApiService) {}

  getDoctors(query: DoctorsQuery): Observable<PagedApiResponse<DoctorDto>> {
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
    if (query.departmentId != null) {
      params['departmentId'] = String(query.departmentId);
    }
    if (query.isActive != null) {
      params['isActive'] = String(query.isActive);
    }
    return this.api.get<PagedApiResponse<DoctorDto>>('Doctors', params);
  }

  getDoctor(staffMemberId: number): Observable<DoctorDto> {
    return this.api
      .get<{ success: boolean; data: DoctorDto }>(`Doctors/${staffMemberId}`)
      .pipe(map((x) => x.data));
  }

  /** Current logged-in doctor (for appointment form: self + department). */
  getMe(): Observable<DoctorMeDto> {
    return this.api
      .get<{ success?: boolean; data?: DoctorMeDto; Data?: DoctorMeDto }>('Doctors/me')
      .pipe(map((r) => r.data ?? r.Data!));
  }

  /** Create doctor with login account (email + password). Use for new doctors so they can sign in. */
  createDoctorWithAccount(payload: {
    email: string;
    password: string;
    fullName: string;
    phone?: string | null;
    departmentId?: number | null;
    specialty?: string | null;
    licenseNumber?: string | null;
  }): Observable<DoctorDto> {
    return this.api
      .post<{ success: boolean; data: DoctorDto }>('Doctors/with-account', {
        email: payload.email,
        password: payload.password,
        fullName: payload.fullName,
        phone: payload.phone ?? null,
        departmentId: payload.departmentId ?? null,
        specialty: payload.specialty ?? null,
        licenseNumber: payload.licenseNumber ?? null,
      })
      .pipe(map((r: { data?: DoctorDto; Data?: DoctorDto }) => r.data ?? r.Data!));
  }

  /** Create doctor without account (legacy). Prefer createDoctorWithAccount for new doctors. */
  createDoctor(payload: {
    fullName: string;
    phone?: string | null;
    email?: string | null;
    departmentId?: number | null;
    specialty?: string | null;
    licenseNumber?: string | null;
  }): Observable<DoctorDto> {
    const staffPayload = {
      fullName: payload.fullName,
      staffType: 1, // Doctor
      phone: payload.phone ?? null,
      email: payload.email ?? null,
      departmentId: payload.departmentId ?? null,
      userId: null as number | null,
    };
    return this.api
      .post<{ success: boolean; data: { id: number } }>('Staff', staffPayload)
      .pipe(
        map((res) => res.data.id),
        switchMap((staffMemberId) =>
          this.api.post<{ success: boolean; data: DoctorDto }>(
            `Doctors/${staffMemberId}/profile`,
            {
              staffMemberId,
              specialty: payload.specialty ?? null,
              licenseNumber: payload.licenseNumber ?? null,
            },
          ).pipe(map((r) => r.data)),
        ),
      );
  }

  /** Update doctor: update staff member then upsert doctor profile */
  updateDoctor(
    staffMemberId: number,
    payload: {
      fullName: string;
      phone?: string | null;
      email?: string | null;
      departmentId?: number | null;
      isActive: boolean;
      specialty?: string | null;
      licenseNumber?: string | null;
    },
  ): Observable<DoctorDto> {
    const staffPayload = {
      id: staffMemberId,
      fullName: payload.fullName,
      staffType: 1,
      phone: payload.phone ?? null,
      email: payload.email ?? null,
      departmentId: payload.departmentId ?? null,
      userId: null as number | null,
      isActive: payload.isActive,
    };
    return this.api
      .put<{ success: boolean; data: DoctorDto }>(`Staff/${staffMemberId}`, staffPayload)
      .pipe(
        switchMap(() =>
          this.api.post<{ success: boolean; data: DoctorDto }>(
            `Doctors/${staffMemberId}/profile`,
            {
              staffMemberId,
              specialty: payload.specialty ?? null,
              licenseNumber: payload.licenseNumber ?? null,
            },
          ),
        ),
        map((r) => r.data),
      );
  }

  setActive(staffMemberId: number, isActive: boolean): Observable<void> {
    return this.api
      .patch<{ success: boolean }>(
        `Staff/${staffMemberId}/active?isActive=${isActive}`,
        {},
      )
      .pipe(map(() => void 0));
  }

  getVisitSettings(staffMemberId: number): Observable<DoctorVisitSettingsDto | null> {
    return this.api
      .get<{ success: boolean; data: DoctorVisitSettingsDto | null }>(
        `Doctors/${staffMemberId}/visit-settings`,
      )
      .pipe(map((r) => (r as any).data ?? (r as any).Data ?? null));
  }

  upsertVisitSettings(payload: {
    id?: number | null;
    staffMemberId: number;
    minVisitDurationMinutes: number;
  }): Observable<DoctorVisitSettingsDto> {
    return this.api
      .post<{ success: boolean; data: DoctorVisitSettingsDto }>(
        `Doctors/${payload.staffMemberId}/visit-settings`,
        {
          id: payload.id ?? null,
          staffMemberId: payload.staffMemberId,
          minVisitDurationMinutes: payload.minVisitDurationMinutes,
        },
      )
      .pipe(map((r) => (r as any).data ?? (r as any).Data));
  }

  getWeeklySchedule(staffMemberId: number): Observable<DoctorWeeklyScheduleDto> {
    return this.api
      .get<{ success: boolean; data: DoctorWeeklyScheduleDto }>(
        `Doctors/${staffMemberId}/weekly-schedule`,
      )
      .pipe(map((r) => (r as any).data ?? (r as any).Data));
  }

  upsertWeeklySchedule(payload: {
    staffMemberId: number;
    days: {
      dayOfWeek: number;
      isWorking: boolean;
      startTime?: string | null;
      endTime?: string | null;
    }[];
  }): Observable<boolean> {
    return this.api
      .post<{ success: boolean; data: boolean }>(
        `Doctors/${payload.staffMemberId}/weekly-schedule`,
        {
          staffMemberId: payload.staffMemberId,
          days: payload.days,
        },
      )
      .pipe(map((r) => (r as any).data ?? (r as any).Data));
  }

  getAvailableSlots(payload: {
    staffMemberId: number;
    from: string; // yyyy-mm-dd
    to: string; // yyyy-mm-dd
  }): Observable<GetDoctorCalendarSlotsDto> {
    return this.api
      .get<{ success: boolean; data: GetDoctorCalendarSlotsDto }>(
        `Doctors/${payload.staffMemberId}/available-slots`,
        {
          from: payload.from,
          to: payload.to,
        } as any,
      )
      .pipe(map((r) => (r as any).data ?? (r as any).Data));
  }
}

