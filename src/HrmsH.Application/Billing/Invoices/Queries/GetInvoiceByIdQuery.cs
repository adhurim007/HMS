using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed record GetInvoiceByIdQuery(int Id) : IRequest<InvoiceDto?>;
