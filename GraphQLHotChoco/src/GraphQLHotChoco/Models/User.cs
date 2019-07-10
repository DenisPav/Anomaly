using GraphQLHotChoco.Database;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQLHotChoco.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);

            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);
        }
    }

    public class RootQuery
    {
        public IEnumerable<string> Strings => new List<string> { "a", "b", "c", "d", "e" };
        public IEnumerable<User> Users(IResolverContext ctx, [Service]DatabaseContext db)
        {
            var users = db.Set<User>().AsNoTracking();
            var fieldsToResolver = ctx.ContextData["fieldsToResolve"] as List<FieldWrapper>;

            var toResolve = fieldsToResolver.Select(x => x.Nested)
                .ToList();

            foreach (var item in toResolve)
            {
                var names = item.Select(x => x.Name)
                    .ToList();

                users = users.SelectMembers(names.ToArray());
            }

            return users;
        }
    }

    public class FieldWrapper
    {
        public string Name { get; set; }
        public List<FieldWrapper> Nested { get; set; } = new List<FieldWrapper>();
    }

    public class RootQueryObjectType : ObjectType<RootQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<RootQuery> descriptor)
        {
            descriptor.Field(query => query.Users(default(IResolverContext), default(DatabaseContext)))
                .Use(next => async ctx =>
                {

                    Console.WriteLine("Middleware");

                    var collected = ctx.FieldSelection
                        .SelectionSet
                        .Selections
                        .Select(selection => selection as FieldNode)
                        .ToList()
                        .Select(GetFields)
                        .ToList();


                    ctx.ContextData["fieldsToResolve"] = collected;

                    await next(ctx);

                })
                .UsePaging<UserObjectType>();

            descriptor.Field(query => query.Strings)
                .UsePaging<StringType>();
        }

        private FieldWrapper GetFields(FieldNode fieldNode)
        {
            var fieldName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldNode.Name.Value);

            var wrapper = new FieldWrapper
            {
                Name = fieldName
            };

            if (fieldNode.SelectionSet != null)
            {
                fieldNode.SelectionSet
                    .Selections
                    .Select(x => x as FieldNode)
                    .Select(nestedFieldNode => GetFields(nestedFieldNode))
                    .ToList()
                    .ForEach(nestedFieldDef => wrapper.Nested.Add(nestedFieldDef));
            }

            return wrapper;
        }
    }

    public class UserObjectType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(user => user.Id).Type<IntType>();
            descriptor.Field(user => user.UserName).Type<StringType>();
            descriptor.Field(user => user.Email).Type<StringType>();
            descriptor.Field(user => user.UserRoles);
        }
    }

    public static partial class QueryableExtensions
    {
        public static IQueryable<T> SelectMembers<T>(this IQueryable<T> source, params string[] memberNames)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var bindings = memberNames
                .Select(name => Expression.PropertyOrField(parameter, name))
                .Select(member => Expression.Bind(member.Member, member));
            var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            var selector = Expression.Lambda<Func<T, T>>(body, parameter);
            return source.Select(selector);
        }
    }
}
