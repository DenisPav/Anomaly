using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotChocholateExpandable.Models
{
    public class BlogPostTag
    {
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

    public class BlogPostTagConfig : IEntityTypeConfiguration<BlogPostTag>
    {
        public void Configure(EntityTypeBuilder<BlogPostTag> builder)
        {
            builder.HasKey(x => new { x.BlogPostId, x.TagId });
        }
    }
}
