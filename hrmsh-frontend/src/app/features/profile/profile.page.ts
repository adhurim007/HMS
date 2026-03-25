import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

interface ProfileDto {
  id: number;
  email: string;
  fullName?: string | null;
  phone?: string | null;
}

interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: unknown;
}

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss',
})
export class ProfilePage implements OnInit {
  loadingProfile = false;
  savingProfile = false;
  changingPassword = false;

  profileError = '';
  passwordError = '';
  passwordSuccess = '';

  readonly profileForm = this.fb.group({
    fullName: [''],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
  });

  readonly passwordForm = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly api: ApiService,
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loadingProfile = true;
    this.profileError = '';
    this.api
      .get<ApiResponse<ProfileDto>>('Profile')
      .subscribe({
        next: (res) => {
          this.loadingProfile = false;
          if (!res.success || !res.data) {
            this.profileError =
              res.message || 'Failed to load profile information.';
            return;
          }
          const p = res.data;
          this.profileForm.patchValue({
            fullName: p.fullName ?? '',
            email: p.email,
            phone: p.phone ?? '',
          });
        },
        error: (err) => {
          this.loadingProfile = false;
          const msg =
            err?.error?.message ??
            err?.message ??
            'Failed to load profile information.';
          this.profileError = msg;
        },
      });
  }

  submitProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const { fullName, email, phone } = this.profileForm.value;
    this.savingProfile = true;
    this.profileError = '';

    this.api
      .put<ApiResponse<ProfileDto>>('Profile', {
        fullName: fullName ?? null,
        email: email!,
        phone: phone ?? null,
      })
      .subscribe({
        next: (res) => {
          this.savingProfile = false;
          if (!res.success || !res.data) {
            this.profileError = res.message || 'Failed to save profile.';
            return;
          }
          const p = res.data;
          this.profileForm.patchValue({
            fullName: p.fullName ?? '',
            email: p.email,
            phone: p.phone ?? '',
          });
        },
        error: (err) => {
          this.savingProfile = false;
          const msg =
            err?.error?.message ??
            err?.message ??
            'Failed to save profile.';
          this.profileError = msg;
        },
      });
  }

  submitPassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const { currentPassword, newPassword } = this.passwordForm.value;
    this.changingPassword = true;
    this.passwordError = '';
    this.passwordSuccess = '';

    this.api
      .post<ApiResponse<object>>('Profile/change-password', {
        currentPassword,
        newPassword,
      })
      .subscribe({
        next: (res) => {
          this.changingPassword = false;
          if (!res.success) {
            this.passwordError = res.message || 'Failed to change password.';
            return;
          }
          this.passwordForm.reset();
          this.passwordSuccess = 'Password changed successfully.';
        },
        error: (err) => {
          this.changingPassword = false;
          const msg =
            err?.error?.message ??
            err?.message ??
            'Failed to change password.';
          this.passwordError = msg;
        },
      });
  }
}

