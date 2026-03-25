export interface DoctorDto {
  staffMemberId: number;
  fullName: string;
  specialty?: string | null;
  licenseNumber?: string | null;
  departmentId?: number | null;
  phone?: string | null;
  email?: string | null;
  isActive: boolean;
}

export interface DoctorVisitSettingsDto {
  id: number;
  staffMemberId: number;
  minVisitDurationMinutes: number;
}

export interface DoctorWeeklyScheduleDayDto {
  dayOfWeek: number; // 0=Sunday ... 6=Saturday
  isWorking: boolean;
  startTime?: string | null;
  endTime?: string | null;
}

export interface DoctorWeeklyScheduleDto {
  staffMemberId: number;
  slotDurationMinutes: number;
  days: DoctorWeeklyScheduleDayDto[];
}

export interface DoctorCalendarSlotDto {
  slotStart: string;
  slotEnd: string;
  isAvailable: boolean;
  appointmentId?: number | null;
  patientId?: number | null;
  appointmentStatus?: string | null;
}

export interface DoctorCalendarDaySlotsDto {
  date: string;
  slots: DoctorCalendarSlotDto[];
}

export interface GetDoctorCalendarSlotsDto {
  staffMemberId: number;
  slotDurationMinutes: number;
  days: DoctorCalendarDaySlotsDto[];
}

export interface DoctorMeDto {
  staffMemberId: number;
  fullName: string;
  departmentId?: number | null;
  departmentName?: string | null;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

