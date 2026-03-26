import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FacilitiesService } from './facilities.service';
import { FacilityDto } from './facilities.api';

@Component({
  selector: 'app-facilities-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './facilities.page.html',
  styleUrl: './facilities.page.scss',
})
export class FacilitiesPage implements OnInit {
  facilities: FacilityDto[] = [];
  facilityLookup: FacilityDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = null;
  sortDesc = false;
  loading = false;

  editingId: number | null = null;
  showForm = false;
  parentFacilityInput = '';

  readonly form = this.fb.group({
    name: ['', Validators.required],
    code: [''],
    address: [''],
    parentId: [null as number | null],
  });

  readonly math = Math;

  constructor(
    private readonly facilitiesService: FacilitiesService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadAllFacilities();
    this.load();
  }

  loadAllFacilities(): void {
    this.facilitiesService
      .getFacilities({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'name',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.facilityLookup = res.items;
        },
      });
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
    this.parentFacilityInput = '';
    this.form.reset({
      name: '',
      code: '',
      address: '',
      parentId: null,
    });
    this.parentFacilityInput = '';
    this.showForm = true;
  }

  openEdit(facility: FacilityDto): void {
    this.editingId = facility.id;
    this.parentFacilityInput = '';
    this.form.reset({
      name: facility.name,
      code: facility.code ?? '',
      address: facility.address ?? '',
      parentId: facility.parentId ?? null,
    });
    const selected = this.facilityLookup.find((f) => f.id === (facility.parentId ?? null));
    this.parentFacilityInput = selected ? this.getFacilityOptionLabel(selected) : '';
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
      parentId: value.parentId ?? null,
    };

    if (this.editingId == null) {
      this.facilitiesService.createFacility(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.loadAllFacilities();
          this.load();
        },
      });
    } else {
      this.facilitiesService
        .updateFacility(this.editingId, payload)
        .subscribe({
          next: () => {
            this.showForm = false;
            this.loadAllFacilities();
            this.load();
          },
        });
    }
  }

  delete(facility: FacilityDto): void {
    if (!confirm(`Delete facility ${facility.name}?`)) return;
    this.facilitiesService.deleteFacility(facility.id).subscribe({
      next: () => {
        this.loadAllFacilities();
        this.load();
      },
    });
  }

  getParentName(parentId: number | null | undefined): string {
    if (parentId == null) return '-';
    const parent = this.facilityLookup.find((x) => x.id === parentId);
    return parent ? parent.name : String(parentId);
  }

  getParentOptions(currentId: number | null): FacilityDto[] {
    return this.facilityLookup.filter((f) => f.id !== currentId);
  }

  onParentFacilityChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.form.patchValue({ parentId: null });
      return;
    }
    const selected = this.getParentOptions(this.editingId).find(
      (f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase(),
    );
    this.form.patchValue({ parentId: selected?.id ?? null });
  }

  getFacilityOptionLabel(facility: FacilityDto): string {
    return facility.code ? `${facility.name} (${facility.code})` : facility.name;
  }
}

