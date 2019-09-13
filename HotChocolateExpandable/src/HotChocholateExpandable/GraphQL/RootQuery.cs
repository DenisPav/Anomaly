using HotChocholateExpandable.Database;
using HotChocholateExpandable.Extensions;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace HotChocholateExpandable.GraphQL
{
    public class RootQuery
    {
        public IQueryable<User> Users(IResolverContext ctx)
        {
            return ctx.GetService<DatabaseContext>()
                .Set<User>()
                .AsNoTracking();
        }

        public IQueryable<Blog> Blogs(IResolverContext ctx)
        {
            return ctx.GetService<DatabaseContext>()
                .Set<Blog>()
                .AsNoTracking();
        }

        public IQueryable<BlogPost> BlogPosts(IResolverContext ctx)
        {
            return ctx.GetService<DatabaseContext>()
                .Set<BlogPost>()
                .AsNoTracking();
        }


        public IQueryable<Tag> Tags(IResolverContext ctx)
        {
            return ctx.GetService<DatabaseContext>()
                .Set<Tag>()
                .AsNoTracking();
        }

        public IQueryable<BlogPostApiModel> BlogPostApiModels(IResolverContext ctx)
        {
            return ctx.GetService<DatabaseContext>()
                .Set<BlogPost>()
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
            descriptor.Field(x => x.Users(default(IResolverContext)))
                .Use<FreshServiceScopeMiddleware>()
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<User>>()
                .UseFiltering();

            descriptor.Field(x => x.Blogs(default(IResolverContext)))
                .Use<FreshServiceScopeMiddleware>()
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Blog>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPosts(default(IResolverContext)))
                .Use<FreshServiceScopeMiddleware>()
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPost>>()
                .UseFiltering();

            descriptor.Field(x => x.Tags(default(IResolverContext)))
                .Use<FreshServiceScopeMiddleware>()
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Tag>>()
                .UseFiltering();

            descriptor.Field(x => x.BlogPostApiModels(default(IResolverContext)))
                .Use<FreshServiceScopeMiddleware>()
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPostApiModel>>()
                .UsePaging<BlogPostApiModelObjectType>()
                .UseFiltering();
        }
    }
}
