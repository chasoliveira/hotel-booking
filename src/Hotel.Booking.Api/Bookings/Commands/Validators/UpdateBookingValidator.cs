using FluentValidation;

namespace Hotel.Booking.Api.Bookings.Commands.Validators;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingCommand>
{
  const int MAX_DAY_IN_ADVANCE = 30;
  const int MAX_DAYS = 3;
  public UpdateBookingValidator()
  {
    ClassLevelCascadeMode = CascadeMode.Continue;

    RuleFor(b => b.StartAt)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .LessThanOrEqualTo(DateTime.Now.Date.AddDays(MAX_DAY_IN_ADVANCE).SetLastHourOfTheDay());

    RuleFor(b => b.EndAt)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .GreaterThan(b => b.StartAt.Date)
        .Must((b, endAt) => (endAt.Date.SetLastHourOfTheDay() - b.StartAt.Date).TotalDays <= MAX_DAYS);
  }
}
