export interface LanguageDto {
  id: number;
  code: string;
  name: string;
  isDefault: boolean;
  isActive: boolean;
}

export interface TranslationDto {
  id: number;
  languageCode: string;
  key: string;
  value: string;
}

