namespace ParserSample.Filters
{
    public class FilterContainer<TEntity>
    {
        public readonly FilterConfiguration<TEntity> FilterConfiguration;
        public readonly FilterBuilder<TEntity> FilterBuilder;

        public FilterContainer(
            FilterBuilder<TEntity> filterBuilder,
            FilterConfiguration<TEntity> filterConfiguration)
        {
            FilterBuilder = filterBuilder;
            FilterConfiguration = filterConfiguration;

            FilterConfiguration.Configure(FilterBuilder);
        }
    }
}
