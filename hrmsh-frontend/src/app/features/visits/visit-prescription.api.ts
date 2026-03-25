export interface PrescriptionItemDto {
  id: number;
  productId: number;
  productName: string;
  dosage?: string | null;
  frequency?: string | null;
  duration?: string | null;
  quantity: number;
  instructions?: string | null;
}

export interface PrescriptionDto {
  id: number;
  visitId: number;
  patientId: number;
  doctorId?: number | null;
  notes?: string | null;
  status: number;
  items: PrescriptionItemDto[];
}

export interface PrescriptionListItemDto {
  id: number;
  visitId: number;
  patientId: number;
  patientName: string;
  doctorId?: number | null;
  doctorName?: string | null;
  createdAt: string;
  status: number;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

