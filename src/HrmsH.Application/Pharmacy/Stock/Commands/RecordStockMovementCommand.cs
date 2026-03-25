using HrmsH.Application.Pharmacy.Stock.Dtos;
using HrmsH.Domain.Pharmacy;
using MediatR;

namespace HrmsH.Application.Pharmacy.Stock.Commands;

public sealed record RecordStockMovementCommand(
    int ProductId,
    int? StockBatchId,
    StockMovementType Type,
    int Quantity,
    string? Reason,
    bool IsIncreaseForAdjustment = true) : IRequest<StockMovementDto>;
