using HotChocholateExpandable.Database;
using HotChocholateExpandable.Extensions;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HotChocholateExpandable.GraphQL
{
    public class RootQuery
    {
        public IEnumerable<User> Users(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<User>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }

        public IEnumerable<Blog> Blogs(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<Blog>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }

        public IEnumerable<BlogPost> BlogPosts(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<BlogPost>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }

        public IEnumerable<BlogPostApiModel> BlogPostApiModels(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

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
                })
                .ApplyFields(fieldsToResolve);
        }
    }

    public class RootQueryObject : ObjectType<RootQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
        {
            descriptor.Field(x => x.Users(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>();

            descriptor.Field(x => x.Blogs(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>();

            descriptor.Field(x => x.BlogPosts(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>();

            descriptor.Field(x => x.BlogPostApiModels(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .UsePaging<BlogPostApiModelObjectType>();
        }
    }
}
