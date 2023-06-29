using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Director
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Movies { get; set; }
        public double DirectorRating { get; set; }
        public int RatedMoviesCount { get; set; }

        public Director()
        {
            DirectorRating = 0;
            RatedMoviesCount = 0;
        }

    }
}
