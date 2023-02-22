using FluentValidation;

namespace Hotel.Search.Api.Rooms.Validations;

public class RoomsAvailableValidator : AbstractValidator<AvailableRoomsQuery>
{
  const int MAX_DAY_IN_ADVANCE = 30;
  const int MAX_DAYS = 3;
  public RoomsAvailableValidator()
  {
    ClassLevelCascadeMode = CascadeMode.Continue;

    RuleFor(b => b.StartAt)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .GreaterThanOrEqualTo(DateTime.Now.Date)
        .LessThanOrEqualTo(DateTime.Now.Date.AddDays(MAX_DAY_IN_ADVANCE).SetLastHourOfTheDay());

    RuleFor(b => b.EndAt)
        .NotNull()
        .NotEqual(DateTime.MinValue)
        .GreaterThanOrEqualTo(b => b.StartAt.Date)
        .LessThanOrEqualTo(DateTime.Now.Date.AddDays(MAX_DAY_IN_ADVANCE).SetLastHourOfTheDay());
  }
}