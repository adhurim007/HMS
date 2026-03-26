using HrmsH.Application.Billing.Installments.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Installments.Queries;

public sealed record GetInstallmentPlansQuery(
    int? FacilityId = null,
    int? PatientId = null,
    int? InvoiceId = null) : IRequest<IReadOnlyList<InstallmentPlanDto>>;
