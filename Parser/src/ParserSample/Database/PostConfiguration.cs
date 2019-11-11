using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParserSample.Models;
using System.Collections.Generic;

namespace ParserSample.Database
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(post => post.Id);

            builder.Property(post => post.Name)
                .IsRequired();
            builder.Property(post => post.Count)
                .IsRequired();
            builder.Property(post => post.CreationDate)
                .IsRequired();
        }
    }
}
