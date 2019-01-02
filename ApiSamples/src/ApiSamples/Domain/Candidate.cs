using Sieve.Attributes;
using System.Collections.Generic;

namespace ApiSamples.Domain
{
    public class Candidate
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Surname { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}