import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormArray,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BillingService } from './billing.service';
import {
  InvoiceListDto,
  InvoiceDto,
  ServiceItemListDto,
  InvoiceLineInput,
  UnbilledLaboratoryItemDto,
  UnbilledVisitServiceDto,
} from './billing.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';

@Component({
  selector: 'app-billing-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './billing.page.html',
  styleUrl: './billing.page.scss',
})
export class BillingPage implements OnInit {
  viewMode: 'invoices' | 'services' = 'invoices';

  // Invoices
  invoices: InvoiceListDto[] = [];
  invoicesTotal = 0;
  invoicesPage = 1;
  invoicesPageSize = 10;
  invoicesLoading = false;
  patientFilter: number | null = null;
  patientFilterSearchTerm = '';
  patientFilterSearchResults: PatientDto[] = [];
  loadingPatientFilterSearch = false;
  statusFilter: string | null = null;
  fromFilter: string | null = null;
  toFilter: string | null = null;
  showInvoiceForm = false;
  showInvoiceDetail = false;
  selectedInvoice: InvoiceDto | null = null;
  patients: PatientDto[] = [];
  patientSearchTerm = '';
  patientSearchResults: PatientDto[] = [];
  selectedPatient: PatientDto | null = null;
  loadingPatientSearch = false;
  serviceItems: ServiceItemListDto[] = [];
  invoiceError: string | null = null;
  invoiceSubmitting = false;

  invoiceSource: 'manual' | 'visit' | 'laboratory' = 'manual';
  unbilledVisitServices: UnbilledVisitServiceDto[] = [];
  unbilledLaboratoryItems: UnbilledLaboratoryItemDto[] = [];
  unbilledLoading = false;
  selectedVisitServiceIds = new Set<number>();
  selectedLabItemIds = new Set<number>();
  private handledRouteIntent = false;

  invoiceForm = this.fb.group({
    patientId: [null as number | null, [Validators.required]],
    invoiceDate: [''],
    items: this.fb.array<ReturnType<typeof this.createItemGroup>>([]),
  });

  paymentForm = this.fb.group({
    amount: [0, [Validators.required, Validators.min(0.01)]],
    method: [''],
    reference: [''],
    paymentDate: [''],
  });
  readonly paymentMethodOptions: string[] = ['Cash', 'Card', 'Bank Transfer', 'Insurance', 'POS'];

  // Services
  services: ServiceItemListDto[] = [];
  servicesTotal = 0;
  servicesPage = 1;
  servicesPageSize = 10;
  servicesLoading = false;
  servicesSearch = '';
  servicesActiveFilter: boolean | null = null;
  editingServiceId: number | null = null;
  serviceForm = this.fb.group({
    code: ['', [Validators.required]],
    name: ['', [Validators.required]],
    price: [0, [Validators.required, Validators.min(0)]],
    isActive: [true],
  });

  readonly statuses = [
    { value: '', label: 'All' },
    { value: 'Draft', label: 'Draft' },
    { value: 'Unpaid', label: 'Unpaid' },
    { value: 'PartiallyPaid', label: 'Partially paid' },
    { value: 'Paid', label: 'Paid' },
    { value: 'Cancelled', label: 'Cancelled' },
  ];

  constructor(
    private readonly billing: BillingService,
    private readonly patientsService: PatientsService,
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    const initialView = String(this.route.snapshot.data['view'] ?? 'invoices');
    this.viewMode = initialView === 'services' ? 'services' : 'invoices';
    this.route.data.subscribe((data) => {
      const view = String(data['view'] ?? 'invoices');
      this.viewMode = view === 'services' ? 'services' : 'invoices';
    });

    this.loadPatients();
    this.loadServiceItemsForSelect();
    if (this.viewMode === 'services') {
      this.loadServices();
    } else {
      this.loadInvoices();
      this.loadServices();
    }
    this.route.queryParamMap.subscribe((params) => {
      if (this.handledRouteIntent) return;
      const source = (params.get('source') ?? '').toLowerCase();
      const patientIdRaw = Number(params.get('patientId'));
      if (source !== 'laboratory' || Number.isNaN(patientIdRaw) || patientIdRaw <= 0) return;

      this.handledRouteIntent = true;
      this.viewMode = 'invoices';
      this.openNewInvoice();
      this.invoiceSource = 'laboratory';
      this.patientsService.getPatient(patientIdRaw).subscribe({
        next: (p) => this.selectPatientForInvoice(p),
      });
    });
  }

