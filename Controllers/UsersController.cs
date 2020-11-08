using DnDTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DnDTrackerContext _context;

        public UsersController(DnDTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(
                await _context.Users
                .Include(o => o.Encounters)
                    .ThenInclude(o => o.Waves)
                        .ThenInclude(o => o.Monsters)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut()]
        public async Task<IActionResult> PutUser(User user)
        {
            _context.Update(user);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost()]
        public async Task<ActionResult<User>> PostUser([FromBody] PostUserPayload args)
        {
            if (await _context.Users.AnyAsync(o => o.UserName.ToUpper() == args.UserName.ToUpper()))
            {
                return BadRequest("A user already exists by that name.");
            }

            var user = new User() { UserName = args.UserName };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostUser), new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet("{id}/encounters")]
        public async Task<ActionResult<IEnumerable<Encounter>>> GetEncounters(int id)
        {
            var encounters = _context.Encounters
                .Where(o => o.UserId == id)
                .OrderBy(o => o.Sort)
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters);

            await encounters.ForEachAsync(o =>
            {
                o.Waves = o.Waves.OrderBy(p => p.Sort).ToList();
                o.Waves.ForEach(p => p.Monsters = p.Monsters.OrderBy(q => q.Sort).ToList());
            });

            return Ok(await encounters.ToListAsync());
        }
    }
}