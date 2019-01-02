using ApiSamples.Domain;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using static Sieve.Services.SievePropertyMapper;

namespace ApiSamples.Services
{
    public class CustomSieveProcessor : SieveProcessor
    {
        public CustomSieveProcessor(
            IOptions<SieveOptions> options
        ) : base(options)
        { }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            mapper.Property<Candidate>(candidate => candidate.Id).SetDefaults();
            mapper.Property<Candidate>(candidate => candidate.Name).SetDefaults();
            mapper.Property<Candidate>(candidate => candidate.Surname).SetDefaults();

            mapper.Property<Match>(match => match.Id).SetDefaults();

            mapper.Property<Company>(company => company.Id).SetDefaults();
            mapper.Property<Company>(company => company.Name).SetDefaults();

            mapper.Property<Position>(position => position.Id).SetDefaults();
            mapper.Property<Position>(position => position.Name).SetDefaults();
            mapper.Property<Position>(position => position.Openings).SetDefaults();

            return base.MapProperties(mapper);
        }
    }

    public static class CustomSieveProcessorExtensions
    {
        public static void SetDefaults<TEntity>(this PropertyFluentApi<TEntity> property)
        {
            property.CanFilter()
                .CanSort();
        }
    }
}
