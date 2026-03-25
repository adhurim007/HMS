import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  ApiResponse,
  DailyPaymentRow,
  PagedApiResponse,
  StockExpiryAlertRow,
} from './dashboard.api';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private readonly api: ApiService) {}

  getPatientsCount(): Observable<number> {
    return this.api
      .get<PagedApiResponse<unknown>>('Patients', {
        pageNumber: '1',
        pageSize: '1',
      })
      .pipe(
        map(
          (r) =>
            r.totalCount ??
            r.TotalCount ??
            0,
        ),
        catchError(() => of(0)),
      );
  }

  getTodayAppointmentsCount(): Observable<number> {
    const today = new Date();
    const from = today.toISOString();
    const to = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate() + 1,
    ).toISOString();

    return this.api
      .get<PagedApiResponse<unknown>>('Appointments', {
        pageNumber: '1',
        pageSize: '1',
        from,
        to,
      })
      .pipe(
        map(
          (r) =>
            r.totalCount ??
            r.TotalCount ??
            0,
        ),
        catchError(() => of(0)),
      );
  }

  getPendingInvoicesCount(): Observable<number> {
    return this.api
      .get<PagedApiResponse<unknown>>('Invoices', {
        page: '1',
        pageSize: '1',
        status: 'Unpaid',
      })
      .pipe(
        map(
          (r) =>
            r.totalCount ??
            r.TotalCount ??
            0,
        ),
        catchError(() => of(0)),
      );
  }

  getDailyPaymentsLastNDays(days: number): Observable<DailyPaymentRow[]> {
    const to = new Date();
    const from = new Date(
      to.getFullYear(),
      to.getMonth(),
      to.getDate() - days + 1,
    );
    const fromStr = from.toISOString();
    const toStr = to.toISOString();

    return this.api
      .get<
        ApiResponse<DailyPaymentRow[]>
      >('Reports/daily-payments', {
        from: fromStr,
        to: toStr,
      })
      .pipe(
        map((r) => r.data ?? r.Data ?? []),
        catchError(() => of([])),
      );
  }

  getStockExpiryAlerts(
    daysThreshold: number,
  ): Observable<StockExpiryAlertRow[]> {
    return this.api
      .get<
        ApiResponse<StockExpiryAlertRow[]>
      >('Reports/stock-expiry-alerts', {
        daysThreshold: String(daysThreshold),
      })
      .pipe(
        map((r) => r.data ?? r.Data ?? []),
        catchError(() => of([])),
      );
  }
}

