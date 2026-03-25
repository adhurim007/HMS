import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { BillingService } from '../billing/billing.service';
import { PatientsService } from './patients.service';
import { PatientDto } from './patients.api';
import {
  InstallmentPlanDto,
  InvoiceListDto,
  InstallmentItemDto,
  PaymentHistoryRowDto,
} from '../billing/billing.api';

@Component({
  selector: 'app-patient-billing-page',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './patient-billing.page.html',
  styleUrl: './patient-billing.page.scss',
})
export class PatientBillingPage implements OnInit {
  patientId!: number;
  patient: PatientDto | null = null;

  invoices: InvoiceListDto[] = [];
  totalInvoicesAmount = 0;
  totalPaidAmount = 0;
  outstandingAmount = 0;

  loading = false;
  creatingPlanForInvoiceId: number | null = null;
  installmentPlans: InstallmentPlanDto[] = [];
  paymentHistoryRows: PaymentHistoryRowDto[] = [];
  selectedInstallmentItem: {
    planId: number;
    planInvoiceId: number;
    item: InstallmentItemDto;
  } | null = null;

  selectedInvoice: InvoiceListDto | null = null;
  readonly paymentMethodOptions: string[] = ['Cash', 'Card', 'Bank Transfer', 'Insurance', 'POS'];
  paymentForm = this.fb.group({
    amount: [0, [Validators.required, Validators.min(0.01)]],
    method: ['Cash'],
    reference: [''],
    paymentDate: [''],
  });
  submittingPayment = false;
  paymentError: string | null = null;
  installmentError: string | null = null;

  installmentForm = this.fb.group({
    installmentsCount: [3, [Validators.required, Validators.min(2), Validators.max(24)]],
    firstDueDate: [''],
  });
  installmentPaymentForm = this.fb.group({
    amount: [0, [Validators.required, Validators.min(0.01)]],
    method: ['Cash'],
    reference: [''],
    paymentDate: [''],
  });

  constructor(
    private readonly route: ActivatedRoute,
    private readonly billing: BillingService,
    private readonly patientsService: PatientsService,
    private readonly fb: FormBuilder,
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.patientId = Number(idParam);
    if (!this.patientId) {
      return;
    }
    this.loadPatient();
    this.loadInvoices();
    this.loadPaymentHistory();
  }

  loadPatient(): void {
    this.patientsService.getPatient(this.patientId).subscribe({
      next: (p) => (this.patient = p),
    });
  }

