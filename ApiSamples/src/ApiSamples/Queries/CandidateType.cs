using ApiSamples.ApiModels;
using HotChocolate.Types;

namespace ApiSamples.Queries
{
    public class CandidateType : ObjectType<CandidateApiModel>
    {
        protected override void Configure(IObjectTypeDescriptor<CandidateApiModel> descriptor)
        {
            descriptor.Field(f => f.Id).Type<NonNullType<IntType>>();
            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();
            descriptor.Field(f => f.Surname).Type<NonNullType<StringType>>();
        }
    }
}
