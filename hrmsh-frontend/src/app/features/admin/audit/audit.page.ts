import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuditService } from './audit.service';
import { AuditLogItem } from './audit.api';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';

@Component({
  selector: 'app-audit-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './audit.page.html',
  styleUrl: './audit.page.scss',
})
export class AuditPage {
  logs: AuditLogItem[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 50;
  loading = false;

  readonly filterForm = this.fb.group({
    entityType: [''],
    patientId: [''],
    userName: [''],
    fromDate: [''],
    toDate: [''],
  });

  readonly knownEntityTypes = ['Patient', 'Visit', 'Invoice', 'StockMovement'];

  readonly math = Math;

  constructor(
    private readonly fb: FormBuilder,
    private readonly audit: AuditService,
  ) {
    this.load();
  }

  load(page: number = this.pageNumber): void {
    this.pageNumber = page;
    const value = this.filterForm.value;
    const from =
      value.fromDate && value.fromDate !== ''
        ? new Date(value.fromDate)
        : null;
    const to =
      value.toDate && value.toDate !== ''
        ? new Date(value.toDate)
        : null;

    const toIso = (d: Date | null) =>
      d ? new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate())).toISOString() : null;

    const patientId =
      value.patientId && value.patientId.trim() !== ''
        ? Number(value.patientId)
        : null;

    this.loading = true;
    this.audit
      .getAuditLogs({
        entityType: value.entityType || null,
        patientId: patientId && !Number.isNaN(patientId) ? patientId : null,
        userName: value.userName || null,
        fromUtc: toIso(from),
        toUtc: toIso(to),
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.logs = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => {
          this.loading = false;
          this.logs = [];
          this.totalCount = 0;
        },
      });
  }

  resetFilters(): void {
    this.filterForm.reset({
      entityType: '',
      patientId: '',
      userName: '',
      fromDate: '',
      toDate: '',
    });
    this.pageNumber = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.pageNumber + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.load(next);
  }
}

