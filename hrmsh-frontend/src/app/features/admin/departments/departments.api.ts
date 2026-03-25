export interface DepartmentDto {
  id: number;
  name: string;
  code?: string;
  facilityId: number;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

