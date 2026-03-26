# Multi-Facility Upgrade Notes

## 1) Facility-scoped entity coverage

The following transactional entities now include nullable `FacilityId` for phased rollout:

- `Appointment`
- `Visit`
- `Invoice`
- `Payment`
- `InstallmentPlan`
- `InstallmentItem`
- `LaboratoryOrder`
- `StockMovement`
- `PharmacyPurchaseInvoice`

Staff multi-facility support is modeled with `StaffFacilityAssignment`.

## 2) Migration and backfill strategy implemented

Migration: `AddMultiFacilitySupport`

- Adds nullable `FacilityId` columns (safe phase-1 rollout).
- Adds indexes on all new `FacilityId` columns.
- Creates `StaffFacilityAssignments` table with unique `(StaffMemberId, FacilityId)`.
- Backfills with SQL in this order:
  1. Appointment <- Department facility
  2. Visit <- Doctor's department facility
  3. Invoice <- Visit/Lab source lines
  4. Payment <- Invoice facility
  5. InstallmentPlan <- Invoice facility
  6. InstallmentItem <- InstallmentPlan facility

Rollback remains standard EF migration down path.

## 3) Request context and authorization

New application contract:

- `IFacilityContextService` with `ActiveFacilityId`.

API implementation:

- `FacilityContextService` reads active facility from:
  - `X-Facility-Id` header (primary)
  - `facilityId` query parameter (fallback)

Authorization/business checks added in handlers:

- Doctor assignment validation to selected facility in appointment/visit create+update flows via `StaffFacilityAssignments`.

## 4) Command/query facility scoping

Facility support added to command/query contracts and handlers for:

- appointments
- visits
- invoices
- payments
- installment plans
- installment payments

List queries now apply facility filter by:

- explicit query `facilityId`, otherwise
- active facility context from request.

## 5) Frontend facility context

Added:

- `FacilityContextService` (local storage persistence)
- `X-Facility-Id` injection in `authInterceptor`
- facility selector in main layout (loads available facilities and sets active context)

## 6) Verification matrix and rollout

### Test matrix

1. Doctor with assignments in two facilities can create appointments/visits in both.
2. Doctor without assignment cannot create/update in unauthorized facility.
3. Same patient can have visits/invoices in different facilities.
4. Invoice/payment/installment history list responds to active facility selection.
5. API lists return facility-filtered data when `X-Facility-Id` is present.

### Phased rollout

1. Deploy migration with nullable `FacilityId`.
2. Run backfill and validate null counts.
3. Enable strict operational policy in services/UI (required active facility for daily ops).
4. After cleanup, convert key `FacilityId` columns to non-null in a follow-up migration.
