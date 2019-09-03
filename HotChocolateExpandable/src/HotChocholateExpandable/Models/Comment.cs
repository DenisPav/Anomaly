using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace HotChocholateExpandable.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
    }

    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Text).IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .IsRequired();
        }
    }

    public class CommentObjectType : ObjectType<Comment>
    {
        protected override void Configure(IObjectTypeDescriptor<Comment> descriptor)
        {

        }
    }
}
