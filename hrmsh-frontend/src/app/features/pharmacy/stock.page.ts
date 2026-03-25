import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PharmacyService } from './pharmacy.service';
import {
  ProductListDto,
  StockBatchDto,
  StockMovementType,
} from './pharmacy.api';

@Component({
  selector: 'app-pharmacy-stock-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './stock.page.html',
  styleUrl: './stock.page.scss',
})
export class PharmacyStockPage implements OnInit {
  products: ProductListDto[] = [];
  selectedProductId: number | null = null;
  batches: StockBatchDto[] = [];
  loadingBatches = false;

  batchForm = this.fb.group({
    batchNumber: [''],
    expiryDate: [''],
    quantity: [0, [Validators.required, Validators.min(1)]],
  });

  movementForm = this.fb.group({
    type: [1 as StockMovementType, [Validators.required]],
    stockBatchId: [null as number | null],
    quantity: [0, [Validators.required, Validators.min(1)]],
    reason: [''],
    isIncreaseForAdjustment: [true],
  });

  constructor(
    private readonly pharmacy: PharmacyService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.pharmacy
      .getProducts({
        page: 1,
        pageSize: 500,
        search: null,
        isActive: true,
        sortBy: 'Name',
        sortDescending: false,
      })
      .subscribe((res) => {
        this.products = res.items;
        if (this.products.length > 0 && this.selectedProductId == null) {
          this.selectedProductId = this.products[0].id;
          this.onProductChange();
        }
      });
  }

  onProductChange(): void {
    if (this.selectedProductId == null) {
      this.batches = [];
      return;
    }
    this.loadBatches();
  }

  loadBatches(): void {
    if (this.selectedProductId == null) return;
    this.loadingBatches = true;
    this.pharmacy.getStockBatches(this.selectedProductId).subscribe({
      next: (b) => {
        this.loadingBatches = false;
        this.batches = b;
      },
      error: () => {
        this.loadingBatches = false;
        this.batches = [];
      },
    });
  }

  submitBatch(): void {
    if (this.batchForm.invalid || this.selectedProductId == null) {
      this.batchForm.markAllAsTouched();
      return;
    }
    const v = this.batchForm.value;
    this.pharmacy
      .createStockBatch({
        productId: this.selectedProductId,
        batchNumber: v.batchNumber || null,
        expiryDate: v.expiryDate || null,
        quantity: Number(v.quantity),
      })
      .subscribe({
        next: () => {
          this.batchForm.reset({
            batchNumber: '',
            expiryDate: '',
            quantity: 0,
          });
          this.loadBatches();
        },
      });
  }

  submitMovement(): void {
    if (this.movementForm.invalid || this.selectedProductId == null) {
      this.movementForm.markAllAsTouched();
      return;
    }
    const v = this.movementForm.value;
    this.pharmacy
      .recordStockMovement({
        productId: this.selectedProductId,
        stockBatchId: v.stockBatchId ?? null,
        type: v.type as StockMovementType,
        quantity: Number(v.quantity),
        reason: v.reason || null,
        isIncreaseForAdjustment: v.isIncreaseForAdjustment ?? true,
      })
      .subscribe({
        next: () => {
          this.movementForm.reset({
            type: 1 as StockMovementType,
            stockBatchId: null,
            quantity: 0,
            reason: '',
            isIncreaseForAdjustment: true,
          });
          this.loadBatches();
        },
      });
  }

  getProductName(id: number | null): string {
    if (id == null) return '-';
    const p = this.products.find((x) => x.id === id);
    return p ? p.name : String(id);
  }
}

