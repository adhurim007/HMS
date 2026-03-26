import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FacilityContextService {
  private readonly storageKey = 'activeFacilityId';

  getActiveFacilityId(): number | null {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) {
      return null;
    }

    const value = Number(raw);
    return Number.isFinite(value) && value > 0 ? value : null;
  }

  setActiveFacilityId(value: number | null): void {
    if (!value || value <= 0) {
      localStorage.removeItem(this.storageKey);
      return;
    }

    localStorage.setItem(this.storageKey, String(value));
  }
}
