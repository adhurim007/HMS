import { Injectable } from '@angular/core';
import { ApiService } from '../services/api.service';
import { map } from 'rxjs/operators';

type LangCode = 'en' | 'sq';

type TranslationMap = Record<string, string>;

const STATIC_TRANSLATIONS: Record<LangCode, TranslationMap> = {
  en: {
    // Layout / menu
    'layout.brand': 'HRMSH',
    'app.tagline': 'Hospital Management System',
    'menu.dashboard': 'Dashboard',
    'menu.patients': 'Patients',
    'menu.appointments': 'Appointments',
    'menu.visits': 'Visits',
    'menu.doctors': 'Doctors',
    'menu.billing': 'Billing',
    'menu.pharmacy': 'Pharmacy',
    'menu.pharmacy.products': 'Pharmacy Products',
    'menu.pharmacy.stock': 'Stock',
    'menu.facilities': 'Facilities',
    'menu.departments': 'Departments',
    'menu.admin': 'Administration',
    'menu.menus': 'Menu Management',
    'menu.users': 'Users',
    'menu.localization': 'Localization',

    // Auth
    'auth.login.title': 'Sign In',
    'auth.login.welcomeTitle': 'Welcome Back!',
    'auth.login.subtitle': 'Sign in to continue to HRMSH.',
    'auth.login.email': 'Email',
    'auth.login.emailPlaceholder': 'Enter email',
    'auth.login.emailRequired': 'Email is required',
    'auth.login.emailInvalid': 'Email must be valid',
    'auth.login.password': 'Password',
    'auth.login.passwordPlaceholder': 'Enter password',
    'auth.login.passwordRequired': 'Password is required',
    'auth.login.submit': 'Sign In',
    'auth.login.loading': 'Signing in...',
    'auth.login.noAccount': "Don't have an account?",
    'auth.login.goToDashboard': 'Go to Dashboard',

    // Patients
    'patients.title': 'Patients',
    'patients.list': 'Patient List',
    'patients.add': 'Add Patient',
    'patients.mrn': 'MRN',
    'patients.name': 'Name',
    'patients.dob': 'Date of Birth',
    'patients.gender': 'Gender',
    'patients.contact': 'Contact',
    'patients.address': 'Address',

    // Patient billing
    'patientBilling.title': 'Patient Billing',
    'patientBilling.totalInvoiced': 'Total invoiced',
    'patientBilling.totalPaid': 'Total paid',
    'patientBilling.outstanding': 'Outstanding balance',
    'patientBilling.invoices': 'Invoices',
    'patientBilling.goToBilling': 'Go to Billing',
    'patientBilling.addPayment': 'Add payment',
    'patientBilling.recordPayment': 'Record payment',

    // Buttons
    'common.cancel': 'Cancel',
    'common.save': 'Save',

    // Admin / localization
    // Admin / menus
    'admin.menus.title': 'Menu management',
    'admin.menus.menus': 'Menus',
    'admin.menus.newMenu': 'New menu',
    'admin.menus.editMenu': 'Edit menu',
    'admin.menus.menuName': 'Menu name',
    'admin.menus.menuKey': 'Menu key',
    'admin.menus.url': 'Route path',
    'admin.menus.parent': 'Parent menu',
    'admin.menus.noParent': 'No parent (top level)',
    'admin.menus.displayOrder': 'Display order',
    'admin.menus.icon': 'Icon class',
    'admin.menus.active': 'Active',
    'admin.menus.actions': 'Actions',
    'admin.menus.noMenus': 'No menus defined yet.',
    'admin.menus.savingMenu': 'Saving menu...',
    'admin.menus.roleAssignment': 'Role menu assignment',
    'admin.menus.selectRole': 'Select role',
    'admin.menus.selectRolePlaceholder': 'Choose a role...',
    'admin.menus.allowed': 'Allowed',
    'admin.menus.saveRoleMenus': 'Save permissions',
    'admin.menus.savingRoleMenus': 'Saving permissions...',
    'admin.menus.selectRoleHint':
      'Select a role to configure which menus it can access.',
    'admin.localization.title': 'Localization',
    'admin.localization.languages': 'Languages',
    'admin.localization.newLanguage': 'New language',
    'admin.localization.languageCode': 'Language code',
    'admin.localization.languageName': 'Language name',
    'admin.localization.default': 'Default',
    'admin.localization.active': 'Active',
    'admin.localization.savingLanguage': 'Saving language...',
    'admin.localization.translations': 'Translations',
    'admin.localization.searchTranslations': 'Search key or value...',
    'admin.localization.newTranslation': 'New translation',
    'admin.localization.selectLanguageHint':
      'Select or create a language to manage its translations.',
    'admin.localization.key': 'Key',
    'admin.localization.value': 'Value',
    'admin.localization.saveTranslation': 'Save translation',
    'admin.localization.savingTranslation': 'Saving translation...',
    'admin.localization.actions': 'Actions',
    'admin.localization.noTranslations': 'No translations found for this language.',
  },
  sq: {
    // Layout / menu
    'layout.brand': 'HRMSH',
    'app.tagline': 'Sistemi i menaxhimit të spitalit',
    'menu.dashboard': 'Paneli',
    'menu.patients': 'Pacientët',
    'menu.appointments': 'Terminët',
    'menu.visits': 'Vizitat',
    'menu.doctors': 'Mjekët',
    'menu.billing': 'Faturimi',
    'menu.pharmacy': 'Farmacia',
    'menu.pharmacy.products': 'Produkte Farmacie',
    'menu.pharmacy.stock': 'Stoku',
    'menu.facilities': 'Institucionet',
    'menu.departments': 'Departamentet',
    'menu.admin': 'Administrimi',
    'menu.menus': 'Menutë',
    'menu.users': 'Përdoruesit',
    'menu.localization': 'Përkthimet',

    // Auth
    'auth.login.title': 'Hyrja',
    'auth.login.welcomeTitle': 'Mirë se u kthyet!',
    'auth.login.subtitle': 'Hyni për të vazhduar në HRMSH.',
    'auth.login.email': 'Email',
    'auth.login.emailPlaceholder': 'Shkruani emailin',
    'auth.login.emailRequired': 'Emaili është i detyrueshëm',
    'auth.login.emailInvalid': 'Emaili duhet të jetë i vlefshëm',
    'auth.login.password': 'Fjalëkalimi',
    'auth.login.passwordPlaceholder': 'Shkruani fjalëkalimin',
    'auth.login.passwordRequired': 'Fjalëkalimi është i detyrueshëm',
    'auth.login.submit': 'Hyr',
    'auth.login.loading': 'Duke u identifikuar...',
    'auth.login.noAccount': 'Nuk keni llogari?',
    'auth.login.goToDashboard': 'Shko te paneli',

    // Patients
    'patients.title': 'Pacientët',
    'patients.list': 'Lista e pacientëve',
    'patients.add': 'Shto pacient',
    'patients.mrn': 'Numri i kartelës',
    'patients.name': 'Emri',
    'patients.dob': 'Data e lindjes',
    'patients.gender': 'Gjinia',
    'patients.contact': 'Kontakti',
    'patients.address': 'Adresa',

    // Patient billing
    'patientBilling.title': 'Bilanci i pacientit',
    'patientBilling.totalInvoiced': 'Totali i faturuar',
    'patientBilling.totalPaid': 'Totali i paguar',
    'patientBilling.outstanding': 'Detyrimi i mbetur',
    'patientBilling.invoices': 'Faturat',
    'patientBilling.goToBilling': 'Shko te Faturimi',
    'patientBilling.addPayment': 'Shto pagesë',
    'patientBilling.recordPayment': 'Regjistro pagesën',

    // Buttons
    'common.cancel': 'Anulo',
    'common.save': 'Ruaj',

    // Admin / localization
    // Admin / menus
    'admin.menus.title': 'Menaxhimi i menuseve',
    'admin.menus.menus': 'Menutë',
    'admin.menus.newMenu': 'Meny e re',
    'admin.menus.editMenu': 'Përditëso menynë',
    'admin.menus.menuName': 'Emri i menysë',
    'admin.menus.menuKey': 'Çelësi i menysë',
    'admin.menus.url': 'Rruga (route)',
    'admin.menus.parent': 'Meny prind',
    'admin.menus.noParent': 'Pa prind (niveli kryesor)',
    'admin.menus.displayOrder': 'Renditja',
    'admin.menus.icon': 'Klasa e ikonës',
    'admin.menus.active': 'Aktive',
    'admin.menus.actions': 'Veprimet',
    'admin.menus.noMenus': 'Ende nuk ka meny të përcaktuara.',
    'admin.menus.savingMenu': 'Duke ruajtur menynë...',
    'admin.menus.roleAssignment': 'Caktimi i menuseve për rol',
    'admin.menus.selectRole': 'Zgjidh rolin',
    'admin.menus.selectRolePlaceholder': 'Zgjidh një rol...',
    'admin.menus.allowed': 'Lejohet',
    'admin.menus.saveRoleMenus': 'Ruaj lejet',
    'admin.menus.savingRoleMenus': 'Duke ruajtur lejet...',
    'admin.menus.selectRoleHint':
      'Zgjidh një rol për të konfiguruar cilat meny mund të shohë.',
    'admin.localization.title': 'Përkthimet',
    'admin.localization.languages': 'Gjuhët',
    'admin.localization.newLanguage': 'Gjuhë e re',
    'admin.localization.languageCode': 'Kodi i gjuhës',
    'admin.localization.languageName': 'Emri i gjuhës',
    'admin.localization.default': 'Parazgjedhur',
    'admin.localization.active': 'Aktive',
    'admin.localization.savingLanguage': 'Duke ruajtur gjuhën...',
    'admin.localization.translations': 'Përkthimet',
    'admin.localization.searchTranslations': 'Kërko sipas çelësit ose vlerës...',
    'admin.localization.newTranslation': 'Përkthim i ri',
    'admin.localization.selectLanguageHint':
      'Zgjidhni ose krijoni një gjuhë për të menaxhuar përkthimet e saj.',
    'admin.localization.key': 'Çelësi',
    'admin.localization.value': 'Vlera',
    'admin.localization.saveTranslation': 'Ruaj përkthimin',
    'admin.localization.savingTranslation': 'Duke ruajtur përkthimin...',
    'admin.localization.actions': 'Veprimet',
    'admin.localization.noTranslations':
      'Nuk u gjetën përkthime për këtë gjuhë.',
  },
};

