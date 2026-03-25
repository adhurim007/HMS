using HrmsH.Application.Pharmacy.Stock.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Stock.Queries;

public sealed record GetStockBatchesByProductQuery(int ProductId) : IRequest<IReadOnlyList<StockBatchDto>>;
