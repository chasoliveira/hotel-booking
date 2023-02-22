using FluentValidation;

namespace Hotel.Booking.Api.Bookings.Commands.Validators;

public class NewBookingValidator : AbstractValidator<NewBookingCommand>
{
  const int MAX_DAY_IN_ADVANCE = 30;
  const int MAX_DAYS = 3;
  public NewBookingValidator()
  {
    ClassLevelCascadeMode = CascadeMode.Continue;

    RuleFor(b => b.RoomId)
        .NotEmpty()
        .GreaterThan(0);

    RuleFor(b => b.StartAt.Date)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .LessThanOrEqualTo(DateTime.Now.Date.AddDays(MAX_DAY_IN_ADVANCE).SetLastHourOfTheDay());

    RuleFor(b => b.EndAt.Date)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .GreaterThan(b => b.StartAt.Date)
        .Must((b, endAt) => (endAt.Date.SetLastHourOfTheDay() - b.StartAt.Date).TotalDays <= MAX_DAYS)
        .WithMessage("'End At' must to exceed three days.");
  }
}