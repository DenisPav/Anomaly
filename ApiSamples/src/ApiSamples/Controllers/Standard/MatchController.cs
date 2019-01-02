using ApiSamples.ApiModels;
using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Spectre.Query;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSamples.Controllers.Standard
{
    public class MatchController : BaseController
    {
        readonly DatabaseContext Db;
        readonly IConfigurationProvider ConfigurationProvider;
        readonly ISieveProcessor SieveProcessor;
        readonly IQueryProvider<DatabaseContext> QueryProvider;

        public MatchController(
            DatabaseContext db,
            IConfigurationProvider configurationProvider,
            ISieveProcessor sieveProcessor,
            IQueryProvider<DatabaseContext> queryProvider
        )
        {
            Db = db;
            ConfigurationProvider = configurationProvider;
            SieveProcessor = sieveProcessor;
            QueryProvider = queryProvider;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches Matches", typeof(IEnumerable<MatchApiModel>))]
        public async Task<IActionResult> List([FromQuery, Required]string fields)
        {
            var wantedFields = fields.Split(',')
                .ToArray();

            var positions = await Db.Set<Match>()
                .ProjectTo<MatchApiModel>(ConfigurationProvider, null, wantedFields)
                .ToListAsync();

            return Ok(positions);
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches Matches by id", typeof(MatchApiModel))]
        public async Task<IActionResult> List(Guid id, [FromQuery, Required]string fields)
        {
            var wantedFields = fields.Split(',')
                .ToArray();

            var position = await Db.Set<Match>()
                .Where(x => x.Id == id)
                .ProjectTo<MatchApiModel>(ConfigurationProvider, null, wantedFields)
                .FirstOrDefaultAsync();

            return Ok(position);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> Filtered([FromQuery]SieveModel model)
        {
            var set = Db.Set<Match>();
            var filtered = await SieveProcessor.Apply(model, set)
                .ToListAsync(); ;

            return Ok(filtered);
        }

        [HttpGet("queried")]
        public async Task<IActionResult> Queried([FromQuery]string q)
        {
            var candidates = await QueryProvider.Query<Match>(Db, q)
                .ToListAsync();

            return Ok(candidates);
        }
    }
}