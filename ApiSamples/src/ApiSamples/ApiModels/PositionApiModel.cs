using System.Collections.Generic;

namespace ApiSamples.ApiModels
{
    public class PositionApiModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Openings { get; set; }

        public CompanyApiModel Company { get; set; }
        public ICollection<MatchApiModel> Matches { get; set; }
    }
}
