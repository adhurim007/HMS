import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  InvoiceDto,
  InvoiceLineInput,
  InvoiceListDto,
  InstallmentPlanDto,
  PatientPaymentHistoryDto,
  PagedApiResponse,
  PaymentDto,
  ServiceItemDto,
  ServiceItemListDto,
  UnbilledLaboratoryItemDto,
  UnbilledPrescriptionItemDto,
  UnbilledVisitServiceDto,
} from './billing.api';

export interface InvoicesQuery {
  patientId?: number | null;
  status?: string | null;
  from?: string | null;
  to?: string | null;
  page: number;
  pageSize: number;
  sortBy?: string | null;
  sortDescending?: boolean;
}

@Injectable({ providedIn: 'root' })
export class BillingService {
  constructor(private readonly api: ApiService) {}

  getInvoices(query: InvoicesQuery): Observable<PagedApiResponse<InvoiceListDto>> {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
    };
    if (query.patientId != null) params['patientId'] = String(query.patientId);
    if (query.status) params['status'] = query.status;
    if (query.from) params['from'] = query.from;
    if (query.to) params['to'] = query.to;
    if (query.sortBy) params['sortBy'] = query.sortBy;
    if (query.sortDescending !== undefined)
      params['sortDescending'] = String(query.sortDescending);