@Injectable({ providedIn: 'root' })
export class I18nService {
  private currentLang: LangCode = 'en';
  private loadedFromApi: Partial<Record<LangCode, TranslationMap>> = {};

  constructor(private readonly api: ApiService) {
    const stored =
      (typeof localStorage !== 'undefined' &&
        (localStorage.getItem('lang') as LangCode | null)) ||
      null;
    if (stored === 'en' || stored === 'sq') {
      this.currentLang = stored;
    }
    this.loadFromApi(this.currentLang);
  }

  get lang(): LangCode {
    return this.currentLang;
  }

  setLang(lang: string): void {
    if (lang === 'en' || lang === 'sq') {
      this.currentLang = lang;
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem('lang', lang);
      }
      this.loadFromApi(this.currentLang);
    }
  }

  t(key: string): string {
    const apiDict = this.loadedFromApi[this.currentLang];
    const baseDict =
      STATIC_TRANSLATIONS[this.currentLang] ?? STATIC_TRANSLATIONS.en;
    return apiDict?.[key] ?? baseDict[key] ?? key;
  }

  private loadFromApi(lang: LangCode): void {
    this.api
      .get<{
        success?: boolean;
        data?: Record<string, string>;
        Data?: Record<string, string>;
      }>(`Localization/${lang}`)
      .pipe(
        map((r) => r.data ?? r.Data ?? {}),
      )
      .subscribe({
        next: (dict) => {
          this.loadedFromApi[lang] = dict;
        },
        error: () => {
          // ignore, fallback to static translations
        },
      });
  }
}

