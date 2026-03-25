import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MenusService } from './menus.service';
import { MenuDto, MenuForRoleDto, RoleDto } from './menus.api';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';

interface MenuTreeNode extends MenuDto {
  children: MenuTreeNode[];
}

@Component({
  selector: 'app-menus-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterLink,
    TranslatePipe,
  ],
  templateUrl: './menus.page.html',
  styleUrl: './menus.page.scss',
})
export class MenusPage implements OnInit {
  menus: MenuDto[] = [];
  menuTree: MenuTreeNode[] = [];
  loadingMenus = false;
  savingMenu = false;

  editingId: number | null = null;

  roles: RoleDto[] = [];
  selectedRoleId: number | null = null;
  roleMenus: MenuForRoleDto[] = [];
  loadingRoles = false;
  loadingRoleMenus = false;
  savingRoleMenus = false;

  readonly menuForm = this.fb.group({
    name: ['', Validators.required],
    menuKey: ['', Validators.required],
    url: [''],
    parentId: [null as number | null],
    displayOrder: [0, Validators.required],
    icon: [''],
    isActive: [true],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly menusService: MenusService,
  ) {}

  ngOnInit(): void {
    this.loadMenus();
    this.loadRoles();
  }

  private buildTree(): void {
    const byId = new Map<number, MenuTreeNode>();
    const roots: MenuTreeNode[] = [];

    for (const m of this.menus.sort(
      (a, b) => a.displayOrder - b.displayOrder,
    )) {
      byId.set(m.id, { ...m, children: [] });
    }

    for (const node of byId.values()) {
      if (node.parentId && byId.has(node.parentId)) {
        byId.get(node.parentId)!.children.push(node);
      } else {
        roots.push(node);
      }
    }

    this.menuTree = roots;
  }

  loadMenus(): void {
    this.loadingMenus = true;
    this.menusService.getMenus().subscribe({
      next: (list) => {
        this.loadingMenus = false;
        this.menus = list;
        this.buildTree();
      },
      error: () => {
        this.loadingMenus = false;
      },
    });
  }

  openCreate(): void {
    this.editingId = null;
    this.menuForm.reset({
      name: '',
      menuKey: '',
      url: '',
      parentId: null,
      displayOrder: 0,
      icon: '',
      isActive: true,
    });
  }

  openEdit(menu: MenuDto): void {
    this.editingId = menu.id;
    this.menuForm.reset({
      name: menu.name,
      menuKey: menu.menuKey,
      url: menu.url ?? '',
      parentId: menu.parentId ?? null,
      displayOrder: menu.displayOrder,
      icon: menu.icon ?? '',
      isActive: menu.isActive,
    });
  }

  deleteMenu(menu: MenuDto): void {
    if (!confirm(`Delete menu "${menu.name}"?`)) return;
    this.menusService.deleteMenu(menu.id).subscribe({
      next: () => this.loadMenus(),
    });
  }

  submitMenu(): void {
    if (this.menuForm.invalid) {
      this.menuForm.markAllAsTouched();
      return;
    }
    const value = this.menuForm.value;
    const payload = {
      name: value.name!.trim(),
      menuKey: value.menuKey!.trim(),
      url: value.url?.trim() || null,
      parentId: value.parentId ?? null,
      displayOrder: Number(value.displayOrder ?? 0),
      icon: value.icon?.trim() || null,
      isActive: !!value.isActive,
    };
    this.savingMenu = true;
    const obs =
      this.editingId == null
        ? this.menusService.createMenu(payload)
        : this.menusService.updateMenu(this.editingId, payload);
    obs.subscribe({
      next: () => {
        this.savingMenu = false;
        this.openCreate();
        this.loadMenus();
      },
      error: () => {
        this.savingMenu = false;
      },
    });
  }

  // --- Role / menu assignment ---

  loadRoles(): void {
    this.loadingRoles = true;
    this.menusService.getRoles().subscribe({
      next: (list) => {
        this.loadingRoles = false;
        this.roles = list;
        if (!this.selectedRoleId && list.length > 0) {
          this.selectRole(list[0].id);
        }
      },
      error: () => {
        this.loadingRoles = false;
      },
    });
  }

  selectRole(roleId: number): void {
    this.selectedRoleId = roleId;
    this.loadRoleMenus();
  }

  loadRoleMenus(): void {
    if (!this.selectedRoleId) {
      this.roleMenus = [];
      return;
    }
    this.loadingRoleMenus = true;
    this.menusService.getMenusForRole(this.selectedRoleId).subscribe({
      next: (list) => {
        this.loadingRoleMenus = false;
        this.roleMenus = list;
      },
      error: () => {
        this.loadingRoleMenus = false;
      },
    });
  }

  toggleMenuForRole(menu: MenuForRoleDto, checked: boolean): void {
    menu.isAssigned = checked;
  }

  saveRoleMenus(): void {
    if (!this.selectedRoleId) return;
    this.savingRoleMenus = true;
    const menuIds = this.roleMenus.filter((m) => m.isAssigned).map((m) => m.id);
    this.menusService
      .updateMenusForRole(this.selectedRoleId, menuIds)
      .subscribe({
        next: () => {
          this.savingRoleMenus = false;
        },
        error: () => {
          this.savingRoleMenus = false;
        },
      });
  }
}


