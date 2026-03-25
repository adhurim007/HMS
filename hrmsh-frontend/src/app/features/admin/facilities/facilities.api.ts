export interface FacilityDto {
  id: number;
  name: string;
  code?: string;
  address?: string;
}

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

