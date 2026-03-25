import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  DiagnosticTestDto,
  DiagnosticType,
  LaboratoryOrderDto,
  LaboratoryOrderStatus,
  LaboratoryCollectorDto,
  PatientLabHistoryRowDto,
  PagedApiResponse,
} from './diagnostics.api';

@Injectable({ providedIn: 'root' })
export class DiagnosticsService {
  constructor(private readonly api: ApiService) {}

  getTests(params: { type?: DiagnosticType | null; isActive?: boolean | null }): Observable<DiagnosticTestDto[]> {
    const q: Record<string, string> = {};
    if (params.type != null) q['type'] = String(params.type);
    if (params.isActive != null) q['isActive'] = String(params.isActive);
    return this.api
      .get<{ success?: boolean; data?: DiagnosticTestDto[]; Data?: DiagnosticTestDto[] }>('Diagnostics/tests', q)
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  saveTest(payload: {
    id?: number | null;
    code: string;
    name: string;
    type: number;
    price: number;
    isActive: boolean;
  }): Observable<DiagnosticTestDto> {
    return this.api
      .post<{ success?: boolean; data?: DiagnosticTestDto; Data?: DiagnosticTestDto }>('Diagnostics/tests', payload)
      .pipe(map((r) => r.data ?? r.Data!));
  }

  getLaboratoryOrders(query: {
    patientId?: number | null;
    visitId?: number | null;
    doctorId?: number | null;
    status?: LaboratoryOrderStatus | null;
    from?: string | null;
    to?: string | null;
    page: number;
    pageSize: number;
  }): Observable<PagedApiResponse<LaboratoryOrderDto>> {
    const q: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
    };
    if (query.patientId != null) q['patientId'] = String(query.patientId);
    if (query.visitId != null) q['visitId'] = String(query.visitId);
    if (query.doctorId != null) q['doctorId'] = String(query.doctorId);
    if (query.status != null) q['status'] = String(query.status);
    if (query.from) q['from'] = query.from;
    if (query.to) q['to'] = query.to;
    return this.api.get<PagedApiResponse<LaboratoryOrderDto>>('Laboratory/orders', q);
  }

  createLaboratoryOrder(payload: {
    patientId: number;
    visitId?: number | null;
    referringDoctorId?: number | null;
    clinicalIndication?: string | null;
    priority?: number;
    items: { diagnosticTestId: number; notes?: string | null }[];
  }): Observable<LaboratoryOrderDto> {
    return this.api
      .post<{ success?: boolean; data?: LaboratoryOrderDto; Data?: LaboratoryOrderDto }>('Laboratory/orders', payload)
      .pipe(map((r) => r.data ?? r.Data!));
  }

  markLaboratoryOrderPaid(orderId: number, payload: { paymentMethod?: string | null }): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/mark-paid`, payload)
      .pipe(map(() => void 0));
  }

  createLaboratorySample(orderId: number, payload: {
    sampleType: string;
    collectedAt?: string | null;
    collectedById: number;
    sampleBarcode: string;
  }): Observable<void> {
    return this.api
      .post<{ success?: boolean }>(`Laboratory/orders/${orderId}/samples`, payload)
      .pipe(map(() => void 0));
  }

  startLaboratoryProcessing(orderId: number): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/start-processing`, {})
      .pipe(map(() => void 0));
  }

  addLaboratoryResult(orderId: number, payload: {
    laboratoryOrderItemId: number;
    laboratorySampleId: number;
    value: string;
    unit?: string | null;
    referenceRange?: string | null;
    flag?: number;
    enteredById: number;
  }): Observable<void> {
    return this.api
      .post<{ success?: boolean }>(`Laboratory/orders/${orderId}/results`, payload)
      .pipe(map(() => void 0));
  }

  validateLaboratoryResults(orderId: number, payload: { validatedById: number }): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/validate`, payload)
      .pipe(map(() => void 0));
  }

  deliverLaboratoryOrder(orderId: number): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/deliver`, {})
      .pipe(map(() => void 0));
  }

  cancelLaboratoryOrder(orderId: number): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/cancel`, {})
      .pipe(map(() => void 0));
  }

  retestLaboratoryOrder(orderId: number): Observable<void> {
    return this.api
      .put<{ success?: boolean }>(`Laboratory/orders/${orderId}/retest`, {})
      .pipe(map(() => void 0));
  }

  getPatientLaboratoryHistory(patientId: number): Observable<PatientLabHistoryRowDto[]> {
    return this.api
      .get<{ success?: boolean; data?: PatientLabHistoryRowDto[]; Data?: PatientLabHistoryRowDto[] }>(`Laboratory/patient-history/${patientId}`)
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  getLaboratoryCollectors(): Observable<LaboratoryCollectorDto[]> {
    return this.api
      .get<{ success?: boolean; data?: LaboratoryCollectorDto[]; Data?: LaboratoryCollectorDto[] }>('Laboratory/collectors')
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }
}
