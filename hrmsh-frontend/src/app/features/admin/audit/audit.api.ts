export interface AuditLogItem {
  id: number;
  entityType: string;
  entityId: number;
  action: string;
  createdAt: string;
  userName?: string | null;
  userId?: number | null;
  patientId?: number | null;
  description?: string | null;
}

export interface PagedAuditResult {
  items: AuditLogItem[];
  totalCount: number;
}

