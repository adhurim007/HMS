export interface NotificationDto {
  type: string;
  key: string;
  title: string;
  message: string;
  link?: string | null;
  createdAt: string;
  isRead: boolean;
}
