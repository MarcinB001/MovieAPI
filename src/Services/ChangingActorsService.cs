using System;
using System.Linq;

namespace API
{
    public class ChangingActorsService
    {
        public static void NameUpdate(Actor existingActor, Actor actor)
        {
            foreach (var movieTitle in existingActor.Movies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null)
                {
                    for (int i = 0; i < movie.Actors.Count; i++)
                    {
                        if (movie.Actors[i] == existingActor.Name)
                        {
                            movie.Actors[i] = actor.Name;
                        }
                    }
                }
            }
        }

        public static void MoviesUpdate(Actor existingActor, Actor actor)
        {
            var removedMovies = existingActor.Movies.Except(actor.Movies).ToList();
            var addedMovies = actor.Movies.Except(existingActor.Movies).ToList();

            foreach (var movieTitle in removedMovies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null)
                {
                    movie.Actors.Remove(existingActor.Name);
                }
            }

            foreach (var movieTitle in addedMovies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null && !movie.Actors.Contains(actor.Name))
                {
                    movie.Actors.Add(actor.Name);
                }
            }
        }

        public static void CountUpdate(Actor actor)
        {
            int count = 0;

            foreach (var movieTitle in actor.Movies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null)
                    count++;
            }

            actor.MoviesCount = count;
        }

        public static void MoviesAfterDeletingActor(Actor actor)
        {
            var movies = MovieService.GetAll().Where(p => actor.Movies.Any(m => String.Equals(p.Title, m, StringComparison.OrdinalIgnoreCase))).ToList();

            if (movies != null)
            {
                foreach (Movie movie in movies)
                {
                    movie.Actors.Remove(actor.Name);
                }
            }
        }

        public static void CheckAndUpdateMovies(Actor actor)
        {
            var movies = MovieService.GetAll().Where(p => actor.Movies.Any(m => String.Equals(p.Title, m, StringComparison.OrdinalIgnoreCase))).ToList(); //mozna dac ze nie patrzy na wielkosc liter

            if (movies != null)
            {
                foreach (Movie movie in movies)
                {
                    movie.Actors.Add(actor.Name);
                }
            }
        }
    }
}