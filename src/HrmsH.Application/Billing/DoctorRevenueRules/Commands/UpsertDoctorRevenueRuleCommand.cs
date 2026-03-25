using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.DoctorRevenueRules.Dtos;
using FluentValidation;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.DoctorRevenueRules.Commands;

public sealed record UpsertDoctorRevenueRuleCommand(
    int? Id,
    int? DoctorId,
    int? DepartmentId,
    int? ServiceItemId,
    int MinVisitsPerDay,
    int? MaxVisitsPerDay,
    decimal DoctorSharePercent,
    decimal HospitalSharePercent,
    bool IsActive) : IRequest<DoctorRevenueRuleDto>;

public sealed class UpsertDoctorRevenueRuleCommandHandler
    : IRequestHandler<UpsertDoctorRevenueRuleCommand, DoctorRevenueRuleDto>
{
    private readonly IHrmsDbContext _db;

    public UpsertDoctorRevenueRuleCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<DoctorRevenueRuleDto> Handle(
        UpsertDoctorRevenueRuleCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DoctorId is null)
        {
            throw new ValidationException("Doctor is required for a revenue rule.");
        }

        if (request.DoctorSharePercent < 0 || request.DoctorSharePercent > 100 ||
            request.HospitalSharePercent < 0 || request.HospitalSharePercent > 100)
        {
            throw new ValidationException("Share percents must be between 0 and 100.");
        }

        if (request.DoctorSharePercent + request.HospitalSharePercent != 100)
        {
            throw new ValidationException("Doctor and hospital share must sum to 100%.");
        }

        DoctorRevenueRule entity;

        if (request.Id.HasValue)
        {
            entity = await _db.DoctorRevenueRules
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken)
                ?? throw new NotFoundException("Doctor revenue rule not found.");
        }
        else
        {
            entity = new DoctorRevenueRule();
            _db.DoctorRevenueRules.Add(entity);
        }

        entity.DoctorId = request.DoctorId;
        entity.DepartmentId = request.DepartmentId;
        entity.ServiceItemId = request.ServiceItemId;
        entity.MinVisitsPerDay = request.MinVisitsPerDay;
        entity.MaxVisitsPerDay = request.MaxVisitsPerDay;
        entity.DoctorSharePercent = request.DoctorSharePercent;
        entity.HospitalSharePercent = request.HospitalSharePercent;
        entity.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties for DTO.
        entity = await _db.DoctorRevenueRules
            .Include(x => x.Doctor)
            .Include(x => x.Department)
            .Include(x => x.ServiceItem)
            .FirstAsync(x => x.Id == entity.Id, cancellationToken);

        return new DoctorRevenueRuleDto(
            entity.Id,
            entity.DoctorId,
            entity.Doctor?.FullName,
            entity.DepartmentId,
            entity.Department?.Name,
            entity.ServiceItemId,
            entity.ServiceItem?.Name,
            entity.MinVisitsPerDay,
            entity.MaxVisitsPerDay,
            entity.DoctorSharePercent,
            entity.HospitalSharePercent,
            entity.IsActive);
    }
}

