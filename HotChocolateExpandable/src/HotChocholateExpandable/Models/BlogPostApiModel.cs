using HotChocholateExpandable.GraphQL;
using HotChocholateExpandable.GraphQL.Middlewares;
using HotChocolate.Types;
using System;
using System.Collections.Generic;

namespace HotChocholateExpandable.Models
{
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

    public class BlogPostApiModelObjectType : ObjectType<BlogPostApiModel>
    {
        protected override void Configure(IObjectTypeDescriptor<BlogPostApiModel> descriptor)
        {
            descriptor.Name("blogPostApiModels");

            descriptor.Field<RootQuery>(x => x.Comments(default))
                .Use<FieldCollectingMiddleware>()
                .Use<ExpandableFieldMiddleware<Comment>>()
                .Name("commentsDifferentResolver");
        }
    }
}
