using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<string>? Directors { get; set; }
        //public DateTime ReleaseDate { get; set; }
        public List<string> Actors { get; set; }
        public string TrailerUrl { get; set; }
        public double? Rating { get; set; }
    }
}
