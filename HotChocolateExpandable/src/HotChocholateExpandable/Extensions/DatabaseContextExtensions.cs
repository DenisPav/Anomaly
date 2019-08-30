using Bogus;
using HotChocholateExpandable.Database;
using HotChocholateExpandable.Models;
using System.Collections.Generic;
using System.Linq;

namespace HotChocholateExpandable.Extensions
{
    public static class DatabaseContextExtensions
    {
        public static void Seed(this DatabaseContext db)
        {
            if (!db.Set<Blog>().Any())
            {
                var (users, blogs, blogPosts, comments, tags, blogPostTags) = CreateData();

                db.Set<Tag>().AddRange(tags);
                db.SaveChanges();

                db.Set<User>().AddRange(users);
                db.SaveChanges();

                db.Set<Blog>().AddRange(blogs);
                db.SaveChanges();

                db.Set<BlogPost>().AddRange(blogPosts);
                db.SaveChanges();

                db.Set<Comment>().AddRange(comments);
                db.SaveChanges();

                db.Set<BlogPostTag>().AddRange(blogPostTags);
                db.SaveChanges();
            }
        }

        private static (IEnumerable<User> users, IEnumerable<Blog> blogs, IEnumerable<BlogPost> blogPosts, IEnumerable<Comment> comments, IEnumerable<Tag> tags, IEnumerable<BlogPostTag> blogPostTags) CreateData()
        {
            var userId = 0;
            var users = new Faker<User>()
                .RuleFor(user => user.Id, f => ++userId)
                .RuleFor(user => user.Name, f => f.Name.FirstName())
                .RuleFor(user => user.Surname, f => f.Name.LastName())
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name, user.Surname))
                .Generate(100);

            var blogs = new Faker<Blog>()
                .RuleFor(blog => blog.Id, f => f.Random.Guid())
                .RuleFor(blog => blog.Name, f => f.Name.JobTitle())
                .RuleFor(blog => blog.OwnerId, f => users[f.Random.Number(99)].Id)
                .Generate(1000);

            var blogPostId = 0;
            var blogPosts = new Faker<BlogPost>()
                .RuleFor(blogPost => blogPost.Id, f => ++blogPostId)
                .RuleFor(blogPost => blogPost.Title, f => f.Lorem.Word())
                .RuleFor(blogPost => blogPost.Description, f => f.Lorem.Sentence())
                .RuleFor(blogPost => blogPost.Content, f => f.Lorem.Text())
                .RuleFor(blogPost => blogPost.BlogId, f => blogs[f.Random.Number(999)].Id)
                .Generate(200);

            var comments = new Faker<Comment>()
                .RuleFor(comment => comment.Id, f => f.Random.Guid())
                .RuleFor(comment => comment.Text, f => f.Lorem.Sentence())
                .RuleFor(comment => comment.BlogPostId, f => blogPosts[f.Random.Number(199)].Id)
                .RuleFor(comment => comment.OwnerId, f => users[f.Random.Number(99)].Id)
                .Generate(350);

            var tagId = 0;
            var tags = new Faker<Tag>()
                .RuleFor(tag => tag.Id, f => ++tagId)
                .RuleFor(tag => tag.TagName, f => f.Lorem.Word())
                .Generate(80);

            var blogPostTags = new Faker<BlogPostTag>()
                .RuleFor(blogPostTag => blogPostTag.BlogPostId, f => blogPosts[f.Random.Number(199)].Id)
                .RuleFor(blogPostTag => blogPostTag.TagId, f => tags[f.Random.Number(79)].Id)
                .Generate(150);

            return (
                users,
                blogs,
                blogPosts,
                comments,
                tags,
                blogPostTags
            );
        }
    }
}
