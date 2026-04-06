/** Mirrors backend VisitFormTemplates (department Code → same value). */
export const VisitFormTemplate = {
  General: 'GENERAL',
  Pediatrics: 'PEDIATRICS',
  Gynecology: 'GYNECOLOGY',
  Dentistry: 'DENTISTRY',
} as const;

export type VisitFormTemplateId =
  (typeof VisitFormTemplate)[keyof typeof VisitFormTemplate];

export interface VisitClinicalVitals {
  weightValue?: number | null;
  weightUnit?: string | null;
  heightCm?: number | null;
  headCircumferenceCm?: number | null;
  temperatureC?: number | null;
  spo2?: number | null;
  glucoseMgDl?: number | null;
  pulseBpm?: number | null;
  bloodPressureSystolic?: number | null;
  bloodPressureDiastolic?: number | null;
}

/** Versioned payload stored in Visit.clinicalDataJson (v = 1). */
export interface VisitClinicalV1 {
  v: 1;
  vitals?: VisitClinicalVitals;
  complaintsHtml?: string | null;
  examinationsHtml?: string | null;
  diagnosesHtml?: string | null;
  therapiesHtml?: string | null;
  adviceHtml?: string | null;
  colposcopyHtml?: string | null;
  spermiogramHtml?: string | null;
}

export function resolveTemplateFromDepartmentCode(
  code?: string | null,
): VisitFormTemplateId {
  const c = (code ?? '').trim().toUpperCase();
  if (c === VisitFormTemplate.Pediatrics) return VisitFormTemplate.Pediatrics;
  if (c === VisitFormTemplate.Gynecology) return VisitFormTemplate.Gynecology;
  if (c === VisitFormTemplate.Dentistry) return VisitFormTemplate.Dentistry;
  return VisitFormTemplate.General;
}

export function defaultClinicalDraft(): VisitClinicalV1 {
  return {
    v: 1,
    vitals: {
      weightUnit: 'kg',
    },
    complaintsHtml: '',
    examinationsHtml: '',
    diagnosesHtml: '',
    therapiesHtml: '',
    adviceHtml: '',
    colposcopyHtml: '',
    spermiogramHtml: '',
  };
}

export function parseClinicalJson(raw: string | null | undefined): VisitClinicalV1 {
  const base = defaultClinicalDraft();
  if (!raw?.trim()) return base;
  try {
    const o = JSON.parse(raw) as Record<string, unknown>;
    if (o['v'] !== 1 && o['v'] !== '1') return base;
    const vitals = (o['vitals'] as VisitClinicalVitals | undefined) ?? {};
    return {
      v: 1,
      vitals: { ...base.vitals, ...vitals },
      complaintsHtml: (o['complaintsHtml'] as string) ?? '',
      examinationsHtml: (o['examinationsHtml'] as string) ?? '',
      diagnosesHtml: (o['diagnosesHtml'] as string) ?? '',
      therapiesHtml: (o['therapiesHtml'] as string) ?? '',
      adviceHtml: (o['adviceHtml'] as string) ?? '',
      colposcopyHtml: (o['colposcopyHtml'] as string) ?? '',
      spermiogramHtml: (o['spermiogramHtml'] as string) ?? '',
    };
  } catch {
    return base;
  }
}

/** BMI from kg and m (height in meters). */
export function computeBmiKgM(weightKg: number, heightM: number): number | null {
  if (!weightKg || !heightM || heightM <= 0) return null;
  return Math.round((weightKg / (heightM * heightM)) * 100) / 100;
}