    return this.api.get<PagedApiResponse<InvoiceListDto>>('Invoices', params);
  }

  getInvoice(id: number): Observable<InvoiceDto> {
    return this.api
      .get<{ success: boolean; data: InvoiceDto }>(`Invoices/${id}`)
      .pipe(map((x) => x.data));
  }

  getUnbilledVisitServices(params: {
    patientId: number;
    from?: string | null;
    to?: string | null;
    doctorId?: number | null;
  }): Observable<UnbilledVisitServiceDto[]> {
    const q: Record<string, string> = { patientId: String(params.patientId) };
    if (params.from) q['from'] = params.from;
    if (params.to) q['to'] = params.to;
    if (params.doctorId != null) q['doctorId'] = String(params.doctorId);
    return this.api
      .get<{ success?: boolean; data?: UnbilledVisitServiceDto[]; Data?: UnbilledVisitServiceDto[] }>('Invoices/UnbilledVisitServices', q)
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  getUnbilledLaboratoryItems(params: {
    patientId: number;
    from?: string | null;
    to?: string | null;
    doctorId?: number | null;
  }): Observable<UnbilledLaboratoryItemDto[]> {
    const q: Record<string, string> = { patientId: String(params.patientId) };
    if (params.from) q['from'] = params.from;
    if (params.to) q['to'] = params.to;
    if (params.doctorId != null) q['doctorId'] = String(params.doctorId);
    return this.api
      .get<{ success?: boolean; data?: UnbilledLaboratoryItemDto[]; Data?: UnbilledLaboratoryItemDto[] }>('Invoices/UnbilledLaboratoryItems', q)
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  getUnbilledPrescriptionItems(params: {
    patientId: number;
    from?: string | null;
    to?: string | null;
    doctorId?: number | null;
  }): Observable<UnbilledPrescriptionItemDto[]> {
    const q: Record<string, string> = { patientId: String(params.patientId) };
    if (params.from) q['from'] = params.from;
    if (params.to) q['to'] = params.to;
    if (params.doctorId != null) q['doctorId'] = String(params.doctorId);
    return this.api
      .get<{
        success?: boolean;
        data?: UnbilledPrescriptionItemDto[];
        Data?: UnbilledPrescriptionItemDto[];
      }>('Invoices/UnbilledPrescriptionItems', q)
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  createInvoice(payload: {
    patientId: number;
    invoiceDate?: string | null;
    items: InvoiceLineInput[];
  }): Observable<InvoiceDto> {
    return this.api
      .post<{ success?: boolean; data?: InvoiceDto; Success?: boolean; Data?: InvoiceDto }>('Invoices', payload)
      .pipe(map((x) => (x.data ?? (x as { Data?: InvoiceDto }).Data)!));
  }

  addPayment(payload: {
    invoiceId: number;
    amount: number;
    method?: string | null;
    reference?: string | null;
    paymentDate?: string | null;
  }): Observable<PaymentDto> {
    return this.api
      .post<{ success: boolean; data: PaymentDto }>('Payments', payload)
      .pipe(map((x) => x.data));
  }

  createInstallmentPlan(payload: {
    invoiceId: number;
    startDate?: string | null;
    items: { dueDate: string; amount: number }[];
  }): Observable<InstallmentPlanDto> {
    return this.api
      .post<{ success?: boolean; data?: InstallmentPlanDto; Data?: InstallmentPlanDto }>(
        'Installments/plans',
        payload,
      )
      .pipe(map((r) => r.data ?? r.Data!));
  }

  getInstallmentPlans(params: {
    patientId?: number | null;
    invoiceId?: number | null;
  }): Observable<InstallmentPlanDto[]> {
    const q: Record<string, string> = {};
    if (params.patientId != null) q['patientId'] = String(params.patientId);
    if (params.invoiceId != null) q['invoiceId'] = String(params.invoiceId);
    return this.api
      .get<{ success?: boolean; data?: InstallmentPlanDto[]; Data?: InstallmentPlanDto[] }>(
        'Installments/plans',
        q,
      )
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  addInstallmentPayment(payload: {
    installmentItemId: number;
    amount: number;
    method?: string | null;
    reference?: string | null;
    paymentDate?: string | null;
  }): Observable<PaymentDto> {
    return this.api
      .post<{ success?: boolean; data?: PaymentDto; Data?: PaymentDto }>(
        'Installments/payments',
        payload,
      )
      .pipe(map((r) => r.data ?? r.Data!));
  }

  getPatientPaymentHistory(patientId: number): Observable<PatientPaymentHistoryDto> {
    return this.api
      .get<{ success?: boolean; data?: PatientPaymentHistoryDto; Data?: PatientPaymentHistoryDto }>(
        `Installments/patient-history/${patientId}`,
      )
      .pipe(map((r) => r.data ?? r.Data!));
  }

  getServiceItems(query: {
    page: number;
    pageSize: number;
    search?: string | null;
    isActive?: boolean | null;
    sortBy?: string | null;
    sortDescending?: boolean;
  }): Observable<PagedApiResponse<ServiceItemListDto>> {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
    };
    if (query.search) params['search'] = query.search;
    if (query.isActive != null) params['isActive'] = String(query.isActive);
    if (query.sortBy) params['sortBy'] = query.sortBy;
    if (query.sortDescending !== undefined)
      params['sortDescending'] = String(query.sortDescending);

    return this.api.get<PagedApiResponse<ServiceItemListDto>>('Services', params);
  }

  getServicesForMe(): Observable<ServiceItemListDto[]> {
    return this.api
      .get<{ success?: boolean; data?: ServiceItemListDto[]; Data?: ServiceItemListDto[] }>(
        'Services/for-me',
      )
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  getServiceItem(id: number): Observable<ServiceItemDto> {
    return this.api
      .get<{ success: boolean; data: ServiceItemDto }>(`Services/${id}`)
      .pipe(map((x) => x.data));
  }

  createServiceItem(payload: {
    code: string;
    name: string;
    price: number;
  }): Observable<ServiceItemDto> {
    return this.api
      .post<{ success: boolean; data: ServiceItemDto }>('Services', payload)
      .pipe(map((x) => x.data));
  }

  updateServiceItem(
    id: number,
    payload: { name: string; price: number; isActive: boolean },
  ): Observable<ServiceItemDto> {
    return this.api
      .put<{ success: boolean; data: ServiceItemDto }>(`Services/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deleteServiceItem(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Services/${id}`)
      .pipe(map(() => void 0));
  }
}
