export type DiagnosticType = 'Lab' | number;
export type LabPriority = 'Low' | 'Normal' | 'High' | 'Urgent' | number;

export interface DiagnosticTestDto {
  id: number;
  code: string;
  name: string;
  type: DiagnosticType;
  price: number;
  isActive: boolean;
}

export type LaboratoryOrderStatus =
  | 'Ordered'
  | 'Paid'
  | 'SampleCollected'
  | 'InProcess'
  | 'Completed'
  | 'Validated'
  | 'Delivered'
  | 'Cancelled'
  | 'ReTest'
  | number;

export type LaboratoryResultFlag = 'Normal' | 'High' | 'Low' | 'Critical' | number;

export interface LaboratoryOrderItemDto {
  id: number;
  diagnosticTestId: number;
  testName: string;
  price: number;
  notes?: string | null;
}

export interface LaboratorySampleDto {
  id: number;
  sampleType: string;
  collectedAt: string;
  collectedById: number;
  sampleBarcode: string;
}

export interface LaboratoryResultDto {
  id: number;
  laboratoryOrderItemId: number;
  laboratorySampleId: number;
  value: string;
  unit?: string | null;
  referenceRange?: string | null;
  flag: LaboratoryResultFlag;
  enteredById: number;
  enteredAt: string;
}

export interface LaboratoryOrderDto {
  id: number;
  patientId: number;
  visitId?: number | null;
  referringDoctorId?: number | null;
  orderedAt: string;
  priority: LabPriority;
  clinicalIndication?: string | null;
  totalAmount: number;
  isPaid: boolean;
  paidAt?: string | null;
  paymentMethod?: string | null;
  status: LaboratoryOrderStatus;
  validatedById?: number | null;
  validatedAt?: string | null;
  deliveredAt?: string | null;
  items: LaboratoryOrderItemDto[];
  samples: LaboratorySampleDto[];
  results: LaboratoryResultDto[];
}

export interface PatientLabHistoryRowDto {
  laboratoryOrderId: number;
  orderedAt: string;
  testName: string;
  value: string;
  unit?: string | null;
  referenceRange?: string | null;
  flag: LaboratoryResultFlag;
  status: LaboratoryOrderStatus;
}

export interface LaboratoryCollectorDto {
  staffMemberId: number;
  fullName: string;
  staffType: number | string;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}
