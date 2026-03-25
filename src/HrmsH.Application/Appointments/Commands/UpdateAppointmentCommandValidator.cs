using FluentValidation;

namespace HrmsH.Application.Appointments.Commands;

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ScheduledStart).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
        RuleFor(x => x.ScheduledEnd)
            .Must((cmd, end) => end == null || end > cmd.ScheduledStart)
            .WithMessage("Scheduled end must be later than scheduled start.");
        RuleFor(x => x)
            .Must(x => x.ScheduledEnd == null || (x.ScheduledEnd.Value - x.ScheduledStart).TotalHours <= 12)
            .WithMessage("Appointment duration cannot exceed 12 hours.");
    }
}

