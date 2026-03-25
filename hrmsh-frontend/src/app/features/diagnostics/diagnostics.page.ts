import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DiagnosticsService } from './diagnostics.service';
import { DiagnosticTestDto, LaboratoryCollectorDto, LaboratoryOrderDto, PatientLabHistoryRowDto } from './diagnostics.api';
import { PatientsService } from '../patients/patients.service';
import { PatientDto } from '../patients/patients.api';
import { DoctorsService } from '../doctors/doctors.service';
import { DoctorDto } from '../doctors/doctors.api';
import { AuthService } from '../../core/services/auth.service';
import { VisitsService } from '../visits/visits.service';
import { VisitListDto } from '../visits/visits.api';

@Component({
  selector: 'app-diagnostics-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './diagnostics.page.html',
  styleUrl: './diagnostics.page.scss',
})
export class DiagnosticsPage implements OnInit {
  tab: 'tests' | 'laboratory' | 'laboratory-orders' = 'laboratory';

  tests: DiagnosticTestDto[] = [];
  labOrders: LaboratoryOrderDto[] = [];
  selectedLabOrder: LaboratoryOrderDto | null = null;
  patientLabHistory: PatientLabHistoryRowDto[] = [];
  selectedHistoryPatientId: number | null = null;
  selectedHistoryPatientName = '';
  selectedLabOrderId: number | null = null;

  patients: PatientDto[] = [];
  doctors: DoctorDto[] = [];
  collectors: LaboratoryCollectorDto[] = [];
  labVisitSearchResults: VisitListDto[] = [];
  patientSearchTerm = '';
  patientSearchResults: PatientDto[] = [];
  selectedPatient: PatientDto | null = null;
  loadingPatientSearch = false;

  loadingTests = false;

