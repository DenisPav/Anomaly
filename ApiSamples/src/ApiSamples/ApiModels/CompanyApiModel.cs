using System.Collections.Generic;

namespace ApiSamples.ApiModels
{
    public class CompanyApiModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<PositionApiModel> Positions { get; set; }
    }
}
