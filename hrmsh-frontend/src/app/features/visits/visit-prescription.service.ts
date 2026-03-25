import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  PagedApiResponse,
  PrescriptionDto,
  PrescriptionListItemDto,
} from './visit-prescription.api';

@Injectable({ providedIn: 'root' })
export class VisitPrescriptionService {
  constructor(private readonly api: ApiService) {}

  getList(params: {
    page: number;
    pageSize: number;
    patientId?: number | null;
    doctorId?: number | null;
    status?: number | null;
    from?: string | null;
    to?: string | null;
    search?: string | null;
  }): Observable<PagedApiResponse<PrescriptionListItemDto>> {
    const q: Record<string, string> = {
      page: String(params.page),
      pageSize: String(params.pageSize),
    };
    if (params.patientId != null) q['patientId'] = String(params.patientId);
    if (params.doctorId != null) q['doctorId'] = String(params.doctorId);
    if (params.status != null) q['status'] = String(params.status);
    if (params.from) q['from'] = params.from;
    if (params.to) q['to'] = params.to;
    if (params.search) q['search'] = params.search;

    return this.api.get<PagedApiResponse<PrescriptionListItemDto>>(
      'Prescriptions',
      q,
    );
  }

  getByVisit(visitId: number): Observable<PrescriptionDto | null> {
    return this.api
      .get<{ success: boolean; data: PrescriptionDto | null }>(
        `Prescriptions/by-visit/${visitId}`,
      )
      .pipe(map((x) => x.data ?? null));
  }

  upsert(payload: {
    visitId: number;
    notes?: string | null;
    items: {
      productId: number;
      dosage?: string | null;
      frequency?: string | null;
      duration?: string | null;
      quantity: number;
      instructions?: string | null;
    }[];
  }): Observable<PrescriptionDto> {
    return this.api
      .post<{ success: boolean; data: PrescriptionDto }>(
        'Prescriptions',
        payload,
      )
      .pipe(map((x) => x.data));
  }

  dispense(
    prescriptionId: number,
    payload: {
      items: { prescriptionItemId: number; quantity: number }[];
    },
  ): Observable<void> {
    return this.api
      .post<{ success: boolean; message?: string }>(
        `Prescriptions/${prescriptionId}/dispense`,
        payload,
      )
      .pipe(map(() => void 0));
  }
}

