using System;

namespace ApiSamples.Domain
{
    public class Match
    {
        public Guid Id { get; set; }

        public Candidate Candidate { get; set; }
        public Position Position { get; set; }
    }
}