import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from './dashboard.service';
import { DailyPaymentRow, StockExpiryAlertRow } from './dashboard.api';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.scss',
})
export class DashboardPage implements OnInit {
  patientsCount = 0;
  todayAppointmentsCount = 0;
  pendingInvoicesCount = 0;
  todayPaymentsTotal = 0;

  dailyPayments: DailyPaymentRow[] = [];
  stockAlerts: StockExpiryAlertRow[] = [];

  readonly today = new Date();

  constructor(private readonly dashboard: DashboardService) {}

  ngOnInit(): void {
    this.dashboard.getPatientsCount().subscribe((c) => (this.patientsCount = c));
    this.dashboard
      .getTodayAppointmentsCount()
      .subscribe((c) => (this.todayAppointmentsCount = c));
    this.dashboard
      .getPendingInvoicesCount()
      .subscribe((c) => (this.pendingInvoicesCount = c));
    this.dashboard
      .getDailyPaymentsLastNDays(7)
      .subscribe((rows) => {
        this.dailyPayments = rows;
        this.todayPaymentsTotal =
          rows.find(
            (r) =>
              new Date(r.date).toDateString() ===
              this.today.toDateString(),
          )?.totalAmount ?? 0;
      });
    this.dashboard
      .getStockExpiryAlerts(30)
      .subscribe((rows) => (this.stockAlerts = rows));
  }
}

