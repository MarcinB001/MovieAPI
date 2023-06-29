using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API
{
    [ApiController]
    [Route("[controller]")]
    public class DirectorController : ControllerBase
    {
        public DirectorController() {}

        // GET all action
        [HttpGet]
        public ActionResult<List<Director>> GetAll()
        {
            Db.Read();
            return DirectorService.GetAll();
        }

        // GET by Id action
        [HttpGet("{id}")]
        public ActionResult<Director> Get(int id)
        {
            Db.Read();

            var director = DirectorService.Get(id);

            if (director == null)
                return NotFound();

            return director;
        }

        // GET sorted by rating action
        [HttpGet("sorted-by-rating")]
        public ActionResult<List<Director>> GetSortedByRating()
        {
            Db.Read();

            var directors = DirectorService.GetAll();
            var sortedDirectors = directors.OrderByDescending(director => director.DirectorRating).ToList();

            return sortedDirectors;
        }

        // POST action
        [HttpPost]
        public IActionResult Create(Director director) //mozna dac ze update tez ale to po zrobieniu update //i tez zabezpieczyc rating
        {
            Db.Read();

            var id = DirectorService.GetAll().FindIndex(p => String.Equals(p.Name, director.Name, StringComparison.OrdinalIgnoreCase));

            if(id == -1)
            {
                ChangingDirectorsService.CheckAndUpdateMovies(director);
                ChangingDirectorsService.RatingUpdate(director);
                DirectorService.Add(director);
            }
            else
            {
                var existingDirector = DirectorService.GetFromName(director.Name);

                director.Id = existingDirector.Id;

                //zmiana filmow -> trzeba usunac z filmow jak w nich jest albo dodac jak go nie ma //a jak jest jakis nowy film to chyba nic nie robic najlepiej
                if (existingDirector.Movies != director.Movies)
                {
                    ChangingDirectorsService.MoviesUpdate(existingDirector, director);
                    existingDirector.Movies = director.Movies;
                }
                //trzeba zabezpieczyc zeby nie zmienialo sie ratig i moviecount
                //jakas fukcja ktora bierze oceny filmow z tych co sa wpisane i liczy srednia
                ChangingDirectorsService.RatingUpdate(director);


                DirectorService.Update(director);
            }
            
            Db.Save();
            return CreatedAtAction(nameof(Get), new { id = director.Id }, director);
        }
        // PUT action
        [HttpPut("{id}")]
        public IActionResult Update(int id, Director director)
        {
            Db.Read();
            if (id != director.Id)
                return BadRequest();

            var existingDirector = DirectorService.Get(id);
            if (existingDirector is null)
                return NotFound();

            //zmiana name -> w filmach tez trzeba zmienic
            if(existingDirector.Name != director.Name)
            {
                ChangingDirectorsService.NameUpdate(existingDirector,director);
                existingDirector.Name = director.Name;
            }

            //zmiana filmow -> trzeba usunac z filmow jak w nich jest albo dodac jak go nie ma //a jak jest jakis nowy film to chyba nic nie robic najlepiej
            if(existingDirector.Movies != director.Movies)
            {
                ChangingDirectorsService.MoviesUpdate(existingDirector, director);
                existingDirector.Movies = director.Movies;
            }
            //trzeba zabezpieczyc zeby nie zmienialo sie ratig i moviecount
            //jakas fukcja ktora bierze oceny filmow z tych co sa wpisane i liczy srednia
            ChangingDirectorsService.RatingUpdate(director);


            DirectorService.Update(director);
            Db.Save();

            return NoContent();
        }
        // DELETE action
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) //usuwanie jak jest wpisany w jakims filmie 
        {
            Db.Read();

            var director = DirectorService.Get(id);

            if (director is null)
                return NotFound();

            ChangingDirectorsService.MoviesAfterDeletingDirector(director);
            DirectorService.Delete(id);
            Db.Save();

            return NoContent();
        }
    }

}
