import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { MenuDto, MenuForRoleDto, RoleDto } from './menus.api';

@Injectable({ providedIn: 'root' })
export class MenusService {
  constructor(private readonly api: ApiService) {}

  getMenus(isActive?: boolean | null): Observable<MenuDto[]> {
    const params: Record<string, string> = {};
    if (isActive != null) {
      params['isActive'] = String(isActive);
    }
    return this.api
      .get<{ success: boolean; data: MenuDto[] }>('Menus', params)
      .pipe(map((x) => x.data));
  }

  createMenu(payload: {
    name: string;
    menuKey: string;
    url?: string | null;
    parentId?: number | null;
    displayOrder: number;
    icon?: string | null;
    isActive: boolean;
  }): Observable<MenuDto> {
    return this.api
      .post<{ success: boolean; data: MenuDto }>('Menus', payload)
      .pipe(map((x) => x.data));
  }

  updateMenu(
    id: number,
    payload: {
      name: string;
      menuKey: string;
      url?: string | null;
      parentId?: number | null;
      displayOrder: number;
      icon?: string | null;
      isActive: boolean;
    },
  ): Observable<MenuDto> {
    return this.api
      .put<{ success: boolean; data: MenuDto }>(`Menus/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => x.data));
  }

  deleteMenu(id: number): Observable<void> {
    return this.api
      .delete<{ success: boolean }>(`Menus/${id}`)
      .pipe(map(() => void 0));
  }

  getRoles(): Observable<RoleDto[]> {
    return this.api
      .get<{ success: boolean; data: RoleDto[] }>('Roles')
      .pipe(map((x) => x.data));
  }

  getMenusForRole(roleId: number): Observable<MenuForRoleDto[]> {
    return this.api
      .get<{ success: boolean; data: MenuForRoleDto[] }>(
        `Menus/for-role/${roleId}`,
      )
      .pipe(map((x) => x.data));
  }

  /** Menus for the current user's roles. Use this for sidebar navigation (any role). */
  getMyMenus(): Observable<MenuDto[]> {
    return this.api
      .get<{ success: boolean; data?: MenuDto[] }>('Menus/my-menus')
      .pipe(map((x) => x.data ?? []));
  }

  updateMenusForRole(roleId: number, menuIds: number[]): Observable<void> {
    return this.api
      .put<{ success: boolean }>(`Menus/for-role/${roleId}`, {
        menuIds,
      })
      .pipe(map(() => void 0));
  }
}

