using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {

        private readonly ILogger<MovieController> _logger;
        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
        }


        // GET all action
        [HttpGet]
        public ActionResult<List<Movie>> GetAll()
        {
            Db.Read();
            return MovieService.GetAll();
        }

        

        // GET by Id action
        [HttpGet("{id}")]
        public ActionResult<Movie> Get(int id)
        {
            Db.Read();

            var movie = MovieService.Get(id);

            if (movie == null)
                return NotFound();

            return movie;
        }

        // GET sorted by rating action
        [HttpGet("sorted-by-rating")]
        public ActionResult<List<Movie>> GetSortedByRating()
        {
            Db.Read();

            var movies = MovieService.GetAll();
            var sortedMovies = movies.OrderByDescending(movie => movie.Rating).ToList();

            return sortedMovies;
        }

        // POST action
        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            Db.Read();

            var directors = DirectorService.Get(movie.Title);

            var newDirectors = DirectorService.GetAll().Where(director => !directors.Contains(director) && movie.Directors.Contains(director.Name)).ToList();
            directors.AddRange(newDirectors);

            var x = MovieService.GetAll().FindIndex(p => String.Equals(p.Title, movie.Title, StringComparison.OrdinalIgnoreCase));

            

            if (movie.Rating > 10.0 || movie.Rating < 0.0 || movie.Rating == null)
                movie.Rating = 0.0;


            if (x == -1) //nowy film
            {
                ChangingMoviesService.CheckAndUpdateDirectors(movie);
                ChangingMoviesService.CheckAndUpdateActors(movie);
                if (movie.Rating <= 10.0 && movie.Rating >= 0.0 && directors != null)
                {
                    foreach(Director director in directors)
                    {
                        director.DirectorRating = (double)((director.DirectorRating * director.RatedMoviesCount + movie.Rating) / (director.RatedMoviesCount + 1));
                        director.RatedMoviesCount++;
                    }
                }
                else
                    movie.Rating = null;

                MovieService.Add(movie);
            }
            else if (x != -1) //proba wpisania tego samego filmu (tj. ten sam tytul ) 2 raz
            {
                var existingMovie = MovieService.Get(movie.Title);

                movie.Id = existingMovie.Id;

                if (existingMovie.Rating != movie.Rating)
                {
                    if (movie.Rating > 10 || movie.Rating < 0)
                        movie.Rating = 0.0;

                    ChangingMoviesService.RatingUpdate(movie, existingMovie);
                    existingMovie.Rating = movie.Rating;
                }

                if (existingMovie.Directors != movie.Directors)
                {
                    ChangingMoviesService.DirectorsUpdate(movie, existingMovie);
                    existingMovie.Directors = movie.Directors;
                }

                if (existingMovie.Actors != movie.Actors)
                {
                    ChangingMoviesService.ActorsUpdate(movie, existingMovie);
                    existingMovie.Actors = movie.Actors;
                }

                MovieService.Update(movie);

            }

            Db.Save();

            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        [HttpPost("{id}/rate")]
        public IActionResult RateFilm(int id)
        {
            Db.Read();

            //losowa ocena dla filmu
            var random = new Random(Guid.NewGuid().GetHashCode());
            var ratingValue = random.Next(1, 11); //ocena od 1 do 10

            var existingMovie = MovieService.Get(id);

            if (existingMovie is null)
                return NotFound();

            //MovieService.Rate(id,ratingValue);

            Movie movie = new Movie()
            {
                Id = existingMovie.Id,
                Title = existingMovie.Title,
                Category = existingMovie.Category,
                Directors = existingMovie.Directors,
                Actors = existingMovie.Actors,
                TrailerUrl = existingMovie.TrailerUrl,
                Rating = ratingValue
            };


            if (MovieService.Get(id).Rating != ratingValue)
            {
                ChangingMoviesService.RatingUpdate(movie, existingMovie);
                existingMovie.Rating = movie.Rating;
            }
            MovieService.Update(existingMovie);

            Db.Save();

            return NoContent(); 
        }


        // PUT action
        [HttpPut("{id}")]
        public IActionResult Update(int id, Movie movie)
        {
            Db.Read();

            if (id != movie.Id)
                return BadRequest();

            var existingMovie = MovieService.Get(id);
            if (existingMovie is null)
                return NotFound();

            if (MovieService.GetAll().Any(p => String.Equals(p.Title, movie.Title, StringComparison.OrdinalIgnoreCase)))
                return Conflict();


            if (MovieService.Get(id).Title != movie.Title)
            {
                ChangingMoviesService.TitleUpdate(movie, existingMovie);
                existingMovie.Title = movie.Title;
            }

            if (MovieService.Get(id).Rating != movie.Rating)
            {
                ChangingMoviesService.RatingUpdate(movie, existingMovie);
                existingMovie.Rating = movie.Rating;
            }

            if (MovieService.Get(id).Directors != movie.Directors)
            {
                ChangingMoviesService.DirectorsUpdate(movie, existingMovie);
                existingMovie.Directors = movie.Directors;
            }

            if (existingMovie.Actors != movie.Actors)
            {
                ChangingMoviesService.ActorsUpdate(movie, existingMovie);
                existingMovie.Actors = movie.Actors;
            }

            MovieService.Update(movie);

            Db.Save();

            return NoContent();
        }
        // DELETE action
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Db.Read();

            var movie = MovieService.Get(id);

            if (movie is null)
                return NotFound();

            ChangingMoviesService.DirectorAfterDeletingMovie(movie, DirectorService.GetAll());
            ChangingMoviesService.ActorAfterDeletingMovie(movie, ActorService.GetAll());

            MovieService.Delete(id);

            Db.Save();

            return NoContent();
        }
    }

}
