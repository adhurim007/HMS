export type AppointmentStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Completed'
  | 'Cancelled'
  | 'NoShow'
  | number;

export interface AppointmentDto {
  id: number;
  patientId: number;
  doctorId?: number | null;
  departmentId?: number | null;
  scheduledStart: string;
  scheduledEnd?: string | null;
  status: AppointmentStatus;
  reason?: string | null;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

