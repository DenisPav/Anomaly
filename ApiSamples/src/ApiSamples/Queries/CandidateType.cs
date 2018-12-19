using ApiSamples.ApiModels;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
