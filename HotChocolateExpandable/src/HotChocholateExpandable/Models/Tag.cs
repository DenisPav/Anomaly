using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace HotChocholateExpandable.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public ICollection<BlogPostTag> TagBlogPosts { get; set; }
    }

    public class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TagName).IsRequired();

            builder.HasMany(x => x.TagBlogPosts)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId)
                .IsRequired();
        }
    }

    public class TagObjectType : ObjectType<Tag>
    {
        protected override void Configure(IObjectTypeDescriptor<Tag> descriptor)
        {

        }
    }
}