  readonly testForm = this.fb.group({
    id: [null as number | null],
    code: ['', Validators.required],
    name: ['', Validators.required],
    type: [1, Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    isActive: [true],
  });

  readonly labOrderForm = this.fb.group({
    patientId: [null as number | null, Validators.required],
    visitId: [null as number | null],
    referringDoctorId: [null as number | null],
    clinicalIndication: [''],
    priority: [2],
    testIds: [[] as number[]],
  });

  readonly markPaidForm = this.fb.group({
    paymentMethod: ['Cash'],
  });

  readonly paymentMethodOptions: string[] = [
    'Cash',
    'Card',
    'Bank Transfer',
    'Insurance',
    'POS',
  ];

  readonly sampleForm = this.fb.group({
    sampleType: ['Blood', Validators.required],
    collectedAt: [''],
    collectedById: [null as number | null, Validators.required],
    sampleBarcode: ['', Validators.required],
  });

  readonly resultFormLab = this.fb.group({
    laboratoryOrderItemId: [null as number | null, Validators.required],
    laboratorySampleId: [null as number | null, Validators.required],
    value: ['', Validators.required],
    unit: [''],
    referenceRange: [''],
    flag: [1],
    enteredById: [null as number | null, Validators.required],
  });

  readonly validateLabForm = this.fb.group({
    validatedById: [null as number | null, Validators.required],
  });

  readonly statusOrder: Array<{ code: number; label: string }> = [
    { code: 1, label: 'Ordered' },
    { code: 2, label: 'Paid' },
    { code: 3, label: 'SampleCollected' },
    { code: 4, label: 'InProcess' },
    { code: 5, label: 'Completed' },
    { code: 6, label: 'Validated' },
    { code: 7, label: 'Delivered' },
  ];

  constructor(
    private readonly diagnostics: DiagnosticsService,
    private readonly patientsService: PatientsService,
    private readonly doctorsService: DoctorsService,
    private readonly visitsService: VisitsService,
    private readonly auth: AuthService,
    private readonly route: ActivatedRoute,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((d) => {
      const view = String(d['view'] ?? 'laboratory');
      this.tab = view === 'tests' || view === 'laboratory' || view === 'laboratory-orders' ? view : 'laboratory';
      this.initDataForCurrentView();
    });
    this.route.queryParamMap.subscribe((p) => {
      const idRaw = p.get('orderId');
      const id = idRaw ? Number(idRaw) : null;
      this.selectedLabOrderId = id && !Number.isNaN(id) ? id : null;
    });
  }

  private initDataForCurrentView(): void {
    this.loadPatients();
    const isPureLaboratory =
      this.auth.hasRole('Laboratory') &&
      !this.auth.hasRole('SuperAdmin') &&
      !this.auth.hasRole('HospitalAdmin') &&
      !this.auth.hasRole('Reception') &&
      !this.auth.hasRole('Manager') &&
      !this.auth.hasRole('Doctor') &&
      !this.auth.hasRole('Nurse');
    if (!isPureLaboratory) this.loadDoctors();
    else this.doctors = [];

    this.loadCollectors();
    this.loadTests();
    this.loadLabOrders();
  }

  private loadPatients(): void {
    this.patientsService
      .getPatients({ pageNumber: 1, pageSize: 500, sortBy: 'fullName', sortDesc: false, search: null })
      .subscribe((r: any) => (this.patients = r.items ?? r.Items ?? []));
  }

  searchPatientsForLabOrder(): void {
    const term = this.patientSearchTerm.trim();
    if (term.length < 2) {
      this.patientSearchResults = [];
      return;
    }
    this.loadingPatientSearch = true;
    this.patientsService
      .getPatients({ pageNumber: 1, pageSize: 20, sortBy: 'fullName', sortDesc: false, search: term })
      .subscribe({
        next: (r) => {
          this.loadingPatientSearch = false;
          this.patientSearchResults = r.items ?? [];
        },
        error: () => {
          this.loadingPatientSearch = false;
          this.patientSearchResults = [];
        },
      });
  }

  selectPatientForLabOrder(p: PatientDto): void {
    this.selectedPatient = p;
    this.patientSearchTerm = `${p.fullName} - ${p.medicalRecordNumber}`;
    this.patientSearchResults = [];
    this.labOrderForm.patchValue({
      patientId: p.id,
      visitId: null,
      referringDoctorId: null,
    });
    this.loadVisitsForLaboratory(p.id);
  }

  clearSelectedPatientForLabOrder(): void {
    this.selectedPatient = null;
    this.patientSearchTerm = '';
    this.patientSearchResults = [];
    this.labVisitSearchResults = [];
    this.labOrderForm.patchValue({
      patientId: null,
      visitId: null,
      referringDoctorId: null,
    });
  }

  private loadVisitsForLaboratory(patientId: number): void {
    this.visitsService
      .getVisits({ patientId, doctorId: null, from: null, to: null, page: 1, pageSize: 200, sortBy: 'VisitDate', sortDescending: true })
      .subscribe({
        next: (res) => {
          this.labVisitSearchResults = res.items ?? [];
          if (this.labVisitSearchResults.length > 0) {
            const firstVisit = this.labVisitSearchResults[0];
            this.labOrderForm.patchValue({ visitId: firstVisit.id });
            this.onLabVisitChanged();
          }
        },
        error: () => {
          this.labVisitSearchResults = [];
        },
      });
  }

  private loadDoctors(): void {
    this.doctorsService
      .getDoctors({ pageNumber: 1, pageSize: 500, sortBy: 'fullName', sortDesc: false, search: null, departmentId: null, isActive: true })
      .subscribe({
        next: (r: any) => (this.doctors = r.items ?? r.Items ?? []),
        error: () => (this.doctors = []),
      });
  }

  private loadCollectors(): void {
    this.diagnostics.getLaboratoryCollectors().subscribe({
      next: (list) => (this.collectors = list),
      error: () => (this.collectors = []),
    });
  }

  loadTests(): void {
    this.loadingTests = true;
    this.diagnostics.getTests({ isActive: null, type: 1 }).subscribe({
      next: (list) => {
        this.loadingTests = false;
        this.tests = list.filter((t) => Number(t.type) === 1 || String(t.type) === 'Lab');
      },
      error: () => (this.loadingTests = false),
    });
  }

  saveTest(): void {
    if (this.testForm.invalid) {
      this.testForm.markAllAsTouched();
      return;
    }
    const v = this.testForm.value;
    this.diagnostics
      .saveTest({
        id: v.id,
        code: (v.code || '').trim(),
        name: (v.name || '').trim(),
        type: 1,
        price: Number(v.price),
        isActive: !!v.isActive,
      })
      .subscribe(() => {
        this.testForm.reset({ id: null, code: '', name: '', type: 1, price: 0, isActive: true });
        this.loadTests();
      });
  }

  editTest(t: DiagnosticTestDto): void {
    this.tab = 'tests';
    this.testForm.reset({
      id: t.id,
      code: t.code,
      name: t.name,
      type: 1,
      price: t.price,
      isActive: t.isActive,
    });
  }

  getVisitOptionLabel(v: VisitListDto): string {
    return `#${v.id} - ${new Date(v.visitDate).toLocaleString()} - ${this.getDoctorDisplayName(v.doctorId)}`;
  }

  getPatientDisplayName(patientId: number): string {
    const p = this.patients.find((x) => x.id === patientId);
    return p?.fullName ?? `Patient #${patientId}`;
  }

  getDoctorDisplayName(staffMemberId: number | null | undefined): string {
    if (!staffMemberId) return 'No doctor';
    const d = this.doctors.find((x) => x.staffMemberId === staffMemberId);
    return d?.fullName ?? `Doctor #${staffMemberId}`;
  }

  getReferringDoctorOptions(): Array<{ id: number; name: string }> {
    const map = new Map<number, string>();
    for (const d of this.doctors) {
      map.set(d.staffMemberId, d.fullName);
    }
    for (const c of this.collectors) {
      if (!map.has(c.staffMemberId)) {
        map.set(c.staffMemberId, c.fullName);
      }
    }

    // Ensure doctor from selected visit is always selectable even if doctors list is empty for current role.
    const visitId = this.labOrderForm.value.visitId ? Number(this.labOrderForm.value.visitId) : null;
    const selectedVisit = visitId ? this.labVisitSearchResults.find((v) => v.id === visitId) : null;
    const visitDoctorId = selectedVisit?.doctorId ? Number(selectedVisit.doctorId) : null;
    if (visitDoctorId && !map.has(visitDoctorId)) {
      map.set(visitDoctorId, `Doctor #${visitDoctorId}`);
    }

    return Array.from(map.entries()).map(([id, name]) => ({ id, name }));
  }

  getLabStatusLabel(status: number | string): string {
    const raw = typeof status === 'number' ? status : String(status).toLowerCase();
    if (raw === 1 || raw === 'ordered') return 'Ordered';
    if (raw === 2 || raw === 'paid') return 'Paid';
    if (raw === 3 || raw === 'samplecollected') return 'Sample collected';
    if (raw === 4 || raw === 'inprocess') return 'In process';
    if (raw === 5 || raw === 'completed') return 'Completed';
    if (raw === 6 || raw === 'validated') return 'Validated';
    if (raw === 7 || raw === 'delivered') return 'Delivered';
    if (raw === 8 || raw === 'cancelled') return 'Cancelled';
    if (raw === 9 || raw === 'retest') return 'Re-test';
    return String(status);
  }

  getResultFlagLabel(flag: number | string): string {
    const raw = typeof flag === 'number' ? flag : String(flag).toLowerCase();
    if (raw === 1 || raw === 'normal') return 'Normal';
    if (raw === 2 || raw === 'high') return 'High';
    if (raw === 3 || raw === 'low') return 'Low';
    if (raw === 4 || raw === 'critical') return 'Critical';
    return String(flag);
  }

  onLabVisitChanged(): void {
    const visitId = this.labOrderForm.value.visitId ? Number(this.labOrderForm.value.visitId) : null;
    if (!visitId) return;
    const selectedVisit = this.labVisitSearchResults.find((v) => v.id === visitId);
    const visitDoctorId = selectedVisit?.doctorId ? Number(selectedVisit.doctorId) : null;
    if (visitDoctorId) {
      this.labOrderForm.patchValue({ referringDoctorId: visitDoctorId });
    }
  }

  get sampleCollectors(): { staffMemberId: number; fullName: string }[] {
    if (this.collectors.length > 0) {
      return this.collectors.map((c) => ({ staffMemberId: c.staffMemberId, fullName: c.fullName }));
    }
    return this.doctors.map((d) => ({ staffMemberId: d.staffMemberId, fullName: d.fullName }));
  }

  loadLabOrders(): void {
    this.diagnostics
      .getLaboratoryOrders({ patientId: null, visitId: null, doctorId: null, status: null, from: null, to: null, page: 1, pageSize: 200 })
      .subscribe({
        next: (r) => {
          this.labOrders = r.items ?? [];
          if (this.selectedLabOrderId) {
            const matched = this.labOrders.find((x) => x.id === this.selectedLabOrderId) ?? null;
            if (matched) this.selectLabOrder(matched);
            this.selectedLabOrderId = null;
          } else if (this.selectedLabOrder) {
            this.selectedLabOrder = this.labOrders.find((x) => x.id === this.selectedLabOrder!.id) ?? null;
          }
        },
      });
  }

  createLabOrder(): void {
    const v = this.labOrderForm.value;
    const testIds = (v.testIds ?? []).map((x) => Number(x)).filter((x) => !Number.isNaN(x));
    if (!v.patientId || testIds.length === 0) {
      this.labOrderForm.markAllAsTouched();
      return;
    }
    const visitId = v.visitId ? Number(v.visitId) : null;
    const selectedVisit = visitId ? this.labVisitSearchResults.find((x) => x.id === visitId) : null;
    const inferredDoctorId = selectedVisit?.doctorId ? Number(selectedVisit.doctorId) : null;
    const referringDoctorId = v.referringDoctorId ? Number(v.referringDoctorId) : inferredDoctorId;

    this.diagnostics.createLaboratoryOrder({
      patientId: Number(v.patientId),
      visitId,
      referringDoctorId,
      clinicalIndication: (v.clinicalIndication || '').trim() || null,
      priority: Number(v.priority || 2),
      items: testIds.map((id) => ({ diagnosticTestId: id })),
    }).subscribe({
      next: () => {
        this.labOrderForm.reset({ patientId: null, visitId: null, referringDoctorId: null, clinicalIndication: '', priority: 2, testIds: [] });
        this.clearSelectedPatientForLabOrder();
        this.loadLabOrders();
      },
    });
  }

  selectLabOrder(order: LaboratoryOrderDto): void {
    this.selectedLabOrder = order;
    const defaultEnteredBy = this.sampleCollectors[0]?.staffMemberId ?? order.referringDoctorId ?? this.doctors[0]?.staffMemberId ?? null;
    this.resultFormLab.reset({
      laboratoryOrderItemId: order.items[0]?.id ?? null,
      laboratorySampleId: order.samples[0]?.id ?? null,
      value: '',
      unit: '',
      referenceRange: '',
      flag: 1,
      enteredById: defaultEnteredBy,
    });
    this.validateLabForm.reset({ validatedById: this.sampleCollectors[0]?.staffMemberId ?? this.doctors[0]?.staffMemberId ?? null });
  }

  private statusCode(order: LaboratoryOrderDto | null): number {
    if (!order) return 0;
    const raw = order.status as unknown;
    if (typeof raw === 'number') return raw;
    const key = String(raw).toLowerCase();
    switch (key) {
      case 'ordered': return 1;
      case 'paid': return 2;
      case 'samplecollected': return 3;
      case 'inprocess': return 4;
      case 'completed': return 5;
      case 'validated': return 6;
      case 'delivered': return 7;
      case 'retest': return 9;
      default: return 0;
    }
  }

  get currentStatusCode(): number {
    return this.statusCode(this.selectedLabOrder);
  }

  get canMarkPaidStep(): boolean {
    return this.currentStatusCode === 1;
  }

  get canCollectSampleStep(): boolean {
    return this.currentStatusCode === 1 || this.currentStatusCode === 2;
  }

  get canStartProcessingStep(): boolean {
    return this.currentStatusCode === 3 || this.currentStatusCode === 9;
  }

  get canEnterResultsStep(): boolean {
    return this.currentStatusCode === 3 || this.currentStatusCode === 4 || this.currentStatusCode === 5 || this.currentStatusCode === 9;
  }

  get canValidateStep(): boolean {
    return this.currentStatusCode === 5;
  }

  get canDeliverStep(): boolean {
    return this.currentStatusCode === 6;
  }

  get completedStepIndex(): number {
    const code = this.currentStatusCode;
    if (code <= 0) return 0;
    if (code >= 7) return 7;
    return code;
  }

  getMissingResultItemNames(order: LaboratoryOrderDto | null): string[] {
    if (!order) return [];
    const itemIdsWithResults = new Set(order.results.map((r) => r.laboratoryOrderItemId));
    return order.items
      .filter((i) => !itemIdsWithResults.has(i.id))
      .map((i) => i.testName);
  }

  getResultTestName(order: LaboratoryOrderDto, orderItemId: number): string {
    return order.items.find((i) => i.id === orderItemId)?.testName ?? String(orderItemId);
  }

  get canValidateCurrentOrder(): boolean {
    if (!this.selectedLabOrder) return false;
    return this.getMissingResultItemNames(this.selectedLabOrder).length === 0;
  }

  selectLabOrderById(id: number | null): void {
    if (id == null) {
      this.selectedLabOrder = null;
      return;
    }
    const order = this.labOrders.find((o) => o.id === Number(id));
    if (order) this.selectLabOrder(order);
  }

  markLabPaid(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canMarkPaidStep) return;
    const method = (this.markPaidForm.value.paymentMethod || '').toString();
    this.diagnostics.markLaboratoryOrderPaid(this.selectedLabOrder.id, { paymentMethod: method || null }).subscribe({ next: () => this.loadLabOrders() });
  }

