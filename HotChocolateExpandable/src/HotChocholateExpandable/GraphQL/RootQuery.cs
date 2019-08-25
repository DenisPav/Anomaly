using HotChocholateExpandable.Database;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HotChocholateExpandable.GraphQL
{
    public class RootQuery
    {
        public IEnumerable<User> Users([Service]DatabaseContext db)
        {
            return db.Set<User>()
                .AsNoTracking();
        }

        public IEnumerable<Blog> Blogs([Service]DatabaseContext db)
        {
            return db.Set<Blog>()
                .AsNoTracking();
        }

        public IEnumerable<BlogPost> BlogPosts([Service]DatabaseContext db)
        {
            return db.Set<BlogPost>()
                .AsNoTracking();
        }

        public IEnumerable<BlogPostApiModel> BlogPostApiModels([Service]DatabaseContext db)
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
                .Use<ExpandableFieldMiddleware<User>>();

            descriptor.Field(x => x.Blogs(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Blog>>();

            descriptor.Field(x => x.BlogPosts(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPost>>();

            descriptor.Field(x => x.BlogPostApiModels(default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<BlogPostApiModel>>()
                .UsePaging<BlogPostApiModelObjectType>();
        }
    }
}
