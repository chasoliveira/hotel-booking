using FluentValidation;

namespace Hotel.Search.Api.Rooms.Validations;

public class RoomsAvailableValidator : AbstractValidator<AvailableRoomsQuery>
{
  const int MAX_DAY_IN_ADVANCE = 30;
  public RoomsAvailableValidator()
  {
    RuleFor(a => a.EndAt).LessThanOrEqualTo(DateTime.Now.AddDays(MAX_DAY_IN_ADVANCE));
    RuleFor(a => a.StartAt).LessThanOrEqualTo(a => a.EndAt);
  }
}