using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class ChangingDirectorsService
    {
        public static void CheckAndUpdateMovies(Director director)
        {
            var movies = MovieService.GetAll().Where(p => director.Movies.Any(m => String.Equals(p.Title, m, StringComparison.OrdinalIgnoreCase))).ToList();

            if (movies != null)
            {
                foreach (Movie movie in movies)
                {
                    movie.Directors.Add(director.Name);
                }
            }
        }

        public static void NameUpdate(Director existingDirector, Director director)
        {
            foreach (var movieTitle in existingDirector.Movies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null)
                {
                    for (int i = 0; i < movie.Directors.Count; i++)
                    {
                        if (movie.Directors[i] == existingDirector.Name)
                        {
                            movie.Directors[i] = director.Name;
                        }
                    }
                }
            }
        }

        public static void MoviesUpdate(Director existingDirector, Director director)
        {
            var removedMovies = existingDirector.Movies.Except(director.Movies).ToList();
            var addedMovies = director.Movies.Except(existingDirector.Movies).ToList();

            foreach (var movieTitle in removedMovies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null)
                {
                    movie.Directors.Remove(existingDirector.Name);
                }
            }

            foreach (var movieTitle in addedMovies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null && !movie.Directors.Contains(director.Name))
                {
                    movie.Directors.Add(director.Name);
                }
            }
        }

        public static void RatingUpdate(Director director)
        {
            double sum = 0;
            int count = 0;

            foreach (var movieTitle in director.Movies)
            {
                var movie = MovieService.Get(movieTitle);
                if (movie != null && movie.Rating.HasValue)
                {
                    sum += movie.Rating.Value;
                    count++;
                }
            }

            director.RatedMoviesCount = count;

            if (count > 0)
            {
                director.DirectorRating = sum / count;
            }
            else
            {
                director.DirectorRating = 0;
            }
        }

        public static void MoviesAfterDeletingDirector(Director director)
        {
            var movies = MovieService.GetAll().Where(p => director.Movies.Any(m => String.Equals(p.Title, m, StringComparison.OrdinalIgnoreCase))).ToList();

            if (movies != null)
            {
                foreach (Movie movie in movies)
                {
                    movie.Directors.Remove(director.Name);
                }
            }
        }
    }
}
