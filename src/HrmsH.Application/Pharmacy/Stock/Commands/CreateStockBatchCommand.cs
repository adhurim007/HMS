using HrmsH.Application.Pharmacy.Stock.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Stock.Commands;

public sealed record CreateStockBatchCommand(
    int ProductId,
    string? BatchNumber,
    DateTime? ExpiryDate,
    int Quantity,
    decimal? UnitCost = null) : IRequest<StockBatchDto>;
