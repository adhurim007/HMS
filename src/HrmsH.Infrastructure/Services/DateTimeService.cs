using HrmsH.Application.Common.Interfaces;

namespace HrmsH.Infrastructure.Services;

public sealed class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}

