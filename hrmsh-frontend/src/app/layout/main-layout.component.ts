import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NgIf, NgForOf, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../core/services/auth.service';
import { I18nService } from '../core/i18n/i18n.service';
import { TranslatePipe } from '../core/i18n/translate.pipe';
import { MenusService } from '../features/admin/menus/menus.service';
import { MenuDto } from '../features/admin/menus/menus.api';
import { NotificationsService } from '../features/notifications/notifications.service';
import { NotificationDto } from '../features/notifications/notifications.api';
import { FacilitiesService } from '../features/admin/facilities/facilities.service';
import { FacilityDto } from '../features/admin/facilities/facilities.api';
import { FacilityContextService } from '../core/services/facility-context.service';

interface NavItem {
  id: number;
  name: string;
  menuKey: string;
  url?: string | null;
  icon?: string | null;
  children: NavItem[];
  expanded?: boolean;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgForOf,
    NgClass,
    FormsModule,
    TranslatePipe,
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent implements OnInit {
  year = new Date().getFullYear();
  userEmail: string | null = null;
  languages = [
    { code: 'en', label: 'EN' },
    { code: 'sq', label: 'AL' },
  ];

  navItems: NavItem[] = [];
  loadingMenus = false;

  notifications: NotificationDto[] = [];
  unreadCount = 0;
  notificationsLoading = false;
  notificationsOpen = false;
  facilities: FacilityDto[] = [];
  facilityInput = '';
  selectedFacilityId: number | null = null;

  constructor(
    private readonly auth: AuthService,
    public readonly i18n: I18nService,
    private readonly menusService: MenusService,
    private readonly notificationsService: NotificationsService,
    private readonly facilitiesService: FacilitiesService,
    private readonly facilityContext: FacilityContextService,
  ) {}

  ngOnInit(): void {
    document.documentElement.setAttribute('data-layout', 'vertical');
    document.documentElement.setAttribute('data-layout-mode', 'light');
    document.documentElement.setAttribute('data-layout-width', 'fluid');
    document.documentElement.setAttribute('data-sidebar-size', 'lg');
    this.userEmail = this.auth.getEmail();
    this.selectedFacilityId = this.facilityContext.getActiveFacilityId();
    if (this.isAuthenticated()) {
      this.loadFacilities();
      this.loadNavigationMenus();
      this.refreshUnreadCount();
    }
  }

  toggleNotificationsDropdown(): void {
    this.notificationsOpen = !this.notificationsOpen;
    if (this.notificationsOpen) {
      this.loadNotifications();
    }
  }

  loadNotifications(): void {
    this.notificationsLoading = true;
    this.notificationsService.getList().subscribe({
      next: (list) => {
        this.notificationsLoading = false;
        this.notifications = list;
        this.unreadCount = list.filter((n) => !n.isRead).length;
      },
      error: () => (this.notificationsLoading = false),
    });
  }

  refreshUnreadCount(): void {
    if (!this.isAuthenticated()) return;
    this.notificationsService.getUnreadCount().subscribe((c) => (this.unreadCount = c));
  }

  markNotificationRead(n: NotificationDto): void {
    if (n.isRead) return;
    this.notificationsService.markRead(n.type, n.key).subscribe({
      next: () => {
        n.isRead = true;
        this.unreadCount = Math.max(0, this.unreadCount - 1);
      },
    });
  }

  closeNotificationsDropdown(): void {
    this.notificationsOpen = false;
  }

  toggleMobileMenu(): void {
    document.body.classList.toggle('vertical-sidebar-enable');
    document.querySelector('.hamburger-icon')?.classList.toggle('open');
  }

  sidebarHide(): void {
    document.body.classList.remove('vertical-sidebar-enable');
  }

  logout(): void {
    this.auth.logout();
  }

  isAuthenticated(): boolean {
    return this.auth.isAuthenticated();
  }

  onFacilityChanged(rawValue: string): void {
    const text = (rawValue ?? '').trim();
    if (!text) {
      this.facilityInput = '';
      this.selectedFacilityId = null;
      this.facilityContext.setActiveFacilityId(null);
      return;
    }

    const selected = this.facilities.find((f) => this.getFacilityOptionLabel(f).toLowerCase() === text.toLowerCase());
    const facilityId = selected?.id ?? null;
    this.facilityInput = selected ? this.getFacilityOptionLabel(selected) : text;
    this.selectedFacilityId = facilityId;
    this.facilityContext.setActiveFacilityId(facilityId);
  }

  getFacilityOptionLabel(facility: FacilityDto): string {
    return facility.code ? `${facility.name} (${facility.code})` : facility.name;
  }

  private loadNavigationMenus(): void {
    this.loadingMenus = true;
    // Use my-menus so any role (e.g. Doctor) can load their assigned menus without needing admin APIs.
    this.menusService.getMyMenus().subscribe({
      next: (menus) => {
        this.loadingMenus = false;
        this.buildNavTree(menus.filter((m) => m.isActive));
      },
      error: () => {
        this.loadingMenus = false;
        this.navItems = [];
      },
    });
  }

  private loadFacilities(): void {
    this.facilitiesService
      .getFacilities({
        pageNumber: 1,
        pageSize: 200,
        sortBy: 'name',
        sortDesc: false,
      })
      .subscribe({
        next: (result) => {
          this.facilities = result.items ?? [];
          if (
            this.selectedFacilityId &&
            !this.facilities.some((f) => f.id === this.selectedFacilityId)
          ) {
            this.selectedFacilityId = null;
            this.facilityContext.setActiveFacilityId(null);
          }

          if (this.selectedFacilityId) {
            const selected = this.facilities.find((f) => f.id === this.selectedFacilityId);
            this.facilityInput = selected ? this.getFacilityOptionLabel(selected) : '';
          } else {
            this.facilityInput = '';
          }
        },
        error: () => {
          this.facilities = [];
        },
      });
  }

  private buildNavTree(menus: MenuDto[]): void {
    const byId = new Map<number, NavItem>();
    const roots: NavItem[] = [];

    const sorted = [...menus].sort(
      (a, b) =>
        a.displayOrder - b.displayOrder ||
        a.name.localeCompare(b.name, undefined, { sensitivity: 'base' }),
    );

    for (const m of sorted) {
      byId.set(m.id, {
        id: m.id,
        name: m.name,
        menuKey: m.menuKey,
        url: m.url,
        icon: m.icon,
        children: [],
        expanded: true,
      });
    }

    for (const node of byId.values()) {
      const parentId = menus.find((m) => m.id === node.id)?.parentId;
      if (parentId && byId.has(parentId)) {
        byId.get(parentId)!.children.push(node);
      } else {
        roots.push(node);
      }
    }

    this.navItems = roots;
  }
}
