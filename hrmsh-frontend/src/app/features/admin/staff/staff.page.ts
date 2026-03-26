import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { StaffMemberDto, StaffTypeOption } from './staff.api';
import { StaffService } from './staff.service';
import { FacilitiesService } from '../facilities/facilities.service';
import { FacilityDto } from '../facilities/facilities.api';
import { DepartmentsService } from '../departments/departments.service';
import { DepartmentDto } from '../departments/departments.api';

@Component({
  selector: 'app-staff-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './staff.page.html',
})
export class StaffPage implements OnInit {
  readonly staffTypes: StaffTypeOption[] = [
    { value: 1, label: 'Doctor' },
    { value: 2, label: 'Nurse' },
    { value: 3, label: 'Reception' },
    { value: 4, label: 'Pharmacist' },
    { value: 5, label: 'Finance' },
    { value: 6, label: 'Manager' },
    { value: 99, label: 'Other' },
  ];

  facilities: FacilityDto[] = [];
  departments: DepartmentDto[] = [];
  filteredDepartments: DepartmentDto[] = [];
  departmentFilterOptions: DepartmentDto[] = [];

  items: StaffMemberDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  loading = false;

  search = '';
  staffTypeFilter: number | null = null;
  facilityFilter: number | null = null;
  departmentFilter: number | null = null;
  activeFilter: boolean | null = null;

  facilityFormInput = '';
  facilityFilterInput = '';

  readonly form = this.fb.group({
    fullName: ['', Validators.required],
    staffType: [3, Validators.required],
    phone: [''],
    email: [''],
    userId: [null as number | null],
    facilityId: [null as number | null, Validators.required],
    departmentId: [null as number | null],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly staffService: StaffService,
    private readonly facilitiesService: FacilitiesService,
    private readonly departmentsService: DepartmentsService,
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.load();
  }

  private loadLookups(): void {
    this.facilitiesService
      .getFacilities({ pageNumber: 1, pageSize: 1000, sortBy: 'name', sortDesc: false })
      .subscribe({
        next: (res) => (this.facilities = res.items),
      });

    this.departmentsService
      .getDepartments({ pageNumber: 1, pageSize: 1000, sortBy: 'name', sortDesc: false })
      .subscribe({
        next: (res) => {
          this.departments = res.items;
          this.onFacilityChange();
          this.onFacilityFilterChange();
        },
      });
  }

  load(): void {
    this.loading = true;
    this.staffService
      .getStaff({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        search: this.search || null,
        staffType: this.staffTypeFilter,
        facilityId: this.facilityFilter,
        departmentId: this.departmentFilter,
        isActive: this.activeFilter,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.items = res.items;
          this.totalCount = res.totalCount;
        },
        error: () => (this.loading = false),
      });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;
    this.staffService
      .createStaff({
        fullName: v.fullName!,
        staffType: Number(v.staffType),
        phone: v.phone || null,
        email: v.email || null,
        userId: v.userId ? Number(v.userId) : null,
        departmentId: v.departmentId ?? null,
        facilityIds: v.facilityId != null ? [Number(v.facilityId)] : [],
      })
      .subscribe({
        next: () => {
          this.form.reset({
            fullName: '',
            staffType: 3,
            phone: '',
            email: '',
            userId: null,
            facilityId: null,
            departmentId: null,
          });
          this.facilityFormInput = '';
          this.filteredDepartments = [];
          this.pageNumber = 1;
          this.load();
        },
      });
  }

  onFacilityChange(): void {
    const facilityId = this.form.value.facilityId ?? null;
    this.filteredDepartments = this.departments.filter((d) => d.facilityId === facilityId);
    const departmentId = this.form.value.departmentId ?? null;
    if (departmentId != null && !this.filteredDepartments.some((d) => d.id === departmentId)) {
      this.form.patchValue({ departmentId: null });
    }
  }

  onFacilityFormInputChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.form.patchValue({ facilityId: null });
      this.onFacilityChange();
      return;
    }
    const selected = this.facilities.find(
      (f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase(),
    );
    this.form.patchValue({ facilityId: selected?.id ?? null });
    this.onFacilityChange();
  }

  onFacilityFilterInputChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.facilityFilter = null;
    } else {
      const selected = this.facilities.find(
        (f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase(),
      );
      this.facilityFilter = selected?.id ?? null;
    }
    this.onFacilityFilterChange();
    this.applyFilters();
  }

  onFacilityFilterChange(): void {
    this.departmentFilterOptions = this.departments.filter(
      (d) => this.facilityFilter == null || d.facilityId === this.facilityFilter,
    );
    if (
      this.departmentFilter != null &&
      !this.departmentFilterOptions.some((d) => d.id === this.departmentFilter)
    ) {
      this.departmentFilter = null;
    }
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.search = '';
    this.staffTypeFilter = null;
    this.facilityFilter = null;
    this.departmentFilter = null;
    this.activeFilter = null;
    this.facilityFilterInput = '';
    this.onFacilityFilterChange();
    this.applyFilters();
  }

  changePage(delta: number): void {
    const next = this.pageNumber + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.pageSize));
    if (next > maxPage) return;
    this.pageNumber = next;
    this.load();
  }

  toggleActive(item: StaffMemberDto): void {
    this.staffService.setActive(item.id, !item.isActive).subscribe({
      next: () => this.load(),
    });
  }

  getFacilityOptionLabel(facility: FacilityDto): string {
    return facility.code ? `${facility.name} (${facility.code})` : facility.name;
  }

  getFacilityNames(item: StaffMemberDto): string {
    if (!item.facilityIds?.length) return '-';
    return item.facilityIds
      .map((id) => this.facilities.find((f) => f.id === id)?.name ?? String(id))
      .join(', ');
  }

  getDepartmentName(id: number | null | undefined): string {
    if (id == null) return '-';
    return this.departments.find((d) => d.id === id)?.name ?? String(id);
  }

  getStaffTypeLabel(value: number): string {
    return this.staffTypes.find((s) => s.value === value)?.label ?? `Type ${value}`;
  }
}
