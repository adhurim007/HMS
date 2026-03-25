import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { VisitsService } from './visits.service';
import { VisitDto } from './visits.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { PharmacyService } from '../pharmacy/pharmacy.service';
import { ProductListDto } from '../pharmacy/pharmacy.api';
import { VisitPrescriptionService } from './visit-prescription.service';
import {
  PrescriptionDto,
  PrescriptionListItemDto,
} from './visit-prescription.api';

@Component({
  selector: 'app-visit-prescription-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './visit-prescription.page.html',
  styleUrl: './visit-prescription.page.scss',
})
export class VisitPrescriptionPage implements OnInit {
  visitId!: number;
  visit: VisitDto | null = null;
  patient: PatientDto | null = null;

  products: ProductListDto[] = [];

  loading = false;
  saving = false;
  error = '';

  // History for this patient
  history: PrescriptionListItemDto[] = [];
  historyLoading = false;
  historySelected: PrescriptionListItemDto | null = null;
  historySelectedDetails: PrescriptionDto | null = null;

  readonly form = this.fb.group({
    notes: [''],
    items: this.fb.array<FormGroup>([]),
  });

  get itemsArray(): FormArray<FormGroup> {
    return this.form.get('items') as FormArray<FormGroup>;
  }

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly visitsService: VisitsService,
    private readonly patientsService: PatientsService,
    private readonly pharmacyService: PharmacyService,
    private readonly prescriptions: VisitPrescriptionService,
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.router.navigate(['/visits']);
      return;
    }
    this.visitId = Number(idParam);
    if (!this.visitId || Number.isNaN(this.visitId)) {
      this.router.navigate(['/visits']);
      return;
    }
    this.loadLookups();
    this.loadData();
  }

  private loadLookups(): void {
    this.pharmacyService
      .getProducts({
        page: 1,
        pageSize: 500,
        search: null,
        isActive: true,
        sortBy: 'name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.products = res.items ?? [];
        },
      });
  }

  private loadData(): void {
    this.loading = true;
    this.error = '';

    this.visitsService.getVisit(this.visitId).subscribe({
      next: (v) => {
        this.visit = v;
        this.patientsService.getPatient(v.patientId).subscribe({
          next: (p) => (this.patient = p),
        });
        this.loadHistory(v.patientId);
      },
    });

    this.prescriptions.getByVisit(this.visitId).subscribe({
      next: (p) => {
        this.loading = false;
        this.itemsArray.clear();
        if (p) {
          this.form.patchValue({
            notes: p.notes ?? '',
          });
          p.items.forEach((i) =>
            this.itemsArray.push(
              this.buildItemGroup({
                productId: i.productId,
                dosage: i.dosage ?? '',
                frequency: i.frequency ?? '',
                duration: i.duration ?? '',
                quantity: i.quantity,
                instructions: i.instructions ?? '',
              }),
            ),
          );
        } else {
          this.addItemRow();
        }
      },
      error: (err) => {
        this.loading = false;
        this.error =
          err?.error?.message ??
          err?.message ??
          'Failed to load prescription.';
        this.addItemRow();
      },
    });
  }

  private loadHistory(patientId: number): void {
    this.historyLoading = true;
    this.prescriptions
      .getList({
        page: 1,
        pageSize: 20,
        patientId,
        doctorId: null,
        status: null,
        from: null,
        to: null,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.historyLoading = false;
          this.history =
            res.items?.filter((x) => x.visitId !== this.visitId) ?? [];
        },
        error: () => {
          this.historyLoading = false;
          this.history = [];
        },
      });
  }

  openHistoryPrescription(p: PrescriptionListItemDto): void {
    this.historySelected = p;
    this.historySelectedDetails = null;
    this.prescriptions.getByVisit(p.visitId).subscribe({
      next: (dto) => (this.historySelectedDetails = dto),
      error: () => (this.historySelectedDetails = null),
    });
  }

  closeHistoryModal(): void {
    this.historySelected = null;
    this.historySelectedDetails = null;
  }

  private buildItemGroup(data?: {
    productId: number | null;
    dosage: string;
    frequency: string;
    duration: string;
    quantity: number;
    instructions: string;
  }): FormGroup {
    return this.fb.group({
      productId: [data?.productId ?? null, Validators.required],
      dosage: [data?.dosage ?? ''],
      frequency: [data?.frequency ?? ''],
      duration: [data?.duration ?? ''],
      quantity: [data?.quantity ?? 1, [Validators.required, Validators.min(1)]],
      instructions: [data?.instructions ?? ''],
    });
  }

  addItemRow(): void {
    this.itemsArray.push(this.buildItemGroup());
  }

  removeItemRow(index: number): void {
    this.itemsArray.removeAt(index);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;
    const items =
      this.itemsArray.controls
        .map((ctrl) => ctrl.value as any)
        .filter((i) => i.productId && i.quantity && i.quantity > 0)
        .map((i) => ({
          productId: Number(i.productId),
          dosage: i.dosage || null,
          frequency: i.frequency || null,
          duration: i.duration || null,
          quantity: Number(i.quantity),
          instructions: i.instructions || null,
        })) ?? [];

    this.saving = true;
    this.error = '';

    this.prescriptions
      .upsert({
        visitId: this.visitId,
        notes: v.notes || null,
        items,
      })
      .subscribe({
        next: () => {
          this.saving = false;
          this.router.navigate(['/visits']);
        },
        error: (err) => {
          this.saving = false;
          this.error =
            err?.error?.message ??
            err?.message ??
            'Failed to save prescription.';
        },
      });
  }
}

