using ApiSamples.Domain;
using System.Collections.Generic;

namespace ApiSamples.ApiModels
{
    public class CandidateApiModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public IEnumerable<Match> Matches { get; set; }
    }
}
