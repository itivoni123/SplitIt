using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplititActorsApi.Data;
using SplititActorsApi.Models;
using SplititActorsApi.Services; // Import the AuthService namespace

namespace SplititActorsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService; // Add AuthService dependency

        public ActorController(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: api/actor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors(
            [FromQuery] string name = "", 
            [FromQuery] int? minRank = null, 
            [FromQuery] int? maxRank = null, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10 
        )
        {
            var actorsQuery = _context.Actors.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                actorsQuery = actorsQuery.Where(a => a.Name.Contains(name));

            if (minRank.HasValue)
                actorsQuery = actorsQuery.Where(a => a.Rank >= minRank);

            if (maxRank.HasValue)
                actorsQuery = actorsQuery.Where(a => a.Rank <= maxRank);

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
                return BadRequest("Invalid request. Actor data is required.");

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();

            if (_context.Actors.Any(a => a.Rank == updatedActor.Rank && a.Id != id))
                return BadRequest("Actor with this rank already exists.");

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

        // Authentication Endpoint to Get Token
        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Failed to obtain token.");
                }
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
