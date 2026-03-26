using HrmsH.Application.Billing.Installments.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Installments.Queries;

public sealed record GetPatientPaymentHistoryQuery(int PatientId, int? FacilityId = null) : IRequest<PatientPaymentHistoryDto>;
