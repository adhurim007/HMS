export interface UserListDto {
  id: number;
  email: string;
  roles: string[];
  lockoutEnd?: string | null;
}

export interface PagedUsersResponse {
  success: boolean;
  items: UserListDto[];
  totalCount: number;
}
