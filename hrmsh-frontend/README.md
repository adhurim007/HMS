# HRMSH Frontend

Angular 17 frontend for the Hospital Management System (HRMSH), using the **Velzon** admin theme. It is built separately from the .NET backend and talks to it via REST API.

## Features

- **Dashboard** – KPI cards (inpatients, outpatients, pending invoices, appointments)
- **Patients** – Patient list (ready to wire to your backend)
- **Billing** – Invoice list and summary cards (ready for your billing API)
- **Menu Management** – Menus and role assignment (for `/api/menus` and `/api/roles`)
- **Login** – Sign-in page (Velzon auth layout; wire to your auth API)

## API configuration

Set your .NET backend base URL in:

- `src/environments/environment.ts` – `apiUrl` (e.g. `https://localhost:7001/api`)
- `src/environments/environment.prod.ts` – for production

Use `ApiService` (`src/app/core/services/api.service.ts`) in your features to call the backend.

## Theme

Velzon assets and SCSS are under `src/assets` (from the Velzon Angular theme). Layout uses Velzon’s vertical sidebar, topbar, and page structure.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
