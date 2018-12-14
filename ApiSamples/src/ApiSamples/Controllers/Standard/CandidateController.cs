using ApiSamples.ApiModels;
using ApiSamples.Controllers.Standard.Base;
using ApiSamples.Database;
using ApiSamples.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSamples.Controllers.Standard
{
    public class CandidateController : BaseController
    {
        readonly DatabaseContext Db;

        public CandidateController(
            DatabaseContext db
        )
        {
            Db = db;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var queryFields = Request.Query["fields"];

            var mapper = new MapperConfiguration(
                opts =>
                {
                    var map = opts.CreateMap<Candidate, CandidateApiModel>();

                    map.ForAllMembers(x => x.ExplicitExpansion());
                }
            );

            var candidates = await Db.Set<Candidate>().ProjectTo<CandidateApiModel>(mapper, null, queryFields[0].Split(',').ToArray()).ToListAsync();

            return Ok(candidates);
        }
    }
}