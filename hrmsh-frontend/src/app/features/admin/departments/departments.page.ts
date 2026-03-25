import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DepartmentsService } from './departments.service';
import { FacilitiesService } from '../facilities/facilities.service';
import { DepartmentDto } from './departments.api';
import { FacilityDto } from '../facilities/facilities.api';

@Component({
  selector: 'app-departments-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './departments.page.html',
  styleUrl: './departments.page.scss',
})
export class DepartmentsPage implements OnInit {
  departments: DepartmentDto[] = [];
  facilities: FacilityDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  search = '';
  sortBy: string | null = null;
  sortDesc = false;
  loading = false;
  facilityFilter: number | null = null;

  editingId: number | null = null;
  showForm = false;

  readonly form = this.fb.group({
    name: ['', Validators.required],
    code: [''],
    facilityId: [null as number | null, Validators.required],
  });

  readonly math = Math;

  constructor(
    private readonly departmentsService: DepartmentsService,
    private readonly facilitiesService: FacilitiesService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.loadFacilities();
    this.load();
  }

  loadFacilities(): void {
    this.facilitiesService
      .getFacilities({
        pageNumber: 1,
        pageSize: 1000,
        sortBy: 'name',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => (this.facilities = res.items),
      });
  }

  load(): void {
    this.loading = true;
    this.departmentsService
      .getDepartments({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
        facilityId: this.facilityFilter,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.departments = res.items;
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

  onFacilityFilterChange(value: string): void {
    this.facilityFilter = value ? Number(value) : null;
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
      facilityId: null,
    });
    this.showForm = true;
  }

  openEdit(dept: DepartmentDto): void {
    this.editingId = dept.id;
    this.form.reset({
      name: dept.name,
      code: dept.code ?? '',
      facilityId: dept.facilityId,
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
      facilityId: Number(value.facilityId),
    };

    if (this.editingId == null) {
      this.departmentsService.createDepartment(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.departmentsService
        .updateDepartment(this.editingId, payload)
        .subscribe({
          next: () => {
            this.showForm = false;
            this.load();
          },
        });
    }
  }

  delete(dept: DepartmentDto): void {
    if (!confirm(`Delete department ${dept.name}?`)) return;
    this.departmentsService.deleteDepartment(dept.id).subscribe({
      next: () => this.load(),
    });
  }

  getFacilityName(id: number): string | number {
    const f = this.facilities.find((x) => x.id === id);
    return f?.name ?? id;
  }
}

