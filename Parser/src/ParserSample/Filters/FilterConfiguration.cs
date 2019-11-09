namespace ParserSample.Filters
{
    public abstract class FilterConfiguration<TEntity>
    {
        public abstract void Configure(FilterBuilder<TEntity> configuration);
    }
}
