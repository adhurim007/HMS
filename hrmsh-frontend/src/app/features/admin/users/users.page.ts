import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';
import { UserListDto, PagedUsersResponse } from './users.api';
import { HospitalsService } from '../hospitals/hospitals.service';
import { HospitalDto } from '../hospitals/hospitals.api';
import { FacilitiesService } from '../facilities/facilities.service';
import { FacilityDto } from '../facilities/facilities.api';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgFor, FormsModule],
  templateUrl: './users.page.html',
  styleUrl: './users.page.scss',
})
export class UsersPage implements OnInit {
  roles: { id: number; name: string }[] = [];
  loadingRoles = false;

  users: UserListDto[] = [];
  hospitals: HospitalDto[] = [];
  facilities: FacilityDto[] = [];
  facilityOptions: FacilityDto[] = [];
  totalCount = 0;
  userPageNumber = 1;
  userPageSize = 10;
  userSearch = '';
  loadingUsers = false;

  showResetModal = false;
  resetUser: UserListDto | null = null;
  resetPasswordValue = '';
  resetPasswordSaving = false;
  resetPasswordError = '';

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    role: ['', Validators.required],
    hospitalId: [null as number | null],
    facilityId: [null as number | null],
  });

  saving = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly api: ApiService,
    private readonly hospitalsService: HospitalsService,
    private readonly facilitiesService: FacilitiesService,
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    this.loadHospitals();
    this.loadFacilities();
    this.loadUsers();
  }

  get isSuperAdmin(): boolean {
    return this.auth.hasRole('SuperAdmin');
  }

  private loadRoles(): void {
    this.loadingRoles = true;
    this.api
      .get<{
        success: boolean;
        message?: string;
        data?: { id: number; name: string }[];
      }>('Roles')
      .subscribe({
        next: (res) => {
          this.loadingRoles = false;
          if (!res.success || !res.data) {
            this.errorMessage = res.message || 'Failed to load roles.';
            return;
          }
          this.roles = res.data;
        },
        error: () => {
          this.loadingRoles = false;
          this.errorMessage = 'Failed to load roles.';
        },
      });
  }

  private loadHospitals(): void {
    if (!this.isSuperAdmin) return;
    this.hospitalsService
      .getHospitals({ pageNumber: 1, pageSize: 500, sortBy: 'name', sortDesc: false })
      .subscribe({
        next: (res) => {
          this.hospitals = res.items;
        },
      });
  }

  private loadFacilities(): void {
    this.facilitiesService
      .getFacilities({ pageNumber: 1, pageSize: 1000, sortBy: 'name', sortDesc: false })
      .subscribe({
        next: (res) => {
          this.facilities = res.items;
          this.applyFacilityOptions();
        },
      });
  }

  onHospitalChanged(): void {
    this.form.patchValue({ facilityId: null });
    this.applyFacilityOptions();
  }

  private applyFacilityOptions(): void {
    const hospitalId = this.form.value.hospitalId ?? null;
    this.facilityOptions = this.facilities.filter(
      (f) => hospitalId == null || f.hospitalId === hospitalId,
    );
  }

  loadUsers(): void {
    this.loadingUsers = true;
    const params: Record<string, string> = {
      pageNumber: String(this.userPageNumber),
      pageSize: String(this.userPageSize),
    };
    if (this.userSearch.trim()) params['search'] = this.userSearch.trim();
    this.api.get<PagedUsersResponse>('Users', params).subscribe({
      next: (res) => {
        this.loadingUsers = false;
        const r = res as PagedUsersResponse & { Items?: UserListDto[]; TotalCount?: number };
        this.users = r.items ?? r.Items ?? [];
        this.totalCount = r.totalCount ?? r.TotalCount ?? 0;
      },
      error: () => {
        this.loadingUsers = false;
        this.users = [];
      },
    });
  }

  onUserSearchChange(): void {
    this.userPageNumber = 1;
    this.loadUsers();
  }

  changeUserPage(delta: number): void {
    const next = this.userPageNumber + delta;
    if (next < 1) return;
    const maxPage = Math.max(1, Math.ceil(this.totalCount / this.userPageSize));
    if (next > maxPage) return;
    this.userPageNumber = next;
    this.loadUsers();
  }

  openResetModal(user: UserListDto): void {
    this.resetUser = user;
    this.resetPasswordValue = '';
    this.resetPasswordError = '';
    this.showResetModal = true;
  }

  closeResetModal(): void {
    this.showResetModal = false;
    this.resetUser = null;
    this.resetPasswordValue = '';
  }

  submitResetPassword(): void {
    if (!this.resetUser || !this.resetPasswordValue.trim()) {
      this.resetPasswordError = 'Enter a new password.';
      return;
    }
    this.resetPasswordSaving = true;
    this.resetPasswordError = '';
    this.api
      .post<{ success: boolean; message?: string }>(
        `Users/${this.resetUser.id}/reset-password`,
        { newPassword: this.resetPasswordValue },
      )
      .subscribe({
        next: (res) => {
          this.resetPasswordSaving = false;
          if (res.success !== false) {
            this.closeResetModal();
          } else {
            this.resetPasswordError = res.message ?? 'Failed to reset password.';
          }
        },
        error: (err) => {
          this.resetPasswordSaving = false;
          const msg = err?.error?.message ?? err?.message ?? 'Failed to reset password.';
          this.resetPasswordError = msg;
        },
      });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, role, hospitalId, facilityId } = this.form.value;
    this.saving = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.auth
      .createUser(
        email!,
        password!,
        role!,
        this.isSuperAdmin ? (hospitalId ?? null) : null,
        facilityId ?? null,
      )
      .subscribe({
      next: () => {
        this.saving = false;
        this.successMessage = 'User created successfully.';
        this.form.reset({
          email: '',
          password: '',
          role: '',
          hospitalId: null,
          facilityId: null,
        });
        this.applyFacilityOptions();
        this.loadUsers();
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.message || 'Failed to create user.';
      },
    });
  }
}

