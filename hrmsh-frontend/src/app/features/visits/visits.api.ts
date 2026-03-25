export interface VisitDto {
  id: number;
  patientId: number;
  doctorId?: number | null;
  hasPrescription: boolean;
  visitDate: string;
  chiefComplaint?: string | null;
  notes?: string | null;
  diagnosis?: string | null;
}

export interface VisitListDto {
  id: number;
  patientId: number;
  doctorId?: number | null;
  hasPrescription: boolean;
  visitDate: string;
  chiefComplaint?: string | null;
  diagnosis?: string | null;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}
