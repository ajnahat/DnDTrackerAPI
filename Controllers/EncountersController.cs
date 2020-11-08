using DnDTrackerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
            return Ok(await _context.Encounters
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Encounter>> GetEncounter(int id)
        {
            var encounter = await _context
                .Encounters
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters)
                .SingleOrDefaultAsync(o => o.EncounterId == id);

            if (encounter == null)
            {
                return BadRequest();
            }

            return Ok(encounter);
        }

        [HttpPut()]
        public async Task<IActionResult> PutEncounter(Encounter encounter)
        {
            await _context.Database.BeginTransactionAsync();

            var server = await _context.Encounters
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters)
                .SingleOrDefaultAsync(o => o.EncounterId == encounter.EncounterId);

            if (server == null)
            {
                return BadRequest("The encounter does not exist.");
            }

            foreach (var wave in server.Waves.ToList())
            {
                var localWave = encounter.Waves.SingleOrDefault(o => o.WaveId == wave.WaveId);
                if (localWave == null)
                {
                    server.Waves.Remove(wave);
                }
                else
                {
                    foreach (var monster in wave.Monsters.ToList())
                    {
                        var localMonster = localWave.Monsters.SingleOrDefault(o => o.Index == monster.Index);
                        if (localMonster == null)
                        {
                            wave.Monsters.Remove(monster);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            server.EncounterName = encounter.EncounterName;
            foreach (var wave in encounter.Waves)
            {
                if (wave.WaveId == 0)
                {
                    server.Waves.Add(wave);
                    continue;
                }

                var serverWave = server.Waves.SingleOrDefault(o => o.WaveId == wave.WaveId);

                if (serverWave != null)
                {
                    serverWave.Sort = wave.Sort;

                    foreach (var monster in wave.Monsters)
                    {
                        if (monster.WaveId == 0)
                        {
                            serverWave.Monsters.Add(monster);
                            continue;
                        }

                        var serverMonster = serverWave.Monsters.SingleOrDefault(o => o.Index == monster.Index);

                        if (serverMonster != null)
                        {
                            serverMonster.Count = monster.Count;
                            serverMonster.Sort = monster.Sort;
                        }
                        else
                        {
                            serverWave.Monsters.Add(monster);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return Ok("Successfully updated encounter.");
        }

        [HttpPost]
        public async Task<ActionResult<Encounter>> PostEncounter([FromBody] PostEncounterPayload args)
        {
            if (args.Waves?.Count == 0)
            {
                return null;
            }

            Encounter encounter = new Encounter
            {
                EncounterName = args.EncounterName,
                UserId = args.UserId,
                Waves = args.Waves,
                Sort = _context.Encounters.Where(o => o.UserId == args.UserId).Count()
            };

            _context.Encounters.Add(encounter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEncounter), new { id = encounter.EncounterId }, encounter);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteEncounter(int id)
        {
            try
            {
                var encounter = await _context.Encounters.FindAsync(id);
                if (encounter == null)
                {
                    return BadRequest();
                }

                _context.Encounters.Remove(encounter);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception)
            {
                if (await _context.Encounters.SingleOrDefaultAsync(o => o.EncounterId == id) == null)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}