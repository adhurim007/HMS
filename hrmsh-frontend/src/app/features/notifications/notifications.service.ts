import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import { NotificationDto } from './notifications.api';

@Injectable({ providedIn: 'root' })
export class NotificationsService {
  constructor(private readonly api: ApiService) {}

  getList(): Observable<NotificationDto[]> {
    return this.api
      .get<{ success?: boolean; data?: NotificationDto[]; Data?: NotificationDto[] }>('Notifications')
      .pipe(map((r) => r.data ?? r.Data ?? []));
  }

  getUnreadCount(): Observable<number> {
    return this.api
      .get<{ success?: boolean; data?: number; Data?: number }>('Notifications/unread-count')
      .pipe(map((r) => r.data ?? r.Data ?? 0));
  }

  markRead(type: string, key: string): Observable<void> {
    return this.api
      .patch<{ success?: boolean }>('Notifications/mark-read', { type, key })
      .pipe(map(() => void 0));
  }
}
