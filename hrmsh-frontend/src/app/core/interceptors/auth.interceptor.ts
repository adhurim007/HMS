import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { FacilityContextService } from '../services/facility-context.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const facilityContext = inject(FacilityContextService);
  const token = auth.getToken();
  const facilityId = facilityContext.getActiveFacilityId();

  const headers: Record<string, string> = {};
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  if (facilityId) {
    headers['X-Facility-Id'] = String(facilityId);
  }

  if (Object.keys(headers).length === 0) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: headers,
  });

  return next(authReq);
};

