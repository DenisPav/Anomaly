using HotChocholateExpandable.Database;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotChocholateExpandable.GraphQL
{
    public class RootQuery
    {
        public IQueryable<User> Users([Service]DatabaseContext db)
        {
            return db.Set<User>()
                .AsNoTracking();
        }

        public IQueryable<Blog> Blogs([Service]DatabaseContext db)
        {
            return db.Set<Blog>()
                .AsNoTracking();
        }

        public IQueryable<BlogPost> BlogPosts([Service]DatabaseContext db)
        {
            return db.Set<BlogPost>()
                .AsNoTracking();
        }


        public IQueryable<Tag> Tags([Service]DatabaseContext db)
        {
            return db.Set<Tag>()
                .AsNoTracking();
        }

        public IQueryable<BlogPostApiModel> BlogPostApiModels([Service]DatabaseContext db)
        {
            return db.Set<BlogPost>()
                .AsNoTracking()
                .Select(blog => new BlogPostApiModel
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Description = blog.Description,
                    Content = blog.Content,
                    BlogId = blog.BlogId,
                    Blog = blog.Blog,
                    Comments = blog.Comments,
                    BlogPostTags = blog.BlogPostTags.Select(blogBlogPostTag => blogBlogPostTag.Tag)
                });
        }
    }

    public class RootQueryObject : ObjectType<RootQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
        {
            descriptor.Field(x => x.Users(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<User>>()
                .UseFiltering();

            descriptor.Field(x => x.Blogs(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Blog>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPosts(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPost>>()
                .UseFiltering();

            descriptor.Field(x => x.Tags(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Tag>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPostApiModels(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPostApiModel>>()
                .UsePaging<BlogPostApiModelObjectType>()
                .UseFiltering();
        }
    }
}
