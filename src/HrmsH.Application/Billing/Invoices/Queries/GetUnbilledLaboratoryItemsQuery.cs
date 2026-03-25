using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed record GetUnbilledLaboratoryItemsQuery(
    int PatientId,
    DateTime? From = null,
    DateTime? To = null,
    int? DoctorId = null) : IRequest<IReadOnlyList<UnbilledLaboratoryItemDto>>;
