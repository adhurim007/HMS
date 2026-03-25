export type InvoiceStatus = 'Draft' | 'Unpaid' | 'PartiallyPaid' | 'Paid' | 'Cancelled' | number;

export interface UnbilledVisitServiceDto {
  id: number;
  visitId: number;
  visitDate: string;
  doctorName?: string | null;
  serviceItemId: number;
  serviceName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  notes?: string | null;
}

export interface UnbilledPrescriptionItemDto {
  id: number;
  prescriptionId: number;
  visitId: number;
  visitDate: string;
  doctorName?: string | null;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface InvoiceLineInput {
  visitServiceId?: number | null;
  laboratoryOrderItemId?: number | null;
  serviceItemId?: number | null;
  productId?: number | null;
  prescriptionItemId?: number | null;
  description: string;
  unitPrice: number;
  quantity: number;
}

export interface UnbilledLaboratoryItemDto {
  id: number;
  laboratoryOrderId: number;
  orderedAt: string;
  doctorName?: string | null;
  testName: string;
  unitPrice: number;
  lineTotal: number;
}

export interface InvoiceItemDto {
  id: number;
  serviceItemId?: number | null;
  productId?: number | null;
  laboratoryOrderItemId?: number | null;
  prescriptionItemId?: number | null;
  description: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface InvoiceDto {
  id: number;
  invoiceNumber: string;
  patientId: number;
  invoiceDate: string;
  totalAmount: number;
  paidAmount: number;
  status: InvoiceStatus;
  items: InvoiceItemDto[];
}

export interface InvoiceListDto {
  id: number;
  invoiceNumber: string;
  patientId: number;
  invoiceDate: string;
  totalAmount: number;
  paidAmount: number;
  status: InvoiceStatus;
}

export interface PaymentDto {
  id: number;
  invoiceId: number;
  installmentItemId?: number | null;
  paymentDate: string;
  amount: number;
  method?: string | null;
  reference?: string | null;
}

export type InstallmentPlanStatus = 'Active' | 'Completed' | 'Cancelled' | number;
export type InstallmentItemStatus = 'Pending' | 'PartiallyPaid' | 'Paid' | 'Overdue' | number;

export interface InstallmentItemDto {
  id: number;
  dueDate: string;
  amount: number;
  paidAmount: number;
  remainingAmount: number;
  status: InstallmentItemStatus;
}

export interface InstallmentPlanDto {
  id: number;
  invoiceId: number;
  patientId: number;
  startDate: string;
  totalAmount: number;
  status: InstallmentPlanStatus;
  items: InstallmentItemDto[];
}

export interface PaymentHistoryRowDto {
  paymentId: number;
  invoiceId: number;
  invoiceNumber: string;
  installmentItemId?: number | null;
  paymentDate: string;
  amount: number;
  method?: string | null;
  reference?: string | null;
}

export interface PatientPaymentHistoryDto {
  patientId: number;
  installmentPlans: InstallmentPlanDto[];
  payments: PaymentHistoryRowDto[];
}

export interface ServiceItemDto {
  id: number;
  code: string;
  name: string;
  price: number;
  isActive: boolean;
}

export interface ServiceItemListDto {
  id: number;
  code: string;
  name: string;
  price: number;
  isActive: boolean;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}
