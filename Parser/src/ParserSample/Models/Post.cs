using System;

namespace ParserSample.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