  loadPatients(): void {
    this.patientsService
      .getPatients({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'fullName',
        sortDesc: false,
        search: null,
      })
      .subscribe({
        next: (res) => {
          this.patients = (res as { items?: PatientDto[]; Items?: PatientDto[] }).items ?? (res as { Items?: PatientDto[] }).Items ?? [];
        },
        error: () => {
          this.patients = [];
        },
      });
  }

  loadServiceItemsForSelect(): void {
    this.billing
      .getServiceItems({
        page: 1,
        pageSize: 500,
        search: null,
        isActive: null,
        sortBy: 'Name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.serviceItems = (res as { items?: ServiceItemListDto[]; Items?: ServiceItemListDto[] }).items ?? (res as { Items?: ServiceItemListDto[] }).Items ?? [];
        },
        error: () => {
          this.serviceItems = [];
        },
      });
  }

  get itemsArray(): FormArray {
    return this.invoiceForm.get('items') as FormArray;
  }

  createItemGroup(visitService?: UnbilledVisitServiceDto): ReturnType<FormBuilder['group']> {
    if (visitService) {
      return this.fb.group({
        visitServiceId: [visitService.id],
        laboratoryOrderItemId: [null as number | null],
        serviceItemId: [visitService.serviceItemId],
        description: [visitService.serviceName, Validators.required],
        unitPrice: [visitService.unitPrice, [Validators.required, Validators.min(0)]],
        quantity: [visitService.quantity, [Validators.required, Validators.min(0.01)]],
      });
    }
    return this.fb.group({
      visitServiceId: [null as number | null],
      laboratoryOrderItemId: [null as number | null],
      serviceItemId: [null as number | null],
      description: ['', Validators.required],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      quantity: [1, [Validators.required, Validators.min(0.01)]],
    });
  }

  createItemGroupFromLabItem(labItem: UnbilledLaboratoryItemDto): ReturnType<FormBuilder['group']> {
    return this.fb.group({
      visitServiceId: [null as number | null],
      laboratoryOrderItemId: [labItem.id],
      serviceItemId: [null as number | null],
      description: [labItem.testName, Validators.required],
      unitPrice: [labItem.unitPrice, [Validators.required, Validators.min(0)]],
      quantity: [1, [Validators.required, Validators.min(0.01)]],
    });
  }

  addInvoiceLine(): void {
    this.itemsArray.push(this.createItemGroup());
  }

  removeInvoiceLine(i: number): void {
    this.itemsArray.removeAt(i);
  }

  onServiceSelect(index: number): void {
    const group = this.itemsArray.at(index);
    const serviceId = group.get('serviceItemId')?.value;
    const item = this.serviceItems.find((s) => s.id === serviceId);
    if (item) {
      group.patchValue({
        description: item.name,
        unitPrice: item.price,
      });
    }
  }

  loadInvoices(): void {
    this.invoicesLoading = true;
    this.billing
      .getInvoices({
        patientId: this.patientFilter,
        status: this.statusFilter,
        from: this.fromFilter,
        to: this.toFilter,
        page: this.invoicesPage,
        pageSize: this.invoicesPageSize,
        sortBy: 'InvoiceDate',
        sortDescending: true,
      })
      .subscribe({
        next: (res) => {
          this.invoicesLoading = false;
          this.invoices = res.items;
          this.invoicesTotal = res.totalCount;
        },
        error: () => (this.invoicesLoading = false),
      });
  }

  applyInvoiceFilters(): void {
    this.invoicesPage = 1;
    this.loadInvoices();
  }

  clearInvoiceFilters(): void {
    this.patientFilter = null;
    this.patientFilterSearchTerm = '';
    this.patientFilterSearchResults = [];
    this.statusFilter = null;
    this.fromFilter = null;
    this.toFilter = null;
    this.invoicesPage = 1;
    this.loadInvoices();
  }

  searchPatientsForInvoiceFilter(): void {
    const term = this.patientFilterSearchTerm.trim();
    if (term.length < 2) {
      this.patientFilterSearchResults = [];
      return;
    }
    this.loadingPatientFilterSearch = true;
    this.patientsService
      .getPatients({
        pageNumber: 1,
        pageSize: 20,
        sortBy: 'fullName',
        sortDesc: false,
        search: term,
      })
      .subscribe({
        next: (res) => {
          this.loadingPatientFilterSearch = false;
          this.patientFilterSearchResults = (res as { items?: PatientDto[]; Items?: PatientDto[] }).items ?? (res as { Items?: PatientDto[] }).Items ?? [];
        },
        error: () => {
          this.loadingPatientFilterSearch = false;
          this.patientFilterSearchResults = [];
        },
      });
  }

  selectPatientForInvoiceFilter(p: PatientDto): void {
    this.patientFilter = p.id;
    this.patientFilterSearchTerm = `${p.fullName} (${p.medicalRecordNumber})`;
    this.patientFilterSearchResults = [];
    this.applyInvoiceFilters();
  }

  clearInvoicePatientFilterOnly(): void {
    this.patientFilter = null;
    this.patientFilterSearchTerm = '';
    this.patientFilterSearchResults = [];
    this.applyInvoiceFilters();
  }

  openNewInvoice(): void {
    this.invoiceError = null;
    this.invoiceSource = 'manual';
    this.unbilledVisitServices = [];
    this.unbilledLaboratoryItems = [];
    this.selectedVisitServiceIds = new Set();
    this.selectedLabItemIds = new Set();
    this.patientSearchTerm = '';
    this.patientSearchResults = [];
    this.selectedPatient = null;
    this.invoiceForm.reset({
      patientId: null,
      invoiceDate: new Date().toISOString().substring(0, 10),
      items: [],
    });
    this.itemsArray.clear();
    this.showInvoiceForm = true;
    this.showInvoiceDetail = false;
    this.loadServiceItemsForSelect();
  }

  searchPatientsForInvoice(): void {
    const term = this.patientSearchTerm.trim();
    if (term.length < 2) {
      this.patientSearchResults = [];
      return;
    }
    this.loadingPatientSearch = true;
    this.patientsService
      .getPatients({
        pageNumber: 1,
        pageSize: 20,
        sortBy: 'fullName',
        sortDesc: false,
        search: term,
      })
      .subscribe({
        next: (res) => {
          this.loadingPatientSearch = false;
          this.patientSearchResults = (res as { items?: PatientDto[]; Items?: PatientDto[] }).items ?? (res as { Items?: PatientDto[] }).Items ?? [];
        },
        error: () => {
          this.loadingPatientSearch = false;
          this.patientSearchResults = [];
        },
      });
  }

  selectPatientForInvoice(p: PatientDto): void {
    this.selectedPatient = p;
    this.patientSearchTerm = `${p.fullName} (${p.medicalRecordNumber})`;
    this.patientSearchResults = [];
    this.invoiceForm.patchValue({ patientId: p.id });
    this.itemsArray.clear();
    this.selectedVisitServiceIds = new Set();
    this.selectedLabItemIds = new Set();
    this.loadUnbilledForCurrentSource();
  }

  clearSelectedPatientForInvoice(): void {
    this.selectedPatient = null;
    this.patientSearchTerm = '';
    this.patientSearchResults = [];
    this.invoiceForm.patchValue({ patientId: null });
    this.itemsArray.clear();
    this.unbilledVisitServices = [];
    this.unbilledLaboratoryItems = [];
    this.selectedVisitServiceIds = new Set();
    this.selectedLabItemIds = new Set();
  }

  onInvoiceSourceChanged(source: 'manual' | 'visit' | 'laboratory'): void {
    this.invoiceSource = source;
    this.selectedVisitServiceIds = new Set();
    this.selectedLabItemIds = new Set();
    this.itemsArray.clear();
    this.loadUnbilledForCurrentSource();
  }

  loadUnbilledForCurrentSource(): void {
    if (!this.invoiceForm.get('patientId')?.value) return;
    if (this.invoiceSource === 'visit') {
      this.loadUnbilledVisitServices();
      this.unbilledLaboratoryItems = [];
      return;
    }
    if (this.invoiceSource === 'laboratory') {
      this.loadUnbilledLaboratoryItems();
      this.unbilledVisitServices = [];
      return;
    }
    this.unbilledVisitServices = [];
    this.unbilledLaboratoryItems = [];
  }

  loadUnbilledVisitServices(): void {
    const patientId = this.invoiceForm.get('patientId')?.value;
    if (!patientId) return;
    this.unbilledLoading = true;
    this.billing.getUnbilledVisitServices({ patientId }).subscribe({
      next: (list) => {
        this.unbilledLoading = false;
        this.unbilledVisitServices = list;
        this.selectedVisitServiceIds = new Set();
      },
      error: () => (this.unbilledLoading = false),
    });
  }

  loadUnbilledLaboratoryItems(): void {
    const patientId = this.invoiceForm.get('patientId')?.value;
    if (!patientId) return;
    this.unbilledLoading = true;
    this.billing
      .getUnbilledLaboratoryItems({ patientId })
      .subscribe({
        next: (list) => {
          this.unbilledLoading = false;
          this.unbilledLaboratoryItems = list;
          this.selectedLabItemIds = new Set();
        },
        error: () => (this.unbilledLoading = false),
      });
  }

  toggleVisitServiceSelection(id: number): void {
    if (this.selectedVisitServiceIds.has(id)) this.selectedVisitServiceIds.delete(id);
    else this.selectedVisitServiceIds.add(id);
    this.selectedVisitServiceIds = new Set(this.selectedVisitServiceIds);
  }

  toggleLabItemSelection(id: number): void {
    if (this.selectedLabItemIds.has(id)) this.selectedLabItemIds.delete(id);
    else this.selectedLabItemIds.add(id);
    this.selectedLabItemIds = new Set(this.selectedLabItemIds);
  }

  addSelectedUnbilledToInvoice(): void {
    if (this.invoiceSource === 'laboratory') {
      const addedLabs = this.unbilledLaboratoryItems.filter((u) => this.selectedLabItemIds.has(u.id));
      const alreadyInForm = new Set(
        this.itemsArray.controls
          .map((c) => c.get('laboratoryOrderItemId')?.value)
          .filter((id): id is number => id != null),
      );
      for (const u of addedLabs) {
        if (alreadyInForm.has(u.id)) continue;
        this.itemsArray.push(this.createItemGroupFromLabItem(u));
        alreadyInForm.add(u.id);
      }
      this.selectedLabItemIds = new Set();
      return;
    }
    if (this.invoiceSource === 'visit') {
      const addedVisits = this.unbilledVisitServices.filter((u) => this.selectedVisitServiceIds.has(u.id));
      const visitIdsInForm = new Set(
        this.itemsArray.controls
          .map((c) => c.get('visitServiceId')?.value)
          .filter((id): id is number => id != null),
      );
      for (const u of addedVisits) {
        if (visitIdsInForm.has(u.id)) continue;
        this.itemsArray.push(this.createItemGroup(u));
        visitIdsInForm.add(u.id);
      }
      this.selectedVisitServiceIds = new Set();
    }
  }

  closeInvoiceForm(): void {
    this.showInvoiceForm = false;
  }

  submitInvoice(): void {
    if (this.invoiceForm.invalid || this.itemsArray.length === 0) {
      this.invoiceForm.markAllAsTouched();
      return;
    }
    this.invoiceError = null;
    this.invoiceSubmitting = true;
    const v = this.invoiceForm.value;
    const items: InvoiceLineInput[] = this.itemsArray.controls.map((c) => {
      const val = c.value;
      const qty = Number(val.quantity);
      const price = Number(val.unitPrice);
      return {
        visitServiceId: val.visitServiceId ?? null,
        laboratoryOrderItemId: val.laboratoryOrderItemId ?? null,
        serviceItemId: val.serviceItemId ?? null,
        description: (val.description ?? '').trim() || 'Line item',
        unitPrice: isNaN(price) || price < 0 ? 0 : price,
        quantity: isNaN(qty) || qty <= 0 ? 1 : qty,
      };
    });
    this.billing
      .createInvoice({
        patientId: Number(v.patientId),
        invoiceDate: (v.invoiceDate as string) || null,
        items,
      })
      .subscribe({
        next: () => {
          this.invoiceSubmitting = false;
          this.invoiceError = null;
          this.closeInvoiceForm();
          this.loadInvoices();
        },
        error: (err) => {
          this.invoiceSubmitting = false;
          const body = err.error;
          if (body?.message) {
            this.invoiceError = body.message;
          } else if (body?.errors && typeof body.errors === 'object') {
            const msgs = Object.values(body.errors).flat();
            this.invoiceError = Array.isArray(msgs) ? msgs.join(' ') : String(body.errors);
          } else {
            this.invoiceError = err.message || 'Failed to create invoice. Please try again.';
          }
        },
      });
  }

  viewInvoice(id: number): void {
    this.billing.getInvoice(id).subscribe({
      next: (inv) => {
        this.selectedInvoice = inv;
        this.showInvoiceDetail = true;
        this.showInvoiceForm = false;
        this.paymentForm.reset({
          amount: Math.max(0, inv.totalAmount - inv.paidAmount),
          method: this.paymentMethodOptions[0],
          reference: '',
          paymentDate: new Date().toISOString().substring(0, 10),
        });
      },
    });
  }

  closeInvoiceDetail(): void {
    this.showInvoiceDetail = false;
    this.selectedInvoice = null;
  }

  submitPayment(): void {
    if (!this.selectedInvoice || this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    const v = this.paymentForm.value;
    this.billing
      .addPayment({
        invoiceId: this.selectedInvoice.id,
        amount: Number(v.amount),
        method: v.method || null,
        reference: v.reference || null,
        paymentDate: v.paymentDate || null,
      })
      .subscribe({
        next: () => {
          this.billing.getInvoice(this.selectedInvoice!.id).subscribe((inv) => {
            this.selectedInvoice = inv;
            this.loadInvoices();
          });
          this.paymentForm.reset({
            amount: Math.max(0, this.selectedInvoice!.totalAmount - this.selectedInvoice!.paidAmount),
            method: this.paymentMethodOptions[0],
            reference: '',
            paymentDate: new Date().toISOString().substring(0, 10),
          });
        },
      });
  }

  getStatusLabel(s: unknown): string {
    if (s === 1 || s === 'Draft') return 'Draft';
    if (s === 2 || s === 'Unpaid') return 'Unpaid';
    if (s === 3 || s === 'PartiallyPaid') return 'Partially paid';
    if (s === 4 || s === 'Paid') return 'Paid';
    if (s === 5 || s === 'Cancelled') return 'Cancelled';
    return String(s ?? '');
  }

  getPatientName(id: number): string {
    const p = this.patients.find((x) => x.id === id);
    return p ? p.fullName : String(id);
  }

  // --- Services ---
  loadServices(): void {
    this.servicesLoading = true;
    this.billing
      .getServiceItems({
        page: this.servicesPage,
        pageSize: this.servicesPageSize,
        search: this.servicesSearch || null,
        isActive: this.servicesActiveFilter,
        sortBy: 'Name',
        sortDescending: false,
      })
      .subscribe({
        next: (res) => {
          this.servicesLoading = false;
          this.services = res.items;
          this.servicesTotal = res.totalCount;
        },
        error: () => (this.servicesLoading = false),
      });
  }

  onServicesSearch(): void {
    if (this.viewMode !== 'services') return;
    this.servicesPage = 1;
    this.loadServices();
  }

  applyServiceFilters(): void {
    if (this.viewMode !== 'services') return;
    this.servicesPage = 1;
    this.loadServices();
  }

  openNewService(): void {
    this.editingServiceId = null;
    this.serviceForm.reset({ code: '', name: '', price: 0, isActive: true });
  }

  openEditService(s: ServiceItemListDto): void {
    this.editingServiceId = s.id;
    this.serviceForm.reset({
      code: s.code,
      name: s.name,
      price: s.price,
      isActive: s.isActive,
    });
  }

  cancelServiceEdit(): void {
    this.editingServiceId = null;
    this.serviceForm.reset({ code: '', name: '', price: 0, isActive: true });
  }

  submitService(): void {
    if (this.serviceForm.invalid) {
      this.serviceForm.markAllAsTouched();
      return;
    }
    const v = this.serviceForm.value;
    if (this.editingServiceId == null) {
      this.billing
        .createServiceItem({
          code: v.code!,
          name: v.name!,
          price: Number(v.price),
        })
        .subscribe({
          next: () => {
            this.cancelServiceEdit();
            this.loadServices();
            this.loadServiceItemsForSelect();
          },
        });
    } else {
      this.billing
        .updateServiceItem(this.editingServiceId, {
          name: v.name!,
          price: Number(v.price),
          isActive: !!v.isActive,
        })
        .subscribe({
          next: () => {
            this.cancelServiceEdit();
            this.loadServices();
            this.loadServiceItemsForSelect();
          },
        });
    }
  }

  deleteService(s: ServiceItemListDto): void {
    if (!confirm(`Delete service "${s.name}"?`)) return;
    this.billing.deleteServiceItem(s.id).subscribe({
      next: () => {
        this.loadServices();
        this.loadServiceItemsForSelect();
      },
    });
  }

  changeInvoicesPage(delta: number): void {
    const next = this.invoicesPage + delta;
    if (next < 1) return;
    const max = Math.max(1, Math.ceil(this.invoicesTotal / this.invoicesPageSize));
    if (next > max) return;
    this.invoicesPage = next;
    this.loadInvoices();
  }

  changeServicesPage(delta: number): void {
    const next = this.servicesPage + delta;
    if (next < 1) return;
    const max = Math.max(1, Math.ceil(this.servicesTotal / this.servicesPageSize));
    if (next > max) return;
    this.servicesPage = next;
    this.loadServices();
  }
}
