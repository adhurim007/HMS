export interface MenuDto {
  id: number;
  name: string;
  menuKey: string;
  url?: string | null;
  parentId?: number | null;
  displayOrder: number;
  icon?: string | null;
  isActive: boolean;
}

export interface MenuForRoleDto extends MenuDto {
  isAssigned: boolean;
}

export interface RoleDto {
  id: number;
  name: string;
}

