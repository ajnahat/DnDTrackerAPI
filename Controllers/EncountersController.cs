using DnDTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncountersController : ControllerBase
    {
        private readonly DnDTrackerContext _context;

        public EncountersController(DnDTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Encounter>>> GetEncounters()
        {
            return await _context.Encounters
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Encounter>> GetEncounter(int id)
        {
            var encounter = await _context.Encounters.FindAsync(id);

            if (encounter == null)
            {
                return NotFound();
            }

            await _context.Entry(encounter)
                .Collection(o => o.Waves)
                .LoadAsync();

            await _context.Entry(encounter)
                .Collection(o => o.Waves)
                .Query()
                .Include(o => o.Monsters)
                .LoadAsync();

            return encounter;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEncounter(int id, Encounter encounter)
        {
            if (id != encounter.EncounterId)
            {
                return BadRequest();
            }

            _context.Entry(encounter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EncounterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Encounter>> PostEncounter([FromBody] PostEncounter args)
        {
            if (args.Waves?.Count == 0)
            {
                return null;
            }

            int i = 1;

            Encounter encounter = new Encounter
            {
                EncounterName = args.EncounterName,
                UserId = args.UserId,
                Waves = args.Waves.Select(o => new Wave() { Monsters = o.Select(p => new Monster(p.Item) { Count = p.Count }).ToList(), Sort = i++ }).ToList(),
                Sort = _context.Encounters.Where(o => o.UserId == args.UserId).Count()
            };

            _context.Encounters.Add(encounter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEncounter), new { id = encounter.EncounterId }, encounter);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Encounter>> DeleteEncounter(int id)
        {
            var encounter = await _context.Encounters.FindAsync(id);
            if (encounter == null)
            {
                return NotFound();
            }

            _context.Encounters.Remove(encounter);
            await _context.SaveChangesAsync();

            return encounter;
        }

        private bool EncounterExists(int id)
        {
            return _context.Encounters.Any(e => e.EncounterId == id);
        }
    }
}