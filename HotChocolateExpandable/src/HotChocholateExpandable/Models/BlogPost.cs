using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace HotChocholateExpandable.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public Guid BlogId { get; set; }
        public Blog Blog { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<BlogPostTag> BlogPostTags { get; set; }
    }

    public class BlogPostConfig : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Content).IsRequired();

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.BlogPost)
                .HasForeignKey(x => x.BlogPostId)
                .IsRequired();

            builder.HasMany(x => x.BlogPostTags)
                .WithOne(x => x.BlogPost)
                .HasForeignKey(x => x.BlogPostId)
                .IsRequired();
        }
    }

    public class BlogPostObjectType : ObjectType<BlogPost>
    {
        protected override void Configure(IObjectTypeDescriptor<BlogPost> descriptor)
        {
            descriptor.Name("blogPosts");
        }
    }
}
