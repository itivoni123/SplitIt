using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplititActorsApi.Data;
using SplititActorsApi.Models;

namespace SplititActorsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/actor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors(
            [FromQuery] string name = "", // Optional parameter with default value
            [FromQuery] int? minRank = null, // Optional parameter with nullable type
            [FromQuery] int? maxRank = null, // Optional parameter with nullable type
            [FromQuery] int page = 1, // Optional parameter with default value
            [FromQuery] int pageSize = 10 // Optional parameter with default value
        )
        {
            var actorsQuery = _context.Actors.AsQueryable();

            // Filter by name if it's provided
            if (!string.IsNullOrEmpty(name))
                actorsQuery = actorsQuery.Where(a => a.Name.Contains(name));

            // Filter by rank if the range is provided
            if (minRank.HasValue)
                actorsQuery = actorsQuery.Where(a => a.Rank >= minRank);

            if (maxRank.HasValue)
                actorsQuery = actorsQuery.Where(a => a.Rank <= maxRank);

            // Paginate results
            var paginatedActors = await actorsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(paginatedActors);
        }

 

        // GET: api/actor/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        // POST: api/actor
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            if (_context.Actors.Any(a => a.Rank == actor.Rank))
                return BadRequest("Actor with this rank already exists.");

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
        }

        // PUT: api/actor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActor(int id, [FromBody] Actor updatedActor)
        {
            if (updatedActor == null || string.IsNullOrWhiteSpace(updatedActor.Name))
            {
                return BadRequest("Invalid request. Actor data is required.");
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            if (_context.Actors.Any(a => a.Rank == updatedActor.Rank && a.Id != id))
            {
                return BadRequest("Actor with this rank already exists.");
            }

            actor.Name = updatedActor.Name;
            actor.Rank = updatedActor.Rank;

            await _context.SaveChangesAsync();
            return Ok(actor);
        }


        // DELETE: api/actor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(a => a.Id == id);
        }
    }
}
