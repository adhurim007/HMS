export interface VisitServiceLineDto {
  id: number;
  serviceItemId: number;
  serviceName?: string | null;
  quantity: number;
  unitPrice: number;
  notes?: string | null;
  isBilled: boolean;
}

export interface VisitDto {
  id: number;
  facilityId?: number | null;
  patientId: number;
  doctorId?: number | null;
  hasPrescription: boolean;
  visitDate: string;
  visitFormTemplate: string;
  clinicalDataJson?: string | null;
  chiefComplaint?: string | null;
  notes?: string | null;
  diagnosis?: string | null;
  services?: VisitServiceLineDto[];
}

export interface VisitListDto {
  id: number;
  patientId: number;
  doctorId?: number | null;
  hasPrescription: boolean;
  visitDate: string;
  visitFormTemplate: string;
  chiefComplaint?: string | null;
  diagnosis?: string | null;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}
