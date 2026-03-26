import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HospitalDto } from './hospitals.api';
import { HospitalsService } from './hospitals.service';

@Component({
  selector: 'app-hospitals-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './hospitals.page.html',
  styleUrl: './hospitals.page.scss',
})
export class HospitalsPage implements OnInit {
  hospitals: HospitalDto[] = [];
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
    private readonly hospitalsService: HospitalsService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.hospitalsService
      .getHospitals({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        sortBy: this.sortBy,
        sortDesc: this.sortDesc,
        search: this.search || null,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.hospitals = res.items;
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

  openEdit(hospital: HospitalDto): void {
    this.editingId = hospital.id;
    this.form.reset({
      name: hospital.name,
      code: hospital.code ?? '',
      address: hospital.address ?? '',
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
      this.hospitalsService.createHospital(payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    } else {
      this.hospitalsService.updateHospital(this.editingId, payload).subscribe({
        next: () => {
          this.showForm = false;
          this.load();
        },
      });
    }
  }

  delete(hospital: HospitalDto): void {
    if (!confirm(`Delete hospital ${hospital.name}?`)) return;
    this.hospitalsService.deleteHospital(hospital.id).subscribe({
      next: () => this.load(),
    });
  }
}
