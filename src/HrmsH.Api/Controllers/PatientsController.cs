using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Patients.Commands;
using HrmsH.Application.Patients.Dtos;
using HrmsH.Application.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
    public sealed class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly IHrmsDbContext _db;

        public PatientsController(IMediator mediator, ICurrentUserService currentUser, IHrmsDbContext db)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<PagedApiResponse<PatientDto>>> GetList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false,
            [FromQuery] string? search = null)
        {
        // Doctors should only see their own patients; admins/reception see all.
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (staffId != 0)
            {
                var patientIdsQuery =
                    _db.Visits.AsNoTracking().Where(v => v.DoctorId == staffId).Select(v => v.PatientId)
                    .Concat(_db.Appointments.AsNoTracking().Where(a => a.DoctorId == staffId).Select(a => a.PatientId))
                    .Distinct();

                var patientIds = await patientIdsQuery.ToListAsync();
                if (patientIds.Count == 0)
                {
                    return Ok(new PagedApiResponse<PatientDto>
                    {
                        Success = true,
                        Items = Array.Empty<PatientDto>(),
                        TotalCount = 0
                    });
                }

                var pagination = new PaginationParams
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDesc = sortDesc,
                    Search = search
                };

                // Reuse existing handler logic by filtering before it runs
                var query = _db.Patients.AsNoTracking().Where(p => patientIds.Contains(p.Id));

                if (!string.IsNullOrWhiteSpace(pagination.Search))
                {
                    var term = pagination.Search;
                    var normalizedTerm = term.Trim()
                        .Replace(" ", string.Empty)
                        .Replace("-", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace("/", string.Empty);
                    query = query.Where(x =>
                        x.FullName.Contains(term) ||
                        x.MedicalRecordNumber.Contains(term) ||
                        x.MedicalRecordNumber.Contains(normalizedTerm) ||
                        (x.Phone != null && (x.Phone.Contains(term) || x.Phone.Contains(normalizedTerm))));
                }

                query = (pagination.SortBy?.ToLowerInvariant()) switch
                {
                    "name" => pagination.SortDesc ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
                    "mrn" => pagination.SortDesc ? query.OrderByDescending(x => x.MedicalRecordNumber) : query.OrderBy(x => x.MedicalRecordNumber),
                    _ => query.OrderByDescending(x => x.Id)
                };

                var total = await query.CountAsync();
                var items = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(x => new PatientDto
                    {
                        Id = x.Id,
                        MedicalRecordNumber = x.MedicalRecordNumber,
                        FullName = x.FullName,
                        DateOfBirth = x.DateOfBirth,
                        Gender = x.Gender,
                        Phone = x.Phone,
                        Email = x.Email,
                        Address = x.Address
                    })
                    .ToListAsync();

                return Ok(new PagedApiResponse<PatientDto>
                {
                    Success = true,
                    Items = items,
                    TotalCount = total
                });
            }
        }

        var result = await _mediator.Send(new GetPatientsQuery(new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Search = search
        }));

        return Ok(new PagedApiResponse<PatientDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("by-mrn/{mrn}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetByMedicalRecordNumber([FromRoute] string mrn)
    {
        if (string.IsNullOrWhiteSpace(mrn))
            return BadRequest(ApiResponse<PatientDto>.Fail("Medical record number is required."));

        var normalized = mrn.Trim();

        var dto = await _db.Patients
            .AsNoTracking()
            .Where(p => p.MedicalRecordNumber == normalized)
            .Select(x => new PatientDto
            {
                Id = x.Id,
                MedicalRecordNumber = x.MedicalRecordNumber,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                BloodGroup = x.BloodGroup,
                ChronicConditions = x.ChronicConditions,
                Allergies = x.Allergies,
                ParentGuardianName = x.ParentGuardianName,
                PediatricMtl = x.PediatricMtl,
                PediatricGjtl = x.PediatricGjtl,
                PediatricPkl = x.PediatricPkl,
                PriorLiveBirth = x.PriorLiveBirth,
                PriorAbortion = x.PriorAbortion
            })
            .FirstOrDefaultAsync();

        if (dto is null)
            return NotFound(ApiResponse<PatientDto>.Fail("Patient not found."));

        return Ok(ApiResponse<PatientDto>.Ok(dto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetPatientByIdQuery(id));
        return Ok(ApiResponse<PatientDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create([FromBody] CreatePatientCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<PatientDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update([FromRoute] int id, [FromBody] UpdatePatientCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<PatientDto>.Fail("Route id does not match body id."));

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<PatientDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeletePatientCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

