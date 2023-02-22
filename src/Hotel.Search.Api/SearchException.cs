using Hotel.Common;

namespace Hotel.Search.Api;

public class SearchException : HotelException
{
  public SearchException(string message) : base(message)
  {

  }
}
