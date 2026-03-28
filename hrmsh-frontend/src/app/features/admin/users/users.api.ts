export interface UserListDto {
  id: number;
  email: string;
  roles: string[];
  lockoutEnd?: string | null;
  hospitalId?: number | null;
  hospitalName?: string | null;
  facilityId?: number | null;
  facilityName?: string | null;
}

export interface PagedUsersResponse {
  success: boolean;
  items: UserListDto[];
  totalCount: number;
}
