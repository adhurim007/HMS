import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { AuditLogItem, PagedAuditResult } from './audit.api';

@Injectable({ providedIn: 'root' })
export class AuditService {
  constructor(private readonly api: ApiService) {}

  getAuditLogs(params: {
    entityType?: string | null;
    patientId?: number | null;
    userName?: string | null;
    fromUtc?: string | null;
    toUtc?: string | null;
    pageNumber?: number;
    pageSize?: number;
  }): Observable<PagedAuditResult> {
    const query: Record<string, string> = {};
    if (params.entityType) query['entityType'] = params.entityType;
    if (params.patientId != null) query['patientId'] = String(params.patientId);
    if (params.userName) query['userName'] = params.userName;
    if (params.fromUtc) query['fromUtc'] = params.fromUtc;
    if (params.toUtc) query['toUtc'] = params.toUtc;
    if (params.pageNumber) query['pageNumber'] = String(params.pageNumber);
    if (params.pageSize) query['pageSize'] = String(params.pageSize);

    return this.api
      .get<{ success: boolean; data: PagedAuditResult }>('Audit', query)
      .pipe(map((x) => x.data));
  }
}

