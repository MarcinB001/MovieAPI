using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? Movies { get; set; }
        public int MoviesCount { get; set; }

        public Actor()
        {
            MoviesCount = 0;
        }


    }
}
