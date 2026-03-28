import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: unknown;
}

export interface LoginResponse {
  token: string;
}

export interface CreateUserResponse {
  id: number;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly STORAGE_KEY = 'hrmsh_token';
  private readonly EMAIL_KEY = 'hrmsh_email';
  private decodedTokenCache: any | null = null;

  constructor(
    private readonly api: ApiService,
    private readonly router: Router,
  ) {}

  login(email: string, password: string): Observable<void> {
    return this.api
      .post<ApiResponse<LoginResponse>>('Auth/login', { email, password })
      .pipe(
        tap((res) => {
          if (!res.success || !res.data?.token) {
            throw new Error(res.message || 'Login failed');
          }
          localStorage.setItem(this.STORAGE_KEY, res.data.token);
          localStorage.setItem(this.EMAIL_KEY, email);
        }),
        map(() => void 0),
      );
  }

  register(email: string, password: string): Observable<void> {
    return this.api
      .post<ApiResponse<LoginResponse>>('Auth/register', { email, password })
      .pipe(
        tap((res) => {
          if (!res.success || !res.data?.token) {
            throw new Error(res.message || 'Registration failed');
          }
          localStorage.setItem(this.STORAGE_KEY, res.data.token);
          localStorage.setItem(this.EMAIL_KEY, email);
        }),
        map(() => void 0),
      );
  }

  createUser(
    email: string,
    password: string,
    role: string,
    hospitalId?: number | null,
    facilityId?: number | null,
  ): Observable<CreateUserResponse> {
    return this.api
      .post<ApiResponse<CreateUserResponse>>('Auth/create-user', {
        email,
        password,
        role,
        hospitalId: hospitalId ?? null,
        facilityId: facilityId ?? null,
      })
      .pipe(
        map((res) => {
          if (!res.success || !res.data) {
            throw new Error(res.message || 'Failed to create user');
          }
          return res.data;
        }),
      );
  }

  logout(): void {
    localStorage.removeItem(this.STORAGE_KEY);
    localStorage.removeItem(this.EMAIL_KEY);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.STORAGE_KEY);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getEmail(): string | null {
    return localStorage.getItem(this.EMAIL_KEY);
  }

  getRoles(): string[] {
    const payload = this.getDecodedTokenPayload();
    if (!payload) {
      return [];
    }

    const roles: string[] = [];
    const roleClaim =
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      payload['role'] ??
      payload['roles'];

    if (Array.isArray(roleClaim)) {
      roles.push(...roleClaim);
    } else if (typeof roleClaim === 'string' && roleClaim) {
      roles.push(roleClaim);
    }

    const unique = Array.from(new Set(roles.map((r) => r.trim()))).filter(
      (r) => !!r,
    );
    return unique;
  }

  hasRole(role: string): boolean {
    const target = role.toLowerCase();
    return this.getRoles().some((r) => r.toLowerCase() === target);
  }

  private getDecodedTokenPayload(): any | null {
    const token = this.getToken();
    if (!token) return null;
    if (this.decodedTokenCache) {
      return this.decodedTokenCache;
    }
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = atob(payload);
      const parsed = JSON.parse(json);
      this.decodedTokenCache = parsed;
      return parsed;
    } catch {
      return null;
    }
  }
}

