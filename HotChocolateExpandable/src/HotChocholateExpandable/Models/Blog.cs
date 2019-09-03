using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace HotChocholateExpandable.Models
{
    public class Blog
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
    }

    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired();
            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();

            builder.HasMany(x => x.BlogPosts)
                .WithOne(x => x.Blog)
                .HasForeignKey(x => x.BlogId)
                .IsRequired();
        }
    }

    public class BlogObjectType : ObjectType<Blog>
    {
        protected override void Configure(IObjectTypeDescriptor<Blog> descriptor)
        {
            descriptor.Name("blogs");
        }
    }
}
