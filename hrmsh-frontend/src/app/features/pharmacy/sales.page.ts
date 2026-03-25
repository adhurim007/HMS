import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PharmacyService } from './pharmacy.service';
import { ProductListDto } from './pharmacy.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { InvoiceDto } from '../billing/billing.api';

@Component({
  selector: 'app-pharmacy-sales-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sales.page.html',
  styleUrl: './sales.page.scss',
})
export class PharmacySalesPage implements OnInit {
  products: ProductListDto[] = [];

  loadingProducts = false;
  submitting = false;
  error = '';

  // Patient selection (by MRN)
  patientMrn = '';
  patientLoading = false;
  patient: PatientDto | null = null;
  patientId: number | null = null;

  // Sales line draft
  draftProductId: number | null = null;
  draftQuantity = 1;
  lines: { productId: number; quantity: number }[] = [];

  createdInvoice: InvoiceDto | null = null;

  constructor(
    private readonly pharmacy: PharmacyService,
    private readonly patients: PatientsService,
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loadingProducts = true;
    this.pharmacy
      .getProducts({
        page: 1,
        pageSize: 500,
        search: null,
        isActive: true,
        sortBy: 'Name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.products = res.items ?? [];
          this.loadingProducts = false;
        },
        error: () => {
          this.products = [];
          this.loadingProducts = false;
        },
      });
  }

  loadPatient(): void {
    this.error = '';
    this.patient = null;
    this.patientId = null;

    const mrn = this.patientMrn?.trim();
    if (!mrn) {
      this.error = 'Enter patient MRN.';
      return;
    }

    this.patientLoading = true;
    this.patients.getPatientByMrn(mrn).subscribe({
      next: (p) => {
        this.patient = p;
        this.patientId = p.id;
        this.patientLoading = false;
      },
      error: (err) => {
        this.patient = null;
        this.patientId = null;
        this.patientLoading = false;
        this.error =
          err?.error?.message || err?.message || 'Failed to load patient.';
      },
    });
  }

  addLine(): void {
    this.error = '';
    if (this.patientId == null) {
      this.error = 'Load a patient first.';
      return;
    }
    if (this.draftProductId == null) {
      this.error = 'Select a product.';
      return;
    }
    if (this.draftQuantity <= 0) {
      this.error = 'Quantity must be greater than 0.';
      return;
    }

    this.lines.push({
      productId: this.draftProductId,
      quantity: Number(this.draftQuantity),
    });

    this.draftProductId = null;
    this.draftQuantity = 1;
  }

  removeLine(idx: number): void {
    this.lines.splice(idx, 1);
  }

  submit(): void {
    this.error = '';
    this.createdInvoice = null;

    if (this.patientId == null) {
      this.error = 'Load a patient first.';
      return;
    }
    if (this.lines.length === 0) {
      this.error = 'Add at least one product line.';
      return;
    }

    this.submitting = true;
    this.pharmacy
      .createPharmacySale({
        patientId: this.patientId,
        items: this.lines,
      })
      .subscribe({
        next: (inv) => {
          this.createdInvoice = inv;
          this.submitting = false;
          // Reset cart for next sale, keep the patient
          this.lines = [];
          this.draftProductId = null;
          this.draftQuantity = 1;
        },
        error: (err) => {
          this.submitting = false;
          this.error = err?.error?.message || err?.message || 'Failed to sell.';
        },
      });
  }

  getProductName(id: number): string {
    return this.products.find((p) => p.id === id)?.name ?? String(id);
  }
}

