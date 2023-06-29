using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class MovieService
    {
        static List<Movie> Movies { get; set; }
        
        static int nextId;

        //static MovieService() {}

        public static List<Movie> GetAll() => Movies;

        public static Movie? Get(int id) => Movies.FirstOrDefault(p => p.Id == id);

        public static Movie Get(string movieName) => Movies.FirstOrDefault(p => String.Equals(p.Title, movieName, StringComparison.OrdinalIgnoreCase));



        public static void Add(Movie movie)
        {

            var id = Movies.FindIndex(p => String.Equals(p.Title, movie.Title, StringComparison.OrdinalIgnoreCase));

            if (id != -1)
                return;

            movie.Id = nextId++;
            Movies.Add(movie);
        }

        public static void Delete(int id)
        {
            var movie = Get(id);
            if (movie is null)
                return;

            Movies.Remove(movie);

        }

        public static void Update(Movie movie)
        {
            var index = Movies.FindIndex(p => p.Id == movie.Id);
            if (index == -1)
                return;

            Movies[index] = movie;
        }

        public static void Rate(int id,int rate)
        {
            var index = Movies.FindIndex(p => p.Id == id);
            if (index == -1)
                return;

            Movies[index].Rating = rate;

        }
        public static void Read()
        {
            var filePath = "./DB/movies.json";
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                Movies = JsonConvert.DeserializeObject<List<Movie>>(json);
            }
            else
            {
                Movies = new List<Movie>();
            }

            if (Movies.Count > 0)
            {
                int maxId = Movies.Max(movie => movie.Id);
                nextId = maxId + 1;
            }
            else
            {
                nextId = 1;
            }

        }
    }
}
