import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../../core/services/api.service';
import { LanguageDto, TranslationDto } from './localization.api';

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  constructor(private readonly api: ApiService) {}

  getLanguages(): Observable<LanguageDto[]> {
    return this.api
      .get<{ success: boolean; data: LanguageDto[] }>('Localization/languages')
      .pipe(map((x) => x.data));
  }

  saveLanguage(payload: {
    id?: number | null;
    code: string;
    name: string;
    isDefault: boolean;
    isActive: boolean;
  }): Observable<LanguageDto> {
    return this.api
      .post<{ success: boolean; data: LanguageDto }>('Localization/languages', {
        id: payload.id ?? null,
        code: payload.code,
        name: payload.name,
        isDefault: payload.isDefault,
        isActive: payload.isActive,
      })
      .pipe(map((x) => x.data));
  }

  getTranslations(code: string): Observable<TranslationDto[]> {
    return this.api
      .get<{ success: boolean; data: TranslationDto[] }>(
        `Localization/${code}/entries`,
      )
      .pipe(map((x) => x.data));
  }

  saveTranslations(
    code: string,
    items: TranslationDto[],
  ): Observable<TranslationDto[]> {
    return this.api
      .post<{ success: boolean; data: TranslationDto[] }>(
        `Localization/${code}/entries`,
        items,
      )
      .pipe(map((x) => x.data));
  }
}

