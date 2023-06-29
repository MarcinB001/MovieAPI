using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class ChangingMoviesService
    {
        public static void DirectorAfterDeletingMovie(Movie movie, List<Director> directors)
        {
            var directorss = directors.Where(p => p.Movies.Any(m => String.Equals(m, movie.Title, StringComparison.OrdinalIgnoreCase))).ToList();

            foreach (Director director in directorss)
            {
                double x = director.DirectorRating * director.RatedMoviesCount;

                director.RatedMoviesCount--;

                if (director.RatedMoviesCount == 0)
                {
                    director.DirectorRating = 0.0;

                }
                else
                {
                    double movieRating;

                    if (movie.Rating == null)
                        movieRating = 0;
                    else
                        movieRating = (double)movie.Rating;

                    x = (x - movieRating) / director.RatedMoviesCount;

                    director.DirectorRating = x;
                }

                var movieId = director.Movies.FindIndex(p => String.Equals(p, movie.Title, StringComparison.OrdinalIgnoreCase));

                director.Movies.RemoveAt(movieId);
            }

        }

        public static void CheckAndUpdateDirectors(Movie movie)
        {
            //int i = 0;

            var directors = DirectorService.GetAll();


            foreach (var directorName in movie.Directors)
            {
                var id = directors.FindIndex(p => String.Equals(p.Name, directorName, StringComparison.OrdinalIgnoreCase));

                if (id != -1)
                {
                    var movieId = directors[id].Movies.FindIndex(p => String.Equals(p, movie.Title, StringComparison.OrdinalIgnoreCase));
                    if (movieId == -1)
                        directors[id].Movies.Add(movie.Title);
                }
                else
                {
                    directors.Add(new Director
                    {
                        Id = DirectorService.NextId,
                        Name = directorName,
                        Movies = new List<string> { movie.Title },
                        DirectorRating = (double)movie.Rating,
                        RatedMoviesCount = 1
                    });
                    DirectorService.NextId++;
                }
            }

        }
        public static void CheckAndUpdateActors(Movie movie)
        {
            //int i = 0;

            var actors = ActorService.GetAll();


            foreach (var actorName in movie.Actors)
            {
                var id = actors.FindIndex(p => String.Equals(p.Name, actorName, StringComparison.OrdinalIgnoreCase));

                if (id != -1)
                {
                    var movieId = actors[id].Movies.FindIndex(p => String.Equals(p, movie.Title, StringComparison.OrdinalIgnoreCase));
                    if (movieId == -1)
                    {
                        actors[id].Movies.Add(movie.Title);
                        actors[id].MoviesCount++;
                    }
                        
                }
                else
                {
                    actors.Add(new Actor
                    {
                        Id = ActorService.NextId,
                        Name = actorName,
                        Movies = new List<string> { movie.Title },
                        MoviesCount = 1
                    });
                    ActorService.NextId++;
                }
            }

        }

        public static void RatingUpdate(Movie movie, Movie existingMovie)
        {
            var directors = DirectorService.Get(movie.Title);

            foreach (Director director in directors)
            {
                if(existingMovie.Rating != null)
                {
                    double z = director.DirectorRating * director.RatedMoviesCount;

                    if(director.RatedMoviesCount > 1)
                    {
                        director.RatedMoviesCount--;
                        director.DirectorRating = (double)((z - existingMovie.Rating) / director.RatedMoviesCount);
                    }else if(director.RatedMoviesCount <= 1)
                    {
                        director.RatedMoviesCount= 0;
                        director.DirectorRating = 0.0;
                    }
                        
                }
                

                if (movie.Rating <= 10.0 && movie.Rating >= 0.0 && director != null && movie.Rating != null)
                {
                    //if (director.RatedMoviesCount == 1)
                    //    director.RatedMoviesCount--;

                    director.DirectorRating = (double)((director.DirectorRating * director.RatedMoviesCount + movie.Rating) / (director.RatedMoviesCount + 1));

                    director.RatedMoviesCount++;
                }
                else
                    movie.Rating = 0.0;

            }
        }

        public static void TitleUpdate(Movie movie, Movie existingMovie) // to tez w rezyserach tytul sie zmienia
        {
            var directors = DirectorService.Get(existingMovie.Title);

            foreach(Director director in directors)
            {
                var id = director.Movies.FindIndex(p => p == existingMovie.Title /*p => String.Equals(p, existingMovie.Title, StringComparison.OrdinalIgnoreCase)*/);
                if (id != -1)
                    director.Movies[id] = movie.Title;
            }

            var actors = ActorService.Get(existingMovie.Title);
            foreach(Actor actor in actors)
            {
                var id = actor.Movies.FindIndex(p => p == existingMovie.Title);
                if (id != -1)
                    actor.Movies[id] = movie.Title;
            }

        }
        public static void DirectorsUpdate(Movie movie, Movie existingMovie) 
        {
            // - zmiana rezysera to musi przypisac film do innego a ze starego wypisac
            // - ale tez moze dodac np 2 rezysera a stary zostaje

            // czyli trzeba porownac listy rezyserow: sprawdzic imiona nazwiska i ich ilosc

            var directors = DirectorService.GetAll();

            var removedDirectors = existingMovie.Directors.Except(movie.Directors).ToList();

            foreach(string directorName in removedDirectors)
            {
                var director = directors.FirstOrDefault(p => String.Equals(p.Name, directorName, StringComparison.OrdinalIgnoreCase));
                if (director != null)
                {
                    director.Movies.Remove(existingMovie.Title);

                    double z = director.DirectorRating * director.RatedMoviesCount;

                    director.RatedMoviesCount--;

                    director.DirectorRating = (double)((z - existingMovie.Rating) / director.RatedMoviesCount);

                }

            }

            var addedDirectors = movie.Directors.Except(existingMovie.Directors).ToList();
            foreach (var directorName in addedDirectors)
            {
                var director = directors.FirstOrDefault(p => String.Equals(p.Name, directorName, StringComparison.OrdinalIgnoreCase));
                if (director != null)
                {
                    director.Movies.Add(movie.Title);
                }
                else
                {
                    directors.Add(new Director
                    {
                        Id = DirectorService.NextId,
                        Name = directorName,
                        Movies = new List<string> { movie.Title },
                        DirectorRating = (double)movie.Rating,
                        RatedMoviesCount = 1
                    });
                    DirectorService.NextId++;
                }
            }
        }

        internal static void ActorAfterDeletingMovie(Movie movie, List<Actor> actors)
        {
            var actorss = actors.Where(p => p.Movies.Any(m => String.Equals(m, movie.Title, StringComparison.OrdinalIgnoreCase))).ToList();

            foreach (Actor actor in actorss)
            {
                actor.MoviesCount--;
                
                var movieId = actor.Movies.FindIndex(p => String.Equals(p, movie.Title, StringComparison.OrdinalIgnoreCase));

                actor.Movies.RemoveAt(movieId);
            }
        }

        public static void ActorsUpdate(Movie movie, Movie existingMovie)  
        {
            // - zmiana aktora to musi przypisac film do innego a ze starego wypisac
            // - ale tez moze dodac np 2 aktora a stary zostaje

            // czyli trzeba porownac listy aktorow: sprawdzic imiona nazwiska i ich ilosc

            var actors = ActorService.GetAll();

            var removedActors = existingMovie.Actors.Except(movie.Actors).ToList();

            foreach (string actorName in removedActors)
            {
                var actor = actors.FirstOrDefault(p => String.Equals(p.Name, actorName, StringComparison.OrdinalIgnoreCase));
                if (actor != null)
                {
                    actor.Movies.Remove(existingMovie.Title);

                    actor.MoviesCount--;
                }

            }

            var addedActors = movie.Actors.Except(existingMovie.Actors).ToList();
            foreach (var actorName in addedActors)
            {
                var actor = actors.FirstOrDefault(p => String.Equals(p.Name, actorName, StringComparison.OrdinalIgnoreCase));
                if (actor != null)
                {
                    actor.Movies.Add(movie.Title);
                    actor.MoviesCount++;
                }
                else
                {
                    actors.Add(new Actor
                    {
                        Id = ActorService.NextId,
                        Name = actorName,
                        Movies = new List<string> { movie.Title },
                        MoviesCount = 1
                    });
                    ActorService.NextId++;
                }
            }
        }
    }
}
