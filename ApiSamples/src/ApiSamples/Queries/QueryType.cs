using ApiSamples.ApiModels;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSamples.Queries
{
    public class QueryType
    {
        readonly DatabaseContext Db;
        readonly IConfigurationProvider ConfigurationProvider;

        public QueryType(
            DatabaseContext db,
            IConfigurationProvider configurationProvider
        )
        {
            Db = db;
            ConfigurationProvider = configurationProvider;
        }

        private string[] ParseRequiredFields(IResolverContext ctx)
            => ctx.FieldSelection
                .SelectionSet
                .Selections
                .SelectMany(s =>
                {
                    var fieldNode = s as FieldNode;
                    var fieldName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldNode.Name.Value);

                    if (fieldNode.SelectionSet != null)
                    {

                        return fieldNode.SelectionSet
                            .Selections
                            .Select(x => (x as FieldNode).Name.Value)
                            .Select(x => $"{fieldName}.{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x)}")
                            .ToArray();
                    }

                    return new string[] { fieldName };
                })
                .ToArray();

        public async Task<CandidateApiModel> GetCandidate(int id, IResolverContext context)
        {
            var fields = ParseRequiredFields(context);

            var res = await Db.Set<Candidate>()
                .Where(x => x.Id == id)
                .ProjectTo<CandidateApiModel>(ConfigurationProvider, null, fields)
                .FirstOrDefaultAsync();

            return res;
        }

        public Task<MatchApiModel> GetMatch(Guid id, IResolverContext context)
        {
            var fields = ParseRequiredFields(context);

            return Db.Set<Match>()
                .Where(x => x.Id == id)
                .ProjectTo<MatchApiModel>(ConfigurationProvider, null, fields)
                .FirstOrDefaultAsync();
        }

        public Task<PositionApiModel> GetPosition(int id, IResolverContext context)
        {
            var fields = ParseRequiredFields(context);

            return Db.Set<Position>()
                .Where(x => x.Id == id)
                .ProjectTo<PositionApiModel>(ConfigurationProvider, null, fields)
                .FirstOrDefaultAsync();
        }
    }
}
