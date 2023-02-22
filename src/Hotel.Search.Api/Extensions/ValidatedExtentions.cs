using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

[ExcludeFromCodeCoverage]
public static class ValidatedExtentions
{
  public static IDictionary<string, string[]> ToProblem(this ValidationResult validation)
    => validation
        .Errors
        .GroupBy(vf => vf.PropertyName)
        .ToDictionary(group => group.Key, g => g.Select(vf => vf.ErrorMessage).ToArray());
}
