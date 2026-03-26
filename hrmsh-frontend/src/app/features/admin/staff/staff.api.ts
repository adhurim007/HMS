export interface StaffMemberDto {
  id: number;
  fullName: string;
  staffType: number;
  phone?: string | null;
  email?: string | null;
  userId?: number | null;
  departmentId?: number | null;
  facilityIds: number[];
  isActive: boolean;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

export interface StaffTypeOption {
  value: number;
  label: string;
}