  collectSample(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canCollectSampleStep) return;
    if (this.sampleForm.invalid) {
      this.sampleForm.markAllAsTouched();
      return;
    }
    const v = this.sampleForm.value;
    this.diagnostics.createLaboratorySample(this.selectedLabOrder.id, {
      sampleType: String(v.sampleType),
      collectedAt: (v.collectedAt as string) || null,
      collectedById: Number(v.collectedById),
      sampleBarcode: String(v.sampleBarcode),
    }).subscribe({
      next: () => {
        this.sampleForm.reset({ sampleType: 'Blood', collectedAt: '', collectedById: null, sampleBarcode: '' });
        this.loadLabOrders();
      },
    });
  }

  startLabProcessing(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canStartProcessingStep) return;
    this.diagnostics.startLaboratoryProcessing(this.selectedLabOrder.id).subscribe({ next: () => this.loadLabOrders() });
  }

  addLabResult(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canEnterResultsStep) return;
    if (this.resultFormLab.invalid) {
      this.resultFormLab.markAllAsTouched();
      return;
    }
    const v = this.resultFormLab.value;
    this.diagnostics.addLaboratoryResult(this.selectedLabOrder.id, {
      laboratoryOrderItemId: Number(v.laboratoryOrderItemId),
      laboratorySampleId: Number(v.laboratorySampleId),
      value: String(v.value),
      unit: (v.unit as string) || null,
      referenceRange: (v.referenceRange as string) || null,
      flag: Number(v.flag ?? 1),
      enteredById: Number(v.enteredById),
    }).subscribe({ next: () => this.loadLabOrders() });
  }

  validateLab(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canValidateStep) return;
    if (!this.canValidateCurrentOrder) {
      alert(`Cannot validate yet. Missing results for: ${this.getMissingResultItemNames(this.selectedLabOrder).join(', ')}`);
      return;
    }
    if (this.validateLabForm.invalid) {
      this.validateLabForm.markAllAsTouched();
      return;
    }
    this.diagnostics.validateLaboratoryResults(this.selectedLabOrder.id, { validatedById: Number(this.validateLabForm.value.validatedById) })
      .subscribe({ next: () => this.loadLabOrders() });
  }

  deliverLab(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canDeliverStep) return;
    this.diagnostics.deliverLaboratoryOrder(this.selectedLabOrder.id).subscribe({ next: () => this.loadLabOrders() });
  }

  cancelLab(): void {
    if (!this.selectedLabOrder) return;
    this.diagnostics.cancelLaboratoryOrder(this.selectedLabOrder.id).subscribe({ next: () => this.loadLabOrders() });
  }

  retestLab(): void {
    if (!this.selectedLabOrder) return;
    if (!this.canRetestCurrentOrder) return;
    this.diagnostics.retestLaboratoryOrder(this.selectedLabOrder.id).subscribe({ next: () => this.loadLabOrders() });
  }

  canRetestOrder(order: LaboratoryOrderDto): boolean {
    const code = this.statusCode(order);
    return code === 5 || code === 6 || code === 7;
  }

  get canRetestCurrentOrder(): boolean {
    if (!this.selectedLabOrder) return false;
    return this.canRetestOrder(this.selectedLabOrder);
  }

  loadLabPatientHistory(patientId: number): void {
    this.selectedHistoryPatientId = patientId;
    this.selectedHistoryPatientName = this.getPatientDisplayName(patientId);
    this.diagnostics.getPatientLaboratoryHistory(patientId).subscribe({ next: (rows) => (this.patientLabHistory = rows) });
  }

  openHistoryFromOrder(order: LaboratoryOrderDto): void {
    this.loadLabPatientHistory(order.patientId);
  }

  clearHistoryPanel(): void {
    this.selectedHistoryPatientId = null;
    this.selectedHistoryPatientName = '';
    this.patientLabHistory = [];
  }

  printLabReport(order: LaboratoryOrderDto): void {
    const rows = order.results
      .map((r) => {
        const item = order.items.find((i) => i.id === r.laboratoryOrderItemId);
        return `<tr><td>${item?.testName ?? r.laboratoryOrderItemId}</td><td>${r.value}</td><td>${r.unit ?? '-'}</td><td>${r.referenceRange ?? '-'}</td><td>${r.flag}</td></tr>`;
      })
      .join('');

    const win = window.open('', '_blank', 'width=900,height=700');
    if (!win) return;
    win.document.write(`
      <html><head><title>Laboratory Report #${order.id}</title>
      <style>body { font-family: Arial, sans-serif; margin: 24px; color: #1f2937; } table { width: 100%; border-collapse: collapse; margin-top: 12px; } th, td { border: 1px solid #d1d5db; padding: 8px; text-align: left; }</style>
      </head><body><h1>Laboratory Report</h1>
      <div>Order ID: #${order.id}<br/>Patient ID: ${order.patientId}<br/>Date: ${new Date(order.orderedAt).toLocaleString()}<br/>Status: ${order.status}</div>
      <table><thead><tr><th>Test</th><th>Value</th><th>Unit</th><th>Reference</th><th>Flag</th></tr></thead>
      <tbody>${rows || '<tr><td colspan="5">No results entered.</td></tr>'}</tbody></table>
      </body></html>`);
    win.document.close();
    win.focus();
    setTimeout(() => win.print(), 200);
  }

  get canValidateLabResults(): boolean {
    return this.auth.hasRole('Laboratory') || this.auth.hasRole('Laboratori') || this.auth.hasRole('Manager') || this.auth.hasRole('SuperAdmin') || this.auth.hasRole('HospitalAdmin');
  }

  get canDeliverLabResults(): boolean {
    return this.auth.hasRole('Laboratory') || this.auth.hasRole('Laboratori') || this.auth.hasRole('Reception') || this.auth.hasRole('Doctor') || this.auth.hasRole('Manager') || this.auth.hasRole('SuperAdmin') || this.auth.hasRole('HospitalAdmin');
  }

  get canCancelLabOrder(): boolean {
    return this.auth.hasRole('Doctor') || this.auth.hasRole('Reception') || this.auth.hasRole('Manager') || this.auth.hasRole('SuperAdmin') || this.auth.hasRole('HospitalAdmin');
  }

  get canRetestLabOrder(): boolean {
    return this.auth.hasRole('Laboratory') || this.auth.hasRole('Laboratori') || this.auth.hasRole('Manager') || this.auth.hasRole('SuperAdmin') || this.auth.hasRole('HospitalAdmin');
  }

  get canOpenBillingFromLab(): boolean {
    return this.auth.hasRole('Reception') || this.auth.hasRole('Finance') || this.auth.hasRole('Manager') || this.auth.hasRole('SuperAdmin') || this.auth.hasRole('HospitalAdmin');
  }
}
