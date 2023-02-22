using System.Diagnostics.CodeAnalysis;

namespace Hotel.Common.Data;

[ExcludeFromCodeCoverage]
public static class SpecificationExtensions
{
  public static IQueryable<TEntity> Specify<TEntity>(this IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec) where TEntity : class
  {
    var query = inputQuery;

    if (spec.OrderBy != null)
      query = query.OrderBy(spec.OrderBy);

    if (spec.OrderByDescending != null)
      query = query.OrderByDescending(spec.OrderByDescending);
    
    if (spec.Criteria != null)
      query = query.Where(spec.Criteria);
      
    return query;
  }
}