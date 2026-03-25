export interface PatientDto {
  id: number;
  medicalRecordNumber: string;
  fullName: string;
  dateOfBirth?: string;
  gender: 'Unknown' | 'Male' | 'Female' | number;
  phone?: string;
  email?: string;
  address?: string;
  bloodGroup?: string | null;
  chronicConditions?: string | null;
  allergies?: string | null;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

