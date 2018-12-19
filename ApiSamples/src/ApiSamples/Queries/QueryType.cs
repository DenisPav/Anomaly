using ApiSamples.ApiModels;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;
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

        public Task<CandidateApiModel> GetCandidate(int id, IResolverContext context)
        {
            var fields = context.FieldSelection
                .SelectionSet
                .Selections
                .Select(x => (x as FieldNode).Name.Value)
                .Select(x => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x))
                .ToArray();

            return Db.Set<Candidate>()
                .Where(x => x.Id == id)
                .ProjectTo<CandidateApiModel>(ConfigurationProvider, null, fields)
                .FirstOrDefaultAsync();
        }

        public Task<PositionApiModel> GetPosition(int id, IResolverContext context)
        {
            var fields = context.FieldSelection
                .SelectionSet
                .Selections
                .Select(x => (x as FieldNode).Name.Value)
                .Select(x => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x))
                .ToArray();

            return Db.Set<Position>()
                .Where(x => x.Id == id)
                .ProjectTo<PositionApiModel>(ConfigurationProvider, null, fields)
                .FirstOrDefaultAsync();
        }
    }
}
