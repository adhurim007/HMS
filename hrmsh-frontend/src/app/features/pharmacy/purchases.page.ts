import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PharmacyService } from './pharmacy.service';
import { ProductListDto, PharmacyPurchaseInvoiceDto } from './pharmacy.api';

@Component({
  selector: 'app-pharmacy-purchases-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './purchases.page.html',
  styleUrl: './purchases.page.scss',
})
export class PharmacyPurchasesPage implements OnInit {
  loadingProducts = false;
  products: ProductListDto[] = [];

  // Header
  supplierName = '';
  supplierReference = '';
  invoiceDate: string | null = null; // YYYY-MM-DD from <input type="date">
  paidAmount = 0;

  // Line draft (single "add line" row)
  draftProductId: number | null = null;
  draftBatchNumber: string | null = null;
  draftExpiryDate: string | null = null;
  draftQuantity = 1;
  draftUnitPurchasePrice = 0;

  lines: {
    productId: number;
    batchNumber?: string | null;
    expiryDate: string;
    quantity: number;
    unitPurchasePrice: number;
  }[] = [];

  submitting = false;
  error = '';
  createdInvoice: PharmacyPurchaseInvoiceDto | null = null;

  constructor(private readonly pharmacy: PharmacyService) {}

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

  addLine(): void {
    this.error = '';

    if (this.draftProductId == null) {
      this.error = 'Select a product.';
      return;
    }
    if (!this.draftExpiryDate) {
      this.error = 'Expiry date is required.';
      return;
    }
    if (this.draftQuantity <= 0) {
      this.error = 'Quantity must be greater than 0.';
      return;
    }
    if (this.draftUnitPurchasePrice < 0) {
      this.error = 'Unit purchase price cannot be negative.';
      return;
    }

    this.lines.push({
      productId: this.draftProductId,
      batchNumber: this.draftBatchNumber ?? null,
      expiryDate: this.draftExpiryDate,
      quantity: Number(this.draftQuantity),
      unitPurchasePrice: Number(this.draftUnitPurchasePrice),
    });

    // Reset draft (keep expiry/price defaults for fast entry)
    this.draftProductId = null;
    this.draftBatchNumber = null;
    this.draftQuantity = 1;
  }

  removeLine(idx: number): void {
    this.lines.splice(idx, 1);
  }

  submit(): void {
    this.error = '';
    this.createdInvoice = null;

    if (this.lines.length === 0) {
      this.error = 'Add at least one purchase line.';
      return;
    }
    if (this.paidAmount < 0) {
      this.error = 'Paid amount cannot be negative.';
      return;
    }

    this.submitting = true;
    this.pharmacy
      .createPurchaseInvoice({
        invoiceDate: this.invoiceDate,
        supplierName: this.supplierName || null,
        supplierReference: this.supplierReference || null,
        paidAmount: Number(this.paidAmount),
        items: this.lines,
      })
      .subscribe({
        next: (dto) => {
          this.createdInvoice = dto;
          this.submitting = false;
          // Reset form
          this.supplierName = '';
          this.supplierReference = '';
          this.invoiceDate = null;
          this.paidAmount = 0;
          this.lines = [];
        },
        error: (err) => {
          this.submitting = false;
          this.error = err?.error?.message || err?.message || 'Failed to create purchase invoice.';
        },
      });
  }

  getProductName(id: number | null): string {
    if (id == null) return '-';
    return this.products.find((p) => p.id === id)?.name ?? String(id);
  }
}

