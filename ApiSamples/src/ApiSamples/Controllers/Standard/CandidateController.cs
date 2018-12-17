using ApiSamples.ApiModels;
using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSamples.Controllers.Standard
{
    public class CandidateController : BaseController
    {
        readonly DatabaseContext Db;
        readonly IConfigurationProvider ConfigurationProvider;
        readonly IMapper Mapper;

        public CandidateController(
            DatabaseContext db,
            IConfigurationProvider configurationProvider,
            IMapper mapper
        )
        {
            Db = db;
            ConfigurationProvider = configurationProvider;
            Mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var queryFields = Request.Query["fields"];

            var candidates = await Db.Set<Candidate>().ProjectTo<CandidateApiModel>(ConfigurationProvider, null, queryFields[0].Split(',').ToArray()).ToListAsync();

            return Ok(candidates);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Single(int id)
        {
            var queryFields = Request.Query["fields"];

            var candidate = await Db.Set<Candidate>()
                .Where(x => x.Id == id)
                .ProjectTo<CandidateApiModel>(ConfigurationProvider, null, queryFields[0].Split(',').ToArray())
                .FirstOrDefaultAsync();

            return Ok(candidate);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCandidateApiModel model)
        {
            var mapped = Mapper.Map<Candidate>(model);

            try
            {
                await Db.Set<Candidate>()
                    .AddAsync(mapped);
                await Db.SaveChangesAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("Oh no :(");
            }

            return Ok(Mapper.Map<CandidateApiModel>(mapped));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCandidateApiModel model)
        {
            var candidate = await Db.Set<Candidate>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (candidate == null)
            {
                return NotFound();
            }

            Mapper.Map(model, candidate);

            try
            {

                await Db.SaveChangesAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("Oh no :(");
            }

            return Ok(Mapper.Map<CandidateApiModel>(candidate));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException("Same as other...");
        }
    }
}