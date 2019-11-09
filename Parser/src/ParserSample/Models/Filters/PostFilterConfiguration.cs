using ParserSample.Filters;

namespace ParserSample.Models.Filters
{
    public class PostFilterConfiguration : FilterConfiguration<Post>
    {
        public override void Configure(FilterBuilder<Post> configuration)
        {
            configuration.Define("Id", post => post.Id);
            configuration.Define("Count", post => post.Count);
            configuration.Define("Date", post => post.CreationDate);
            configuration.Define("Title", post => post.Name);
        }
    }
}
