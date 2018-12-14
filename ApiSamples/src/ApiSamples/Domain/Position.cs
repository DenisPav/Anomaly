using System.Collections.Generic;

namespace ApiSamples.Domain
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Openings { get; set; }

        public Company Company { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}