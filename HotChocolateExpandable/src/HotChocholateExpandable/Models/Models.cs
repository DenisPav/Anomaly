using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace HotChocholateExpandable.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }

    public class Blog
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
    }

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

    public class BlogPostApiModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public Guid BlogId { get; set; }
        public Blog Blog { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Tag> BlogPostTags { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public ICollection<BlogPostTag> TagBlogPosts { get; set; }
    }

    public class BlogPostTag
    {
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Surname).IsRequired();
            builder.Property(x => x.Email).IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();
        }
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

    public class BlogPostTagConfig : IEntityTypeConfiguration<BlogPostTag>
    {
        public void Configure(EntityTypeBuilder<BlogPostTag> builder)
        {
            builder.HasKey(x => new { x.BlogPostId, x.TagId });
        }
    }

    public class UserObjectType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            //custom resolver sample
            descriptor.Field("sample").Resolver(_ => "321321");
        }
    }

    public class BlogObjectType : ObjectType<Blog>
    {
        protected override void Configure(IObjectTypeDescriptor<Blog> descriptor)
        {

        }
    }

    public class BlogPostObjectType : ObjectType<BlogPost>
    {
        protected override void Configure(IObjectTypeDescriptor<BlogPost> descriptor)
        {

        }
    }

    public class CommentObjectType : ObjectType<Comment>
    {
        protected override void Configure(IObjectTypeDescriptor<Comment> descriptor)
        {
            
        }
    }

    public class TagObjectType : ObjectType<Tag>
    {
        protected override void Configure(IObjectTypeDescriptor<Tag> descriptor)
        {

        }
    }

    public class BlogPostApiModelObjectType : ObjectType<BlogPostApiModel>
    {
        protected override void Configure(IObjectTypeDescriptor<BlogPostApiModel> descriptor)
        {
            
        }
    }
}
