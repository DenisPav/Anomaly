using GraphQLHotChoco.Database;
using GraphQLHotChoco.Models;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLHotChoco.Queries
{
    //public class UserType : ObjectType<UserApiModel>
    //{
    //    protected override void Configure(IObjectTypeDescriptor<UserApiModel> descriptor)
    //    {
    //        descriptor.Field(f => f.Id).Type<NonNullType<IntType>>();
    //        descriptor.Field(f => f.UserName).Type<StringType>();
    //        descriptor.Field(f => f.Email).Type<StringType>();
    //        //descriptor.Field(f => f.UserRoles).Type<ListType<ObjectType<UserRolesApiModel>>>().Resolver(bozjiKurac => bozjiKurac.Parent<IQueryable<UserApiModel>>().Select(x => x.UserRoles).ToList());
    //        descriptor.Field(x => x.GetUsers(default(IResolverContext), default(DatabaseContext)))
    //            .Type<UserType>();
    //    }
    //}

    //public class UserType22 : ObjectType<IEnumerable<UserApiModel>>
    //{

    //}

    //public class UserRolesType : ObjectType<UserRolesApiModel>
    //{
    //    protected override void Configure(IObjectTypeDescriptor<UserRolesApiModel> descriptor)
    //    {
    //        descriptor.Field(f => f.UserId).Type<IntType>();
    //        descriptor.Field(f => f.RoleId).Type<IntType>();
    //    }
    //}
}
