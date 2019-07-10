using GraphQLHotChoco.Database;
using GraphQLHotChoco.Extensions;
using GraphQLHotChoco.GraphQLMiddlewares;
using GraphQLHotChoco.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQLHotChoco.Queries
{
    public class RootQuery
    {
        public IEnumerable<string> Strings => new List<string> { "a", "b", "c", "d", "e" };
        public IEnumerable<User> Users(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<User>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }

        public IEnumerable<UserRoles> UserRoles(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<UserRoles>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }

        public IEnumerable<Role> Roles(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var fieldsToResolve = ctx.ContextData[FieldCollectingMiddleware.DataKey] as List<FieldWrapper>;

            return db.Set<Role>()
                .AsNoTracking()
                .ApplyFields(fieldsToResolve);
        }
    }

    public class RootQueryObjectType : ObjectType<RootQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
        {
            descriptor.Field(query => query.Users(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .UsePaging<UserObjectType>();

            descriptor.Field(query => query.UserRoles(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .UsePaging<UserRolesObjectType>();

            descriptor.Field(query => query.Roles(default(IResolverContext), default(DatabaseContext)))
                .Use<FieldCollectingMiddleware>()
                .UsePaging<RoleObjectType>();

            descriptor.Field(query => query.Strings)
                .UsePaging<StringType>();
        }
    }
}