  loadInvoices(): void {
    this.loading = true;
    this.billing
      .getInvoices({
        patientId: this.patientId,
        status: null,
        from: null,
        to: null,
        page: 1,
        pageSize: 100,
        sortBy: 'InvoiceDate',
        sortDescending: true,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.invoices = res.items;
          this.computeTotals();
        },
        error: () => {
          this.loading = false;
          this.invoices = [];
          this.totalInvoicesAmount = 0;
          this.totalPaidAmount = 0;
          this.outstandingAmount = 0;
        },
      });
  }

  loadPaymentHistory(): void {
    this.billing.getPatientPaymentHistory(this.patientId).subscribe({
      next: (h) => {
        this.installmentPlans = h.installmentPlans ?? [];
        this.paymentHistoryRows = h.payments ?? [];
      },
      error: () => {
        this.installmentPlans = [];
        this.paymentHistoryRows = [];
      },
    });
  }

  computeTotals(): void {
    let total = 0;
    let paid = 0;
    for (const inv of this.invoices) {
      total += inv.totalAmount;
      paid += inv.paidAmount;
    }
    this.totalInvoicesAmount = total;
    this.totalPaidAmount = paid;
    this.outstandingAmount = total - paid;
  }

  getOutstanding(inv: InvoiceListDto): number {
    return inv.totalAmount - inv.paidAmount;
  }

  openPayment(inv: InvoiceListDto): void {
    const outstanding = this.getOutstanding(inv);
    this.selectedInvoice = inv;
    this.paymentError = null;
    const today = new Date().toISOString().substring(0, 10);
    this.paymentForm.reset({
      amount:
        outstanding > 0
          ? outstanding
          : inv.totalAmount,
      method: this.paymentMethodOptions[0],
      reference: '',
      paymentDate: today,
    });
  }

  cancelPayment(): void {
    this.selectedInvoice = null;
    this.paymentError = null;
    this.submittingPayment = false;
  }

  openInstallmentPlan(inv: InvoiceListDto): void {
    this.creatingPlanForInvoiceId = inv.id;
    this.installmentError = null;
    const today = new Date().toISOString().substring(0, 10);
    this.installmentForm.reset({
      installmentsCount: 3,
      firstDueDate: today,
    });
  }

  cancelInstallmentPlan(): void {
    this.creatingPlanForInvoiceId = null;
    this.installmentError = null;
  }

  openInstallmentPayment(plan: InstallmentPlanDto, item: InstallmentItemDto): void {
    if (item.remainingAmount <= 0) return;
    this.selectedInstallmentItem = { planId: plan.id, planInvoiceId: plan.invoiceId, item };
    this.paymentError = null;
    this.installmentError = null;
    const today = new Date().toISOString().substring(0, 10);
    this.installmentPaymentForm.reset({
      amount: item.remainingAmount,
      method: this.paymentMethodOptions[0],
      reference: '',
      paymentDate: today,
    });
  }

  cancelInstallmentPayment(): void {
    this.selectedInstallmentItem = null;
  }

  submitInstallmentPayment(): void {
    if (!this.selectedInstallmentItem) return;
    if (this.installmentPaymentForm.invalid) {
      this.installmentPaymentForm.markAllAsTouched();
      return;
    }
    const v = this.installmentPaymentForm.value;
    this.submittingPayment = true;
    this.installmentError = null;
    this.billing
      .addInstallmentPayment({
        installmentItemId: this.selectedInstallmentItem.item.id,
        amount: Number(v.amount),
        method: v.method || null,
        reference: v.reference || null,
        paymentDate: (v.paymentDate as string) || null,
      })
      .subscribe({
        next: () => {
          this.submittingPayment = false;
          this.selectedInstallmentItem = null;
          this.loadInvoices();
          this.loadPaymentHistory();
        },
        error: (err) => {
          this.submittingPayment = false;
          const body = err?.error;
          this.installmentError = body?.message || err?.message || 'Failed to record installment payment.';
        },
      });
  }

  isInstallmentOverdue(item: InstallmentItemDto): boolean {
    if (item.remainingAmount <= 0) return false;
    const due = new Date(item.dueDate);
    const today = new Date();
    const dueDate = new Date(due.getFullYear(), due.getMonth(), due.getDate());
    const nowDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    return dueDate < nowDate;
  }

  getOverdueInstallmentsCount(plan: InstallmentPlanDto): number {
    return (plan.items ?? []).filter((i) => this.isInstallmentOverdue(i)).length;
  }

  submitInstallmentPlan(): void {
    if (!this.creatingPlanForInvoiceId) return;
    if (this.installmentForm.invalid) {
      this.installmentForm.markAllAsTouched();
      return;
    }
    const inv = this.invoices.find((x) => x.id === this.creatingPlanForInvoiceId);
    if (!inv) return;
    const remaining = this.getOutstanding(inv);
    if (remaining <= 0) {
      this.installmentError = 'Invoice is already paid.';
      return;
    }

    const count = Number(this.installmentForm.value.installmentsCount || 0);
    const firstDueDate = (this.installmentForm.value.firstDueDate as string) || new Date().toISOString().substring(0, 10);
    const firstDate = new Date(firstDueDate);

    const base = Math.floor((remaining / count) * 100) / 100;
    const items: { dueDate: string; amount: number }[] = [];
    let sum = 0;
    for (let i = 0; i < count; i++) {
      const d = new Date(firstDate.getFullYear(), firstDate.getMonth() + i, firstDate.getDate());
      const amount = i === count - 1 ? Number((remaining - sum).toFixed(2)) : base;
      sum += amount;
      items.push({
        dueDate: d.toISOString(),
        amount,
      });
    }

    this.installmentError = null;
    this.billing
      .createInstallmentPlan({
        invoiceId: inv.id,
        startDate: new Date().toISOString(),
        items,
      })
      .subscribe({
        next: () => {
          this.creatingPlanForInvoiceId = null;
          this.loadInvoices();
          this.loadPaymentHistory();
        },
        error: (err) => {
          const body = err?.error;
          this.installmentError = body?.message || err?.message || 'Failed to create installment plan.';
        },
      });
  }

  submitPayment(): void {
    if (!this.selectedInvoice) return;
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    this.submittingPayment = true;
    this.paymentError = null;
    const v = this.paymentForm.value;
    this.billing
      .addPayment({
        invoiceId: this.selectedInvoice.id,
        amount: Number(v.amount),
        method: v.method || null,
        reference: v.reference || null,
        paymentDate: (v.paymentDate as string) || null,
      })
      .subscribe({
        next: () => {
          this.submittingPayment = false;
          this.selectedInvoice = null;
          this.loadInvoices();
          this.loadPaymentHistory();
        },
        error: (err) => {
          this.submittingPayment = false;
          const body = err.error;
          if (body?.message) {
            this.paymentError = body.message;
          } else if (body?.errors && typeof body.errors === 'object') {
            const msgs = Object.values(body.errors).flat();
            this.paymentError = Array.isArray(msgs)
              ? msgs.join(' ')
              : String(body.errors);
          } else {
            this.paymentError =
              err.message || 'Failed to record payment.';
          }
        },
      });
  }
}

