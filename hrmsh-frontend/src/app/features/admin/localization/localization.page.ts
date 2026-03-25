import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { LocalizationService } from './localization.service';
import { LanguageDto, TranslationDto } from './localization.api';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';

@Component({
  selector: 'app-localization-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterLink,
    TranslatePipe,
  ],
  templateUrl: './localization.page.html',
  styleUrl: './localization.page.scss',
})
export class LocalizationPage implements OnInit {
  languages: LanguageDto[] = [];
  selectedLanguageCode: string | null = null;

  translations: TranslationDto[] = [];
  loadingLanguages = false;
  loadingTranslations = false;
  savingLanguage = false;
  savingTranslation = false;

  translationSearch = '';

  readonly languageForm = this.fb.group({
    id: [null as number | null],
    code: ['', [Validators.required]],
    name: ['', [Validators.required]],
    isDefault: [false],
    isActive: [true],
  });

  readonly translationForm = this.fb.group({
    id: [null as number | null],
    key: ['', [Validators.required]],
    value: ['', [Validators.required]],
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly localization: LocalizationService,
  ) {}

  ngOnInit(): void {
    this.loadLanguages();
  }

  get filteredTranslations(): TranslationDto[] {
    const term = this.translationSearch.trim().toLowerCase();
    if (!term) {
      return this.translations;
    }
    return this.translations.filter(
      (t) =>
        t.key.toLowerCase().includes(term) ||
        t.value.toLowerCase().includes(term),
    );
  }

  loadLanguages(): void {
    this.loadingLanguages = true;
    this.localization.getLanguages().subscribe({
      next: (list) => {
        this.loadingLanguages = false;
        this.languages = list;
        if (!this.selectedLanguageCode && list.length > 0) {
          const def = list.find((x) => x.isDefault) ?? list[0];
          this.selectLanguage(def.code);
        }
      },
      error: () => {
        this.loadingLanguages = false;
      },
    });
  }

  selectLanguage(code: string): void {
    this.selectedLanguageCode = code;
    const lang = this.languages.find((x) => x.code === code);
    if (lang) {
      this.languageForm.setValue({
        id: lang.id,
        code: lang.code,
        name: lang.name,
        isDefault: lang.isDefault,
        isActive: lang.isActive,
      });
    } else {
      this.languageForm.reset({
        id: null,
        code,
        name: '',
        isDefault: false,
        isActive: true,
      });
    }
    this.translationForm.reset({
      id: null,
      key: '',
      value: '',
    });
    this.loadTranslations();
  }

  newLanguage(): void {
    this.selectedLanguageCode = null;
    this.languageForm.reset({
      id: null,
      code: '',
      name: '',
      isDefault: false,
      isActive: true,
    });
    this.translations = [];
    this.translationForm.reset({
      id: null,
      key: '',
      value: '',
    });
  }

  submitLanguage(): void {
    if (this.languageForm.invalid) {
      this.languageForm.markAllAsTouched();
      return;
    }
    const value = this.languageForm.value;
    this.savingLanguage = true;
    this.localization
      .saveLanguage({
        id: value.id,
        code: value.code!.trim(),
        name: value.name!.trim(),
        isDefault: !!value.isDefault,
        isActive: !!value.isActive,
      })
      .subscribe({
        next: (saved) => {
          this.savingLanguage = false;
          const existingIndex = this.languages.findIndex(
            (x) => x.id === saved.id,
          );
          if (existingIndex >= 0) {
            this.languages[existingIndex] = saved;
          } else {
            this.languages.push(saved);
          }
          this.selectLanguage(saved.code);
        },
        error: () => {
          this.savingLanguage = false;
        },
      });
  }

  loadTranslations(): void {
    if (!this.selectedLanguageCode) {
      this.translations = [];
      return;
    }
    this.loadingTranslations = true;
    this.localization.getTranslations(this.selectedLanguageCode).subscribe({
      next: (list) => {
        this.loadingTranslations = false;
        this.translations = list;
      },
      error: () => {
        this.loadingTranslations = false;
      },
    });
  }

  editTranslation(row: TranslationDto): void {
    this.translationForm.setValue({
      id: row.id,
      key: row.key,
      value: row.value,
    });
  }

  newTranslation(): void {
    this.translationForm.reset({
      id: null,
      key: '',
      value: '',
    });
  }

  submitTranslation(): void {
    if (!this.selectedLanguageCode) {
      return;
    }
    if (this.translationForm.invalid) {
      this.translationForm.markAllAsTouched();
      return;
    }

    const value = this.translationForm.value;
    const key = value.key!.trim();
    const existing = this.translations.find((t) => t.key === key);
    const item: TranslationDto = {
      id: existing ? existing.id : value.id ?? 0,
      languageCode: this.selectedLanguageCode,
      key,
      value: value.value!.trim(),
    };

    this.savingTranslation = true;
    this.localization.saveTranslations(this.selectedLanguageCode, [item]).subscribe({
      next: (updatedList) => {
        this.savingTranslation = false;
        this.translations = updatedList;
        const saved = updatedList.find((t) => t.key === key);
        if (saved) {
          this.translationForm.setValue({
            id: saved.id,
            key: saved.key,
            value: saved.value,
          });
        } else {
          this.newTranslation();
        }
      },
      error: () => {
        this.savingTranslation = false;
      },
    });
  }
}

