using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{

    [ApiController]
    [Route("[controller]")]
    public class ActorController : ControllerBase
    {
        public ActorController() { }

        // GET all action
        [HttpGet]
        public ActionResult<List<Actor>> GetAll()
        {
            Db.Read();
            return ActorService.GetAll();
        }

        // GET by Id action
        [HttpGet("{id}")]
        public ActionResult<Actor> Get(int id)
        {
            Db.Read();

            var actor = ActorService.Get(id);

            if (actor == null)
                return NotFound();

            return actor;
        }

        // POST action
        [HttpPost]
        public IActionResult Create(Actor actor) //jak jest w nim film ktory istnieje to trza dodac do filmu i tez nadpisywanie z update
        {
            Db.Read();

            var id = ActorService.GetAll().FindIndex(p => String.Equals(p.Name, actor.Name, StringComparison.OrdinalIgnoreCase));

            if (id == -1)
            {
                ChangingActorsService.CheckAndUpdateMovies(actor);
                ChangingActorsService.CountUpdate(actor);
                ActorService.Add(actor);
            }
            else
            {
                var existingActor = ActorService.GetFromName(actor.Name);

                actor.Id = existingActor.Id;

                //zmiana filmow -> trzeba usunac z filmow jak w nich jest albo dodac jak go nie ma //a jak jest jakis nowy film to chyba nic nie robic najlepiej
                if (existingActor.Movies != actor.Movies)
                {
                    ChangingActorsService.MoviesUpdate(existingActor, actor);
                    existingActor.Movies = actor.Movies;
                }
                //trzeba zabezpieczyc zeby nie zmienialo sie ratig i moviecount
                //jakas fukcja ktora bierze oceny filmow z tych co sa wpisane i liczy srednia
                ChangingActorsService.CountUpdate(actor);


                ActorService.Update(actor);
            }

            Db.Save();
            return CreatedAtAction(nameof(Get), new { id = actor.Id }, actor);
        }
        // PUT action
        [HttpPut("{id}")]
        public IActionResult Update(int id, Actor actor) 
        {
            Db.Read();
            if (id != actor.Id)
                return BadRequest();

            var existingActor = ActorService.Get(id);
            if (existingActor is null)
                return NotFound();

            //jak zmieni name to trzeba w filmach zmienic tez
            if (existingActor.Name != actor.Name)
            {
                ChangingActorsService.NameUpdate(existingActor, actor);
                existingActor.Name = actor.Name;
            }
            //jak zmieni movies to trzeba jak w directorze
            if (existingActor.Movies != actor.Movies)
            {
                ChangingActorsService.MoviesUpdate(existingActor, actor);
                existingActor.Movies = actor.Movies;
            }
            //a count musi byc obliczany zeby nie mozna bylo wpisac
            ChangingActorsService.CountUpdate(actor);

            ActorService.Update(actor);
            Db.Save();

            return NoContent();
        }
        // DELETE action
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) //musi sie tez usunac z filmow
        {
            Db.Read();

            var actor = ActorService.Get(id);

            if (actor is null)
                return NotFound();
            if (actor.Movies != null  && actor.Name != null)
                ChangingActorsService.MoviesAfterDeletingActor(actor);
            ActorService.Delete(id);
            Db.Save();

            return NoContent();
        }
    }
}
