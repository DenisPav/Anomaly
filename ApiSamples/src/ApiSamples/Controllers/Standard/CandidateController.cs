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
    public class CandidateController : BaseController
    {
        readonly DatabaseContext Db;
        readonly IConfigurationProvider ConfigurationProvider;
        readonly IMapper Mapper;
        readonly ISieveProcessor SieveProcessor;

        public CandidateController(
            DatabaseContext db,
            IConfigurationProvider configurationProvider,
            IMapper mapper,
            ISieveProcessor sieveProcessor
        )
        {
            Db = db;
            ConfigurationProvider = configurationProvider;
            Mapper = mapper;
            SieveProcessor = sieveProcessor;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches candidates", typeof(IEnumerable<CandidateApiModel>))]
        public async Task<IActionResult> List([FromQuery, Required]string fields)
        {
            var queryFields = fields.Split(',')
                .ToArray();

            var candidates = await Db.Set<Candidate>().ProjectTo<CandidateApiModel>(ConfigurationProvider, null, queryFields).ToListAsync();

            return Ok(candidates);
        }

        [HttpGet("{id:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Fetches candidate by id", typeof(CandidateApiModel))]
        public async Task<IActionResult> Single(int id, [FromQuery, Required]string fields)
        {
            var queryFields = fields.Split(',')
                .ToArray();

            var candidate = await Db.Set<Candidate>()
                .Where(x => x.Id == id)
                .ProjectTo<CandidateApiModel>(ConfigurationProvider, null, queryFields)
                .FirstOrDefaultAsync();

            return Ok(candidate);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> Filtered([FromQuery]SieveModel model)
        {
            var set = Db.Set<Candidate>();
            var filtered = await SieveProcessor.Apply(model, set)
                .ToListAsync(); ;

            return Ok(filtered);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateCandidateApiModel model)
        //{
        //    var mapped = Mapper.Map<Candidate>(model);

        //    try
        //    {
        //        await Db.Set<Candidate>()
        //            .AddAsync(mapped);
        //        await Db.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("Oh no :(");
        //    }

        //    return Ok(Mapper.Map<CandidateApiModel>(mapped));
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, UpdateCandidateApiModel model)
        //{
        //    var candidate = await Db.Set<Candidate>()
        //        .FirstOrDefaultAsync(x => x.Id == id);

        //    if (candidate == null)
        //    {
        //        return NotFound();
        //    }

        //    Mapper.Map(model, candidate);

        //    try
        //    {

        //        await Db.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("Oh no :(");
        //    }

        //    return Ok(Mapper.Map<CandidateApiModel>(candidate));
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    throw new NotImplementedException("Same as other...");
        //}
    }
}