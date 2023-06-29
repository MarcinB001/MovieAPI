using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class DirectorService
    {
        static List<Director> Directors { get; set; }
        static int nextId;

        static public int NextId
        {
            get { return nextId; }
            set { nextId = value; }
        }


        //static DirectorService() {}

        public static List<Director> GetAll() => Directors;

        public static Director? Get(int id) => Directors.FirstOrDefault(p => p.Id == id);
        public static Director GetFromName(string name) => Directors.FirstOrDefault(p => String.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase) );

        public static List<Director> Get(string movieName) => Directors.Where(p => p.Movies.Any(m => String.Equals(m, movieName, StringComparison.OrdinalIgnoreCase))).ToList();


        public static void Add(Director director)
        {
            director.Id = nextId++;
            Directors.Add(director);
        }

        public static void Delete(int id)
        {
            var director = Get(id);
            if (director is null)
                return;

            Directors.Remove(director);
        }

        public static void Update(Director director)
        {
            var index = Directors.FindIndex(p => p.Id == director.Id);
            if (index == -1)
                return;

            Directors[index] = director;
        }


        public static void Read()
        {
            var filePath = "./DB/directors.json";
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                Directors = JsonConvert.DeserializeObject<List<Director>>(json);
            }
            else
            {
                Directors = new List<Director>();
            }

            if (Directors.Count > 0)
            {
                int maxId = Directors.Max(movie => movie.Id);
                nextId = maxId + 1;
            }
            else
            {
                nextId = 1;
            }

        }

    }
}


