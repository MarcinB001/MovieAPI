using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class ActorService
    {
        static List<Actor> Actors { get; set; }
        static int nextId;

        public static int NextId
        {
            get { return nextId; }
            set { nextId = value; }
        }


        public static List<Actor> GetAll() => Actors;

        public static Actor? Get(int id) => Actors.FirstOrDefault(p => p.Id == id);
        public static Actor GetFromName(string name) => Actors.FirstOrDefault(p => String.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

        public static List<Actor> Get(string movieName) => Actors.Where(p => p.Movies.Any(m => String.Equals(m, movieName, StringComparison.OrdinalIgnoreCase))).ToList();


        public static void Add(Actor actor)
        {
            actor.Id = nextId++;
            Actors.Add(actor);
        }

        public static void Delete(int id)
        {
            var actor = Get(id);
            if (actor is null)
                return;

            Actors.Remove(actor);
        }

        public static void Update(Actor actor)
        {
            var index = Actors.FindIndex(p => p.Id == actor.Id);
            if (index == -1)
                return;

            Actors[index] = actor;
        }

        public static void Read()
        {
            var filePath = "./DB/actors.json";
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                Actors = JsonConvert.DeserializeObject<List<Actor>>(json);
            }
            else
            {
                Actors = new List<Actor>();
            }

            if (Actors.Count > 0)
            {
                int maxId = Actors.Max(movie => movie.Id);
                nextId = maxId + 1;
            }
            else
            {
                nextId = 1;
            }

        }

    }
}

