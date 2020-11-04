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
            return await _context.Users
                .Include(o => o.Encounters)
                    .ThenInclude(o => o.Waves)
                        .ThenInclude(o => o.Monsters)
                .ToListAsync();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpPost()]
        public async Task<ActionResult<User>> PostUser([FromBody] PostUser args)
        {
            if (await _context.Users.AnyAsync(o => o.UserName.ToUpper() == args.UserName.ToUpper()))
            {
                return BadRequest("A user already exists by that name.");
            }

            var user = new User() { UserName = args.UserName };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUser", new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        [HttpGet("{id}/encounters")]
        public async Task<ActionResult<IEnumerable<Encounter>>> GetEncounters(int id)
        {
            try
            {
                var encounters = _context.Encounters.Where(o => o.UserId == id)
                    .OrderBy(o => o.Sort)
                    .Include(o => o.Waves)
                        .ThenInclude(o => o.Monsters);

                await encounters.ForEachAsync(o =>
                {
                    o.Waves = o.Waves.OrderBy(p => p.Sort).ToList();
                    o.Waves.ForEach(p => p.Monsters = p.Monsters.OrderBy(q => q.Sort).ToList());
                });


                return await encounters.ToListAsync(); ;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}