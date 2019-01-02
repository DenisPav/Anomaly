using System.Collections.Generic;

namespace ApiSamples.Domain
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}