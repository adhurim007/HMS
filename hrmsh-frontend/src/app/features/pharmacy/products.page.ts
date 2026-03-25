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
import { ProductListDto } from './pharmacy.api';

@Component({
  selector: 'app-pharmacy-products-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './products.page.html',
  styleUrl: './products.page.scss',
})
export class PharmacyProductsPage implements OnInit {
  products: ProductListDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  loading = false;

  search = '';
  isActiveFilter: boolean | null = true;

  editingId: number | null = null;
  form = this.fb.group({
    code: ['', [Validators.required]],
    name: ['', [Validators.required]],
    genericName: [''],
    strength: [''],
    unit: [''],
    defaultSalePrice: [0],
    isActive: [true],
  });

  constructor(
    private readonly pharmacy: PharmacyService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.pharmacy
      .getProducts({
        page: this.page,
        pageSize: this.pageSize,
        search: this.search || null,
        isActive: this.isActiveFilter,
        sortBy: 'Name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.products = res.items;
          this.total = res.totalCount;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  onSearchChange(): void {
    this.page = 1;
    this.load();
  }

  applyFilters(): void {
    this.page = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.page + delta;
    if (next < 1) return;
    const max = Math.max(1, Math.ceil(this.total / this.pageSize));
    if (next > max) return;
    this.page = next;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.page = 1;
    this.load();
  }

  startCreate(): void {
    this.editingId = null;
    this.form.reset({
      code: '',
      name: '',
      genericName: '',
      strength: '',
      unit: '',
      defaultSalePrice: 0,
      isActive: true,
    });
  }

  startEdit(p: ProductListDto): void {
    this.editingId = p.id;
    this.form.reset({
      code: p.code,
      name: p.name,
      genericName: p.genericName ?? '',
      strength: '',
      unit: p.unit ?? '',
      defaultSalePrice: p.defaultSalePrice ?? 0,
      isActive: p.isActive,
    });
  }

  cancelEdit(): void {
    this.startCreate();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.value;
    if (this.editingId == null) {
      this.pharmacy
        .createProduct({
          code: v.code!,
          name: v.name!,
          genericName: v.genericName || null,
          strength: v.strength || null,
          unit: v.unit || null,
          defaultSalePrice:
            v.defaultSalePrice != null
              ? Number(v.defaultSalePrice)
              : null,
        })
        .subscribe({
          next: () => {
            this.startCreate();
            this.load();
          },
        });
    } else {
      this.pharmacy
        .updateProduct(this.editingId, {
          name: v.name!,
          genericName: v.genericName || null,
          strength: v.strength || null,
          unit: v.unit || null,
          defaultSalePrice:
            v.defaultSalePrice != null
              ? Number(v.defaultSalePrice)
              : null,
          isActive: !!v.isActive,
        })
        .subscribe({
          next: () => {
            this.startCreate();
            this.load();
          },
        });
    }
  }

  delete(p: ProductListDto): void {
    if (!confirm(`Delete product "${p.name}"?`)) return;
    this.pharmacy.deleteProduct(p.id).subscribe({
      next: () => this.load(),
    });
  }
}

