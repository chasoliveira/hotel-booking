using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Hotel.Common.Data;

public interface ISpecification<T>
{
  Expression<Func<T, bool>> Criteria { get; }
  Expression<Func<T, object>> OrderBy { get; }
  Expression<Func<T, object>> OrderByDescending { get; }
}

[ExcludeFromCodeCoverage]
public class BaseSpecifcation<T> : ISpecification<T>
{
  protected void Where(Expression<Func<T, bool>> criteria)
  {
    Criteria = criteria;
  }
  private Expression<Func<T, bool>>? _criteria;
  public Expression<Func<T, bool>>? Criteria { get; protected set ; }
  public Expression<Func<T, object>>? OrderBy { get; internal set; }
  public Expression<Func<T, object>>? OrderByDescending { get; private set; }

   protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
  {
    OrderBy = orderByExpression;
  }
  protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
  {
    OrderByDescending = orderByDescExpression;
  }
}