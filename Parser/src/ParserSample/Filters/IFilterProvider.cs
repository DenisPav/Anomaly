using System.Linq;

namespace ParserSample.Filters
{
    public interface IFilterProvider<TEntity>
    {
        IQueryable<TEntity> Apply(IQueryable<TEntity> query, FilterRequestModel filterModel);
    }
}
