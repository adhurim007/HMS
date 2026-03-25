import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  PrescriptionDto,
  PrescriptionListItemDto,
} from '../visits/visit-prescription.api';
import { VisitPrescriptionService } from '../visits/visit-prescription.service';

@Component({
  selector: 'app-prescriptions-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './prescriptions.page.html',
  styleUrl: './prescriptions.page.scss',
})
export class PrescriptionsPage implements OnInit {
  prescriptions: PrescriptionListItemDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;

  search = '';
  statusFilter: number | null = null;
  fromFilter: string | null = null;
  toFilter: string | null = null;

  loading = false;

  // Selected for print
  selected: PrescriptionListItemDto | null = null;
  selectedDetails: PrescriptionDto | null = null;
  printLoading = false;
  dispensing = false;
  dispenseError = '';

  readonly statuses = [
    { value: null, label: 'All statuses' },
    { value: 1, label: 'Draft' },
    { value: 2, label: 'Issued' },
    { value: 3, label: 'Dispensed' },
    { value: 4, label: 'Cancelled' },
  ];

  constructor(private readonly service: VisitPrescriptionService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service
      .getList({
        page: this.page,
        pageSize: this.pageSize,
        status: this.statusFilter,
        from: this.fromFilter,
        to: this.toFilter,
        search: this.search || null,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.prescriptions = res.items ?? [];
          this.totalCount = res.totalCount ?? 0;
        },
        error: () => {
          this.loading = false;
          this.prescriptions = [];
          this.totalCount = 0;
        },
      });
  }

  applyFilters(): void {
    this.page = 1;
    this.load();
  }

  clearFilters(): void {
    this.search = '';
    this.statusFilter = null;
    this.fromFilter = null;
    this.toFilter = null;
    this.page = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.page + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.page = next;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.page = 1;
    this.load();
  }

  openForPrint(p: PrescriptionListItemDto): void {
    this.selected = p;
    this.selectedDetails = null;
    this.printLoading = true;
    this.dispenseError = '';
    this.service.getByVisit(p.visitId).subscribe({
      next: (dto) => {
        this.printLoading = false;
        this.selectedDetails = dto;
      },
      error: () => {
        this.printLoading = false;
        this.selectedDetails = null;
      },
    });
  }

  closePrint(): void {
    this.selected = null;
    this.selectedDetails = null;
    this.dispensing = false;
    this.dispenseError = '';
  }

  print(): void {
    window.print();
  }

  dispense(): void {
    if (!this.selected || !this.selectedDetails) return;
    this.dispensing = true;
    this.dispenseError = '';

    const items = this.selectedDetails.items.map((i) => ({
      prescriptionItemId: i.id,
      quantity: i.quantity,
    }));

    this.service
      .dispense(this.selected.id, {
        items,
      })
      .subscribe({
        next: () => {
          this.dispensing = false;
          this.closePrint();
          this.load();
        },
        error: (err) => {
          this.dispensing = false;
          const status = err?.status;
          const backendMessage = err?.error?.message as string | undefined;

          if (status === 403) {
            this.dispenseError =
              'You are not allowed to dispense prescriptions. Please contact the pharmacy.';
          } else if (status === 409 && backendMessage) {
            // Backend 409 is used for business conflicts like insufficient stock.
            this.dispenseError = backendMessage;
          } else {
            this.dispenseError =
              backendMessage ||
              err?.message ||
              'Failed to dispense prescription.';
          }

          if (this.dispenseError) {
            window.alert(this.dispenseError);
          }
        },
      });
  }
}

