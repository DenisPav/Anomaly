using ApiSamples.ApiModels;
using HotChocolate.Types;

namespace ApiSamples.Queries
{
    public class PositionType : ObjectType<PositionApiModel>
    {
        protected override void Configure(IObjectTypeDescriptor<PositionApiModel> descriptor)
        {
            descriptor.Field(f => f.Id).Type<NonNullType<IntType>>();
            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();
            descriptor.Field(f => f.Openings).Type<NonNullType<IntType>>();
        }
    }
}
