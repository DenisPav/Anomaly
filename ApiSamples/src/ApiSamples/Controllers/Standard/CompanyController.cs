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
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSamples.Controllers.Standard
{
    public class CompanyController : BaseController
    {
        readonly DatabaseContext Db;
        readonly IConfigurationProvider ConfigurationProvider;
        readonly ISieveProcessor SieveProcessor;

        public CompanyController(
            DatabaseContext db,
            IConfigurationProvider configurationProvider,
            ISieveProcessor sieveProcessor
        )
        {
            Db = db;
            ConfigurationProvider = configurationProvider;
            SieveProcessor = sieveProcessor;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches Companies", typeof(IEnumerable<CompanyApiModel>))]
        public async Task<IActionResult> List([FromQuery, Required]string fields)
        {
            var wantedFields = fields.Split(',')
                .ToArray();

            var positions = await Db.Set<Company>()
                .ProjectTo<CompanyApiModel>(ConfigurationProvider, null, wantedFields)
                .ToListAsync();

            return Ok(positions);
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches Companies by id", typeof(CompanyApiModel))]
        public async Task<IActionResult> List(int id, [FromQuery, Required]string fields)
        {
            var wantedFields = fields.Split(',')
                .ToArray();

            var position = await Db.Set<Company>()
                .Where(x => x.Id == id)
                .ProjectTo<CompanyApiModel>(ConfigurationProvider, null, wantedFields)
                .FirstOrDefaultAsync();

            return Ok(position);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> Filtered([FromQuery]SieveModel model)
        {
            var set = Db.Set<Company>();
            var filtered = await SieveProcessor.Apply(model, set)
                .ToListAsync(); ;

            return Ok(filtered);
        }
    }
}