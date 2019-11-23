using HotChocholateExpandable.Database;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HotChocholateExpandable.GraphQL
{
    public class RootQuery
    {
        public IQueryable<User> Users(IResolverContext ctx, [Service] DatabaseContext db)
        {
            return db.Set<User>()
                .AsNoTracking();
        }

        public IQueryable<Blog> Blogs(IResolverContext ctx, [Service] DatabaseContext db)
        {
            return db.Set<Blog>()
                .AsNoTracking();
        }

        public IQueryable<BlogPost> BlogPosts(IResolverContext ctx, [Service] DatabaseContext db)
        {
            return db.Set<BlogPost>()
                .AsNoTracking();
        }


        public IQueryable<Tag> Tags(IResolverContext ctx, [Service] DatabaseContext db)
        {
            return db.Set<Tag>()
                .AsNoTracking();
        }

        public IQueryable<BlogPostApiModel> BlogPostApiModels(IResolverContext ctx, [Service] DatabaseContext db)
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

        public IEnumerable<Comment> Comments([Service] DatabaseContext db)
        {
            return db.Set<Comment>();
        }
    }

    public class RootQueryObject : ObjectType<RootQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
        {
            descriptor.Field(x => x.Users(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<User>>()
                .UseFiltering();

            descriptor.Field(x => x.Blogs(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Blog>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPosts(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPost>>()
                .UseFiltering();

            descriptor.Field(x => x.Tags(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Tag>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPostApiModels(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPostApiModel>>();

            descriptor.Field(x => x.Comments(default))
                .Ignore();
        }
    }
}
