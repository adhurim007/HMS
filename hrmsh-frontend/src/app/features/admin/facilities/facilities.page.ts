import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FacilitiesService } from './facilities.service';
import { FacilityDto } from './facilities.api';

@Component({
  selector: 'app-facilities-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './facilities.page.html',
  styleUrl: './facilities.page.scss',
})
export class FacilitiesPage implements OnInit {
  facilities: FacilityDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = null;
  sortDesc = false;
  loading = false;

  editingId: number | null = null;
  showForm = false;

  readonly form = this.fb.group({
    name: ['', Validators.required],
    code: [''],
    address: [''],
  });

  readonly math = Math;

  constructor(
    private readonly facilitiesService: FacilitiesService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.facilitiesService
      .getFacilities({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.facilities = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => {
          this.loading = false;
        },
      });
  }

  onSearchChange(value: string): void {
    this.search = value;
    this.pageNumber = 1;
    this.load();
  }

  changePage(delta: number): void {
    const next = this.pageNumber + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.pageNumber = next;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDesc = !this.sortDesc;
    } else {
      this.sortBy = column;
      this.sortDesc = false;
    }
    this.load();
  }

  openCreate(): void {
    this.editingId = null;
    this.form.reset({
      name: '',
      code: '',
      address: '',
    });
    this.showForm = true;
  }

  openEdit(facility: FacilityDto): void {
    this.editingId = facility.id;
    this.form.reset({
      name: facility.name,
      code: facility.code ?? '',
      address: facility.address ?? '',
    });
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value;
    const payload = {
      name: value.name!,
      code: value.code || null,
      address: value.address || null,
    };

    if (this.editingId == null) {
      this.facilitiesService.createFacility(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.facilitiesService
        .updateFacility(this.editingId, payload)
        .subscribe({
          next: () => {
            this.showForm = false;
            this.load();
          },
        });
    }
  }

  delete(facility: FacilityDto): void {
    if (!confirm(`Delete facility ${facility.name}?`)) return;
    this.facilitiesService.deleteFacility(facility.id).subscribe({
      next: () => this.load(),
    });
  }
}

