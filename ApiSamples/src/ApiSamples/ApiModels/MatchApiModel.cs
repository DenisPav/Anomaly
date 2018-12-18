using System;

namespace ApiSamples.ApiModels
{
    public class MatchApiModel
    {
        public Guid? Id { get; set; }

        public CandidateApiModel Candidate { get; set; }
        public PositionApiModel Position { get; set; }
    }
}
