using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Hotel.Common;
public class HotelException : Exception
{
  public HotelException(string message) : base(message)
  {  }

  public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
  {
    if (string.IsNullOrWhiteSpace(argument))
      throw new HotelException($" The parameter '{paramName}' has an invalid value: '{argument}'");
  }
}